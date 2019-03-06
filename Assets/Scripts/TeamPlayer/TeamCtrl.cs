using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeamCtrl : MonoBehaviour
{
    NavMeshAgent navmesh;
    GameObject Player;
    public bool isMyTeam;
    public bool added;

    public float teamTimeCount = 10f;

    public float enemyFindRadius;
    public LayerMask playerMask;
    public LayerMask enemyMask;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        navmesh = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Collider[] enemyInRadius = Physics.OverlapSphere(transform.position, enemyFindRadius, enemyMask);

        for (int i = 0; i < enemyInRadius.Length; i++)
        {
            if (enemyInRadius[i].transform.CompareTag("Infectee"))
            {
                Vector3 enemyPos = new Vector3(enemyInRadius[i].transform.position.x, this.transform.position.y, enemyInRadius[i].transform.position.z);
                this.gameObject.transform.LookAt(enemyPos);
            }
        }
    }

    public IEnumerator FollowPlayer()
    {
        float refreshRate = .25f;
        //animator.SetBool("Walking", true);

        while (Player != null)
        {
  
            navmesh.SetDestination(Player.transform.position);

            //if (navmesh.remainingDistance < navmesh.stoppingDistance)
            //    animator.SetBool("Walking", false);

            //else
            //    animator.SetBool("Walking", true);

            //float distance = Vector3.Distance(this.gameObject.transform.position, Player.transform.position);

            yield return new WaitForSeconds(refreshRate);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // player find radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3);

        // enemy find radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyFindRadius);
    }
}
