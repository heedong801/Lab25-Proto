using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class TestPlayerAgent1 : Agent
{
    public Transform[] targets;
    private Rigidbody playerRb;
    public Transform pivotTr;
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

    private Vector3 endPos;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        playerRb = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        myWeaponCtrl = GetComponentInChildren<AgentWeaponCtrl>();

        rayPer = GetComponent<RayPerception>();

        playerTr = GameObject.Find("Player").transform;
        _PlayerManager = GameObject.Find("Player").GetComponent<PlayerManager>();

    }

    public override void CollectObservations()
    {
        var rayDistance = 12f;

        //============================ 2-7 =======================
        //float[] rayAngles = { 0f, 15f, 30f, 45f, 60f, 75f, 90f, 105f, 120f, 135f, 150f, 165f, 180f };

        //var detectableObjects = new[] { "Infectee" };
        //AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        //AddVectorObs(shootPoint.forward);
        //AddVectorObs(playerRb.angularVelocity);

        //============================ 2-13 ======================
        //float[] rayAngles = { 0f, 15f, 30f, 45f, 60f, 75f, 90f, 105f, 120f, 135f, 150f, 165f, 180f };

        //var detectableObjects = new[] { "Infectee", "wall", "Player" };

        //AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        //AddVectorObs(shootPoint.forward);
        //AddVectorObs(playerRb.angularVelocity);

        //============================ 2-14 ~ 15 ======================
        //float[] rayAngles = { 0f, 15f, 30f, 45f, 60f, 75f, 90f, 105f, 120f, 135f, 150f, 165f, 180f };

        //var detectableObjects = new[] { "Infectee", "wall", "Player" };

        //endPos = Vector3.Normalize(transform.TransformDirection(
        //          RayPerception.PolarToCartesian(12, 90)));

        //AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        //AddVectorObs(Vector3.Dot(transform.forward, endPos));
        //AddVectorObs(playerRb.angularVelocity);

        //============================ 2-16 ======================
        //float[] rayAngles = { 0, 90f, 180f, 270f, 360f };

        //var detectableObjects = new[] { "Infectee", "wall", "Player" };

        //endPos = Vector3.Normalize(transform.TransformDirection(
        //          RayPerception.PolarToCartesian(12, 90)));

        //AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        //AddVectorObs(Vector3.Dot(transform.forward, endPos));
        //AddVectorObs(playerRb.angularVelocity);

        //============================ 2-17 ======================
        float[] rayAngles = {85f, 87f, 90f, 92f, 95f};

        var detectableObjects = new[] { "Infectee", "wall", "Player" };

        endPos = Vector3.Normalize(transform.TransformDirection(
                  RayPerception.PolarToCartesian(12, 90)));

        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(Vector3.Dot(transform.forward, endPos));
        //AddVectorObs(playerRb.angularVelocity);


        //AddVectorObs(playerTr.position);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);
        AddReward(-1.0f / agentParameters.maxStep);       

        if (isKill)
        {
            AddReward(1.0f);
            isKill = false;
            ResetTarget();
        }

        if( isShotMiss )
        {
            AddReward(-5.0f / agentParameters.maxStep);
            isShotMiss = false;
        }
        
        if( transform.position.y >= 1.2 || transform.position.y < 0 || transform.rotation.x != 0 || transform.rotation.z != 0)
        {
            Debug.Log("Error");
            Vector3 randomPos = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4, 4f));
            transform.position = randomPos + pivotTr.position;

            transform.rotation = Quaternion.identity;
        }
        //RewardFunctionFarToTarget();

    }
    public void MoveAgent(float[] act)
    {
        // ~2-17 ===============================================================================
        //Vector3 dirToGo = Vector3.zero;
        //Vector3 rotateDir = Vector3.zero;

        //var forwardAxis = (int)act[0];
        //var shootAxis = (int)act[1];

        //switch (forwardAxis)
        //{
        //    case 1:
        //        rotateDir = -transform.up;
        //        break;
        //    case 2:
        //        rotateDir = transform.up;
        //        break;
        //}

        //switch (shootAxis)
        //{
        //    case 1:
        //        myWeaponCtrl.Fire();
        //        break;
        //}

        //playerRb.AddForce(dirToGo * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        //transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

        // 2-17~ ===============================================================================
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        var shootAxis = (int)act[0];

        switch (shootAxis)
        {
            case 1:
                myWeaponCtrl.Fire();
                break;
        }

        //playerRb.AddForce(dirToGo * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        //transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);


    }

    public override void AgentReset()
    {
        isKill = false;
        isShotMiss = false;
        ResetTarget();
        Vector3 randomPos = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4, 4f));
        
        transform.position = randomPos + pivotTr.position;
        // ========= 2 - 13 ===========
        randomPos = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4, 4f));
        //playerTr.position = randomPos + pivotTr.position;
    }


    //void RewardFunctionFarToTarget()
    //{
    //    if (Vector3.Distance(transform.position, playerTr.position) > 3)
    //        AddReward(-0.01f / agentParameters.maxStep);
    //    else
    //        AddReward(0.01f / agentParameters.maxStep);
    //}

    void ResetTarget()
    {
        Vector3 randomPos;

        for (int i = 0; i < targets.Length; i++)
        {
            randomPos = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4, 4f));
            targets[i].position = randomPos + pivotTr.position;
        }

        // 2 - 15 =======================
        randomPos = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4, 4f));
        //playerTr.position = randomPos + pivotTr.position;

        Quaternion randomQuat = Random.rotation;
        randomQuat.x = transform.rotation.x;
        randomQuat.z = transform.rotation.z;

        transform.rotation = randomQuat;
    }
}
