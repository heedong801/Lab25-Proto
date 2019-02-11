using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class AgentManager : MonoBehaviour
{
    // Player Specification
    public static float hp = 100f;
    public static float armor = 100;
    private float maxHp;
    private float maxArmor = 100;


    private void Start()
    {
        maxHp = hp;
   
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            ApplyDamage(50);
    }

    public static void ApplyDamage(float damage)
    {
        if (armor <= 0)
        {
            if (hp <= 0)
            {
                hp = 0;
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
        }

    }

    private void recoverArmorGage(int recoverGage)
    {
        armor += recoverGage;
        if (armor > 100)
            armor = 100;
    }
  
    
}
