using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    //[Header("Manually assigned variables")]

    //Assigned at start
    //[SerializeField] private Bullet bullet;
    //[SerializeField] private MeleeWeapon meleeWeap;

    [Header("Editable in inspector")]
    [SerializeField] private float maxHealth = 20f;
    //[SerializeField] private float damageVal;

    [Header("Visible for debugging")]
    [SerializeField] private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) //Kills the player once health is below 0
        {
            Debug.Log("Enemy died!");
            currentHealth = 0;
            Destroy(this.gameObject);
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
