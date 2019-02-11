using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentWeaponCtrl : MonoBehaviour
{
    // Weapon Specification
    public int bulletsPerMag;
    public int bulletsTotal;
    public static int currentBullets = 30;
    public float range;
    public float fireRate;
    private Vector3 originalPos;
    public float accuracy;
    private float originalAccuracy;
    public int damage;

    // Parameters
    private float fireTimer;
    private bool isReloading = false;
    private bool isRunning;

    // Sounds
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip drawSound;

    // References
    public Transform shootPoint;
    private Animator anim;
    public ParticleSystem muzzleFlash;
    private CharacterController characterController;

    // Prefabs
    public GameObject hitSparkPrefab;
    public GameObject hitHolePrefab;
    public GameObject bloodParticlePrefab;

    // Animations
    //private int hashAttack = Animator.StringToHash("isAttack");

    // ObjectPool
    public static MemoryPool bulletHolePool = new MemoryPool();
    public static MemoryPool flarePool = new MemoryPool();
    public static MemoryPool bloodParticlePool = new MemoryPool();

    private Transform fireTraceParent;

    private void Awake()
    {
        fireTraceParent = GameObject.Find("ObjectManager").transform;
    }
    private void Start()
    {
        bulletHolePool.Create(hitHolePrefab, 6, fireTraceParent);
        flarePool.Create(hitSparkPrefab, 6, fireTraceParent);

        bloodParticlePool.Create(bloodParticlePrefab, 100, fireTraceParent);
        characterController = GetComponentInParent<CharacterController>();

        anim = GameObject.Find("Player").GetComponent<Animator>();
    }

    private void Update()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        isReloading = info.IsName("Reload");


        if (fireTimer < fireRate)
            fireTimer += Time.deltaTime;
        Debug.DrawRay(shootPoint.position, shootPoint.transform.forward * 10, Color.black);

        //RecoilBack();
        //Run();
    }

    private void Run()
    {
        anim.SetBool("isRun", Input.GetKey(KeyCode.LeftShift));
        //isRunning = characterController.velocity.sqrMagnitude > 99 ? true : false;

        anim.SetFloat("Speed", characterController.velocity.sqrMagnitude);
    }

    public void Fire()
    {
        if (fireTimer < fireRate || isReloading || isRunning)
        {
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward + Random.onUnitSphere * accuracy, out hit, range))
        {
            InfecteeCtrl enemyCtrl = hit.transform.GetComponent<InfecteeCtrl>();
            Rigidbody rigidbody = hit.transform.GetComponent<Rigidbody>();

            
            if (hit.transform.gameObject.tag != "Infectee")
                StartCoroutine(FireEffect(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)));
            else
            {
                if (enemyCtrl && enemyCtrl.hp > 0)
                {
                    Debug.Log("aaaaaaaaaa");
                    StartCoroutine(BloodEffect(hit.transform.position + Vector3.up * 1.2f));
                }
                //var bP = (GameObject)Instantiate(bloodParticlePrefab, hit.transform.position + Vector3.up * 1.2f, hit.transform.rotation);
            }


            if (enemyCtrl && enemyCtrl.hp > 0)
                enemyCtrl.ApplyDamage(damage);

        }
        currentBullets--;
        fireTimer = 0.0f;
        anim.CrossFadeInFixedTime("Shoot", 0.01f);
        audioSource.PlayOneShot(shootSound);    //shoot sound
        muzzleFlash.Play();
        //Recoil();
        //StartCoroutine(BulletEffect());
    }
    //void BulletEffect()
    //{ 
    //    Quaternion randomQuaternion = new Quaternion(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(180, 360f), 1);


    //    GameObject casing = Instantiate(bulletCasing, bulletCasingPoint);
    //    casing.transform.localRotation = randomQuaternion;
    //    casing.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-30f, 30f)));
    //    Destroy(casing, 1f);
    //}

    IEnumerator BloodEffect(Vector3 pos)
    {
        GameObject bloodParticleTemp;
        bloodParticleTemp = bloodParticlePool.NewItem();
        if (bloodParticleTemp)
        {
            bloodParticleTemp.transform.position = pos;
            bloodParticleTemp.transform.SetParent(fireTraceParent);
        }
        yield return new WaitForSeconds(1.0f);
        bloodParticlePool.RemoveItem(bloodParticleTemp, bloodParticlePrefab, fireTraceParent);
    }

    IEnumerator FireEffect(Vector3 pos, Quaternion rot)
    {
        GameObject bulletHole, flare;
        bulletHole = bulletHolePool.NewItem();
        flare = flarePool.NewItem();

        if (bulletHole)
        {
            bulletHole.transform.position = pos;
            bulletHole.transform.SetParent(fireTraceParent);
            bulletHole.transform.rotation = rot;

            flare.transform.position = pos;
            flare.transform.SetParent(fireTraceParent);
            flare.transform.rotation = rot;
        }
        yield return new WaitForSeconds(0.5f);
        flarePool.RemoveItem(flare, hitSparkPrefab, fireTraceParent);

        yield return new WaitForSeconds(0.5f);
        bulletHolePool.RemoveItem(bulletHole, hitHolePrefab, fireTraceParent);
    }

    private void DoReload()
    {
        if (!isReloading && currentBullets < bulletsPerMag && bulletsTotal > 0)
        {
            anim.CrossFadeInFixedTime("Reload", 0.01f); // Reloading
            audioSource.PlayOneShot(reloadSound);
        }
    }

    public void Reload()
    {
        int bulletsToReload = bulletsPerMag - currentBullets;
        if (bulletsToReload > bulletsTotal)
        {
            bulletsToReload = bulletsTotal;
        }
        currentBullets += bulletsToReload;
        bulletsTotal -= bulletsToReload;
    }


    void OnApplicationQuit()
    {
        bloodParticlePool.Dispose();
        bulletHolePool.Dispose();
        flarePool.Dispose();

    }
}

    //IEnumerator BulletEffect()
    //{
    //    GameObject bulletCasingObject;
    //    bulletCasingObject = bulletCasingPool.NewItem();
    //    Quaternion randomQuaternion = new Quaternion(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(180, 360f), 1);

    //    if (bulletCasingObject)
    //    {
    //        bulletCasingObject.transform.position = transform.TransformPoint(transform.InverseTransformPoint(bulletCasingPoint.position) + new Vector3(0.4f, 0, 0.3f));
    //        bulletCasingObject.transform.localScale = new Vector3(2, 2, 2);
    //        bulletCasingObject.transform.SetParent(fireTraceParent);
    //        bulletCasingObject.transform.localRotation = randomQuaternion;
    //        //bulletCasingObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(Random.Range(0.01f, 0.05f), Random.Range(0.01f, 0.05f), Random.Range(-0.03f, 0.03f)));
    //        bulletCasingObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-30f, 30f)));
    //    }
    //    yield return new WaitForSeconds(1f);

    //    bulletCasingPool.RemoveItem(bulletCasingObject, bulletCasingPrefab, fireTraceParent);



    //private void Recoil()
    //{
    //    Vector3 recoilVector = new Vector3(Random.Range(-recoilKickback.x, recoilKickback.x), recoilKickback.y, recoilKickback.z);
    //    Vector3 recoilCamVector = new Vector3(-recoilVector.y * 200f, recoilVector.x * 200f, 0);

    //    camRecoil.localRotation = Quaternion.Slerp(camRecoil.localRotation, Quaternion.Euler(camRecoil.localEulerAngles + recoilCamVector), recoilAmount); // cam recoil
    //}

    //// back to original position
    //private void RecoilBack()
    //{
    //    camRecoil.localRotation = Quaternion.Slerp(camRecoil.localRotation, Quaternion.identity, Time.deltaTime * 8f);
    //

