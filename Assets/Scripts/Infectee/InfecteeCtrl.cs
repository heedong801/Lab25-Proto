using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfecteeCtrl : MonoBehaviour
{
    //infectee attributes
    public int hp;
    public int maxHp;
    public int damage;
    public float speed;
    public int rotSpeed;
    public float attackRange;
    public float recognitionRange;
    public Rigidbody spineRigidBody;

    //functional attributes
    private bool isAttack;
    private Transform target;

    //anim
    private Animator anim;
    private int hashWalk = Animator.StringToHash("isWalk");
    private int hashAttack = Animator.StringToHash("isAttack");

    //ref
    Coroutine moveToTargetRoutine;
    ChangeRagDoll myChange;

    //enemy -> player directrion
    Vector3 toTargetDir = Vector3.zero;
    Quaternion idleDir = Quaternion.identity;

    //start flag
    private bool startFlag = false;
    private void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
        anim = GetComponent<Animator>();
        maxHp = hp;
        myChange = GetComponentInParent<ChangeRagDoll>();

        toTargetDir = (new Vector3(Random.Range(-1.0f,1.0f), 0, Random.Range(-1.0f, 1.0f))).normalized;
        Quaternion toTargetRot = Quaternion.LookRotation(new Vector3(toTargetDir.x, transform.position.y, toTargetDir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, toTargetRot, Time.deltaTime * rotSpeed);

        StartCoroutine(Idle());
    }

    private void OnEnable()
    {
        target = GameObject.FindWithTag("Player").transform;
        if( startFlag )
            moveToTargetRoutine = StartCoroutine(MoveToTarget());
        startFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        //anim.SetBool(hashWalk, false);
        transform.position += toTargetDir * speed * Time.fixedDeltaTime;

    }

    private IEnumerator Idle()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        anim.SetBool(hashWalk, true);

        if (distance <= recognitionRange)
        {
            StartCoroutine(MoveToTarget());
            yield return new WaitForSeconds(0.01f);
            speed = 2;

        }

        speed = 1f;
        float randomTime = Random.Range(2, 5);
        yield return new WaitForSeconds(randomTime);
        anim.SetBool(hashWalk, false);

        toTargetDir = (new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f))).normalized;
        Quaternion toTargetRot = Quaternion.LookRotation(new Vector3(toTargetDir.x, transform.position.y, toTargetDir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, toTargetRot, Time.deltaTime * rotSpeed);

        StartCoroutine(Idle());
    }

    private IEnumerator MoveToTarget()
    {
        toTargetDir = (target.position - transform.position).normalized;
        Quaternion toTargetRot = Quaternion.LookRotation(new Vector3(toTargetDir.x , transform.position.y, toTargetDir.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, toTargetRot, Time.deltaTime * rotSpeed);

        anim.SetBool(hashWalk, true);

        //float distance = Vector3.Distance(target.position, transform.position);

        //if (distance <= recognitionRange)
        //{
        //    if (distance <= attackRange && !isAttack)
        //        StartCoroutine(Attack());
        //}
        yield return new WaitForSeconds(.5f);

        moveToTargetRoutine = StartCoroutine(MoveToTarget());
    }

    private IEnumerator Attack()
    {
        isAttack = true;
        anim.SetBool(hashAttack, true);
        PlayerManager.ApplyDamage(damage);
        yield return new WaitForSeconds(1.0f);

        isAttack = false;
        anim.SetBool(hashAttack, false);
    }

    public void ApplyDamage(int damage)
    {
 
        hp -= damage;

        if ( hp <= 0 )
           Die();
    }

    private void Die()
    {
        myChange.StartCoroutine(myChange.ChangeRagdoll());
    }

    public void OnCollisionStay(Collision collision)
    {
        if( collision.gameObject.tag == "Player")
        {
            if (!isAttack)
                StartCoroutine(Attack());
        }
    }
}
