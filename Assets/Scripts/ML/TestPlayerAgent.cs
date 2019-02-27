using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class TestPlayerAgent : Agent
{
    private Rigidbody playerRb;
    private RayPerception rayPer;
    public Transform pivotTr;
    public Transform target;

    public float moveSpeed = 10f;
    public float turnSpeed = 2f;


    public override void InitializeAgent()
    {
        playerRb = GetComponent<Rigidbody>();
        rayPer = GetComponent<RayPerception>();

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-1/ agentParameters.maxStep);
        MoveAgent(vectorAction);

        RewardFunctionFarToTarget();

    }

    public void ResetTarget()
    {
        Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10, 10f));
        target.position = randomPos + pivotTr.position;
    }

    public override void AgentReset()
    {
        Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10, 10f));
        //transform.position = randomPos + pivotTr.position;

        playerRb.velocity = Vector3.zero;

        ResetTarget();
    }

    public override void CollectObservations()
    {
        //Vector3 distanceToTaget = target.position - transform.position;
        ////Debug.Log(distanceToTaget);
        //AddVectorObs(Mathf.Clamp(distanceToTaget.x / 20, -1.0f, 1.0f));
        //AddVectorObs(Mathf.Clamp(distanceToTaget.z / 20, -1.0f, 1.0f));

        //Vector3 relativePos = transform.position - pivotTr.position;

        //AddVectorObs(Mathf.Clamp(relativePos.x / 20, -1.0f, 1.0f));
        //AddVectorObs(Mathf.Clamp(relativePos.z / 20, -1.0f, 1.0f));

        AddVectorObs(playerRb.velocity);
        //AddVectorObs(Mathf.Clamp(playerRb.velocity.x / 20, -1.0f, 1.0f));
        //AddVectorObs(Mathf.Clamp(playerRb.velocity.z / 20, -1.0f, 1.0f));

        //Vector3 relativeTargetPos = target.position - pivotTr.position;

        //AddVectorObs(relativeTargetPos.x);
        //AddVectorObs(relativeTargetPos.z);

        var rayDistance = 20f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
        var detectableObjects = new[] { "PlayerAgent" };

        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        //AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 1.5f, 0f));
    }

    public void MoveAgent(float[] act)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        int action = Mathf.FloorToInt(act[0]);

        // Goalies and Strikers have slightly different action spaces.
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;

        }
        //switch (shootAxis)
        //{
        //    case 1:
        //        myWeaponCtrl.Fire();
        //        break;
        //}

        playerRb.AddForce(dirToGo * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);


    }

    void RewardFunctionFarToTarget()
    {
        if (Vector3.Distance(transform.position, target.position) > 3)
        {
            AddReward(-0.1f / agentParameters.maxStep);
        }
        else
        {
            Debug.Log("Success");
            AddReward(1f);
            //Done();
            ResetTarget();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            Debug.Log("Die");
            AddReward(-1f);
            Done();
        }
    }
}
