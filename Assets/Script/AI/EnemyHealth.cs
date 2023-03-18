using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    //[Header("Manually assigned variables")]

    //Assigned at start
    //[SerializeField] private Bullet bullet;
    //[SerializeField] private MeleeWeapon meleeWeap;

    [Header("Must remain publicly accessible")]
    public bool isAlive;
    public float currentHealth;

    [Header("Editable in inspector")]
    [SerializeField] private float maxHealth = 20f;
    //[SerializeField] private float damageVal;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) //Kills the player once health is below 0
        {
            Debug.Log("Enemy died!");
            isAlive = false;
            currentHealth = 0;
            //Destroy(this.gameObject);
        }
    }

    public void EnemyDamage(float hit) //function is used by all damage sources
    {
        if (currentHealth > 0)
        {
            currentHealth -= hit;
        }
    }
}
