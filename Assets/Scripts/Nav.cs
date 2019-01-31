using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nav : MonoBehaviour
{
    NavMeshAgent nv;
    Transform targetPos;
    // Start is called before the first frame update
    void Start()
    {
        nv = GetComponent<NavMeshAgent>();
        targetPos = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(targetPos.position);
        nv.SetDestination(targetPos.position);
    }
}
