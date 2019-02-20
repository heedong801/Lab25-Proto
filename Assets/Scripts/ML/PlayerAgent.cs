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
    private PlayerManager _PlayerManager;

    // Agent property.
    public float turnSpeed = 300;
    public float moveSpeed = 2;

    public static bool isKill = false;
    public static bool isShotMiss = false;

    public Transform shootPoint;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRb = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        myWeaponCtrl = GetComponentInChildren<AgentWeaponCtrl>();
 
        rayPer = GetComponent<RayPerception>();

        playerTr = GameObject.Find("Player").transform;
        _PlayerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }

    public override void CollectObservations()
    {
        var rayDistance = 12f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };

        var detectableObjects = new[] { "Player", "Infectee", "wall" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 1.5f, 0f));
        AddVectorObs(PlayerManager.hp / 100);
        AddVectorObs(AgentManager.hp / 100);
        AddVectorObs(shootPoint.forward);
        AddVectorObs(playerTr.position);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);

        RewardFunctionFarToTarget();

        if (AgentManager.hp <= 0 )
        {
            Debug.Log("agentDie");
            AddReward(-0.9f);
            Done();
        }

        if( _PlayerManager.isEnd )
        {
            Debug.Log("Clear");
            AddReward(1.0f);
            Done();
        }

        if ( PlayerManager.isHit )
        {
            AddReward(-0.5f / agentParameters.maxStep);
            PlayerManager.isHit = true;
        }

        if ( transform.position.y < - 10 )
        {
            Debug.Log("agentDie");
            Done();
            AddReward(-1.0f);
        }

        if (isKill)
        {
            Debug.Log("Hit");
            AddReward(0.15f / agentParameters.maxStep);
            isKill = false;
        }

        if( isShotMiss )
        {
            Debug.Log("ShotMiss");
            AddReward(-0.3f / agentParameters.maxStep);
            isShotMiss = false;
        }

        if ( PlayerManager.hp <= 0 && PlayerManager.armor <= 0)
        {
            Debug.Log("playerDie");
            AddReward(-1.0f);
            Done();
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
        
        transform.position = new Vector3(-7, 1, 16);
        //_PlayerManager.TeleportPos = new Vector3(Random.Range(-9, 9), 1, Random.Range(-7, 17));
        //_PlayerManager.teleportFlag = true;
        playerTr.position = new Vector3(0,1,-6);
        AgentManager.hp = 100;
        AgentManager.armor = 100;
        PlayerManager.hp = 100;
        PlayerManager.armor = 100;
        
    }

    public override void AgentOnDone()
    {

    }

    void RewardFunctionFarToTarget()
    {
        if( Vector3.Distance(transform.position, playerTr.position) > 3)
            AddReward(-0.01f / agentParameters.maxStep);
        else
            AddReward(0.01f / agentParameters.maxStep);
    }
}
