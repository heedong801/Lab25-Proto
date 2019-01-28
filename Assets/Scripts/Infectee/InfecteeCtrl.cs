using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfecteeCtrl : MonoBehaviour
{
    //infectee attributes
    public int hp;
    public int maxHp;
    public int damage;
    public int speed;
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
    //InfecteeCtrl myInfecteeCtrl;
    Coroutine moveToTargetRoutine;
    //Collider myCollider;
    //Rigidbody[] myChildRigid;
    //Collider[] myChildCollider;
    ChangeRagDoll myChange;

    //enemy -> player directrion
    Vector3 toTargetDir = Vector3.zero;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        //myCollider = GetComponent<CapsuleCollider>();
        maxHp = hp;
        //myInfecteeCtrl = gameObject.GetComponent<InfecteeCtrl>();
        //myChildRigid = gameObject.GetComponentsInChildren<Rigidbody>();
        //myChildCollider = gameObject.GetComponentsInChildren<Collider>();
        myChange = GetComponentInParent<ChangeRagDoll>();

        //for (int i = 1; i < myChildRigid.Length; i++)
        //    myChildRigid[i].isKinematic = true;
        //for (int i = 2; i < myChildCollider.Length; i++)
        //    myChildCollider[i].enabled = false;
    }

    private void OnEnable()
    {
        target = GameObject.FindWithTag("Player").transform;
        moveToTargetRoutine = StartCoroutine(MoveToTarget());
    }

    // Update is called once per frame
    void Update()
    {
        //anim.SetBool(hashWalk, false);
        transform.position += toTargetDir * speed * Time.fixedDeltaTime;
    }

    private IEnumerator MoveToTarget()
    {
        toTargetDir = (target.position - transform.position).normalized;
        Quaternion toTargetRot = Quaternion.LookRotation(new Vector3(toTargetDir.x , transform.position.y, toTargetDir.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, toTargetRot, Time.deltaTime * rotSpeed);

        //anim.SetBool(hashWalk, true);

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
        PlayerManager.ApplyDamage(damage);
        yield return new WaitForSeconds(2.0f);
        isAttack = false;
     
        //anim.SetBool(hashAttack, true);
    }

    public void ApplyDamage(int damage)
    {
 
        hp -= damage;
        Debug.Log(hp);
        if ( hp <= 0 )
           Die();
    }

    private void Die()
    {
        //for (int i = 1; i < myChildRigid.Length; i++)
        //    myChildRigid[i].isKinematic = false;
        //for (int i = 2; i < myChildCollider.Length; i++)
        //    myChildCollider[i].enabled = true;

        //StopCoroutine(moveToTargetRoutine);
        //myInfecteeCtrl.enabled = false;
        //anim.enabled = false;
        //myCollider.enabled = false;
        myChange.StartCoroutine(myChange.ChangeRagdoll());

        //for (int i = 1; i < myChildRigid.Length; i++)
        //    myChildRigid[i].isKinematic = true;
        //for (int i = 2; i < myChildCollider.Length; i++)
        //    myChildCollider[i].enabled = false;

        //myCollider.enabled = true;
        //anim.enabled = true;
        //myInfecteeCtrl.enabled = true;

        //hp = maxHp;
        //Debug.Log("asdasd");
        //InfecteeGenerator.enemyPool.RemoveItem(transform.parent.gameObject, InfecteeGenerator.enemy, InfecteeGenerator.parent);
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.tag == "Player")
        {
            //float distance = Vector3.Distance(target.position, transform.position);

            //if (distance <= recognitionRange)
            //{
                
            //}

            if (!isAttack)
                StartCoroutine(Attack());
        }
    }
}
