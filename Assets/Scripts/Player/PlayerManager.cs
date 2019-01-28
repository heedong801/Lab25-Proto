using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
	public ItemManager itemManager;

	// Player Specification
	public static float hp = 100f;
    public static float armor = 100;
    private float maxHp;
    private float maxArmor = 100;

    // Player Stage
    public static stage currentStage;

    // Teleport Attribute
    private bool teleportFlag = false;
    private Vector3 TeleportPos = Vector3.zero;

    // References
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentStage = stage.STAGE_1;
        maxHp = hp;
        //hpText.text = hp + " / " + maxHp;
        //hpBar.localScale = Vector3.one;

        //armorText.text = armor + " / " + maxArmor;
        //armorBar.localScale = Vector3.one;
        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            ApplyDamage(50);
    }

    private void FixedUpdate()
    {
        if( teleportFlag )
        {
            teleportFlag = false;
            transform.position = TeleportPos;
            TeleportPos = Vector3.zero;
        }
    }
    public static void ApplyDamage(float damage)
    {
        if (armor <= 0)
        {
            if (hp <= 0)
            {
                hp = 0;
                // Die;
                Debug.Log("You Died!");
            }
            else
            {
                hp -= damage;

				ItemManager.targetHealth = hp - damage;
				ItemManager.takeDamage = true;
				//UpdateHP();
			}
        }
        else
        {
            armor -= damage;
            //UpdateArmor();
        }
		// 마크
        //UIManager.takeDamge = true;
    }

    private void recoverArmorGage(int recoverGage)
    {
        armor += recoverGage;
        if (armor > 100)
            armor = 100;
    }

    private void UpdateHP()
    {
        //hpText.text = hp + " / " + maxHp;
        //hpBar.localScale = new Vector3(hp / maxHp, 1, 1);
    }

    private void UpdateArmor()
    {
        //armorText.text = armor + " / " + maxArmor;
        //armorBar.localScale = new Vector3(armor / maxArmor, 1, 1);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Portal" && teleportFlag == false)
        {
            InfecteeGenerator.enemyPool.ClearItem();
            
            if (currentStage == stage.STAGE_1)
            {
                SceneManager.LoadScene("Stage2");
                currentStage = stage.STAGE_2;
                TeleportPos = new Vector3(0, 0, -6);
                InfecteeGenerator.stage_EnemyZone = GameObject.FindGameObjectsWithTag("SpawnZone");
            }
            teleportFlag = true;
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        //Debug.Log("TCollision");
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (collision.transform.tag == "Heal")
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    ApplyDamage(-10);
                    Destroy(collision.gameObject);
                }
            }
            else if (collision.transform.tag == "Armor")
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    recoverArmorGage(20);
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("CCollision");
       
        if (collision.transform.tag == "Item")
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Item")
        {
            //Debug.Log("CCCollision");
            //Destroy(hit.gameObject);
        }
    }
}
