using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAgent : Agent {

    private Rigidbody agentRb;
    private bool isHealing = false;
    private RayPerception rayPer;
    private AgentWeaponCtrl myWeaponCtrl;
   
    private Transform playerTr;
    // Agent property.
    public float turnSpeed = 300;
    public float moveSpeed = 2;

    public static bool isKill = false;
    public static bool isShotMiss = false;


    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRb = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        myWeaponCtrl = GetComponentInChildren<AgentWeaponCtrl>();
 
        rayPer = GetComponent<RayPerception>();

        playerTr = GameObject.Find("Player").transform;
    }

    public override void CollectObservations()
    {
        var rayDistance = 12f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };

        var detectableObjects = new[] { "Player", "Infectee", "wall" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 1.5f, 0f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);

        RewardFunctionFarToTarget();

        if (AgentManager.hp <= 0)
        {
            Done();
            AddReward(-1.0f);
        }

        if( transform.position.y < - 10 )
        {
            Done();
            AddReward(-1.0f);
        }

        if (isKill)
        {
            AddReward(0.01f);
            isKill = false;
        }

        if( isShotMiss )
        {
            AddReward(-0.001f);
            isKill = false;
        }

        if ( PlayerManager.hp <= 0 )
        {
            Debug.Log("ASD");
            Done();
            AddReward(-1.0f);

            PlayerManager.hp = 100;
            PlayerManager.armor = 100;
        }
    }
    public void MoveAgent(float[] act)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        
        if ( !isHealing )
        {
            var forwardAxis = (int)act[0];
            var rotateAxis = (int)act[1];
            var shootAxis = (int)act[2];
            switch (forwardAxis)
            {
                case 1:
                    dirToGo = transform.forward;
                    break;
                case 2:
                    dirToGo = -transform.forward;
                    break;
            }

            switch (rotateAxis)
            {
                case 1:
                    rotateDir = -transform.up;
                    break;
                case 2:
                    rotateDir = transform.up;
                    break;
            }

            switch (shootAxis)
            {
                case 1:
                    myWeaponCtrl.Fire();
                    break;
            }
            
            agentRb.AddForce(dirToGo * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        }
       
    }

    public override void AgentReset()
    {
        transform.position = new Vector3(Random.Range(-9, 9), 1, Random.Range(-7, 17));
        playerTr.position = new Vector3(Random.Range(-9, 9), 1, Random.Range(-7, 17));
        AgentManager.hp = 100;
        AgentManager.armor = 100;
        
    }

    public override void AgentOnDone()
    {

    }

    void RewardFunctionFarToTarget()
    {
        if( Vector3.Distance(transform.position, playerTr.position) > 3)
            AddReward(-0.01f);
    }
}
