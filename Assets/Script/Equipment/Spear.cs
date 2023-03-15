using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{

    //private EnemyHealth enemyHealth;
    //private EnemyMovementScript enemyMovement;
    public float timeToHit;

    // Start is called before the first frame update
    void Start()
    {
        //enemyHealth = FindObjectOfType<EnemyHealth>();
        //enemyMovement = FindObjectOfType<EnemyMovementScript>();
        timeToHit = Time.deltaTime;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //enemyHealth.enemyDamage(3f);
            //enemyMovement.ChasePlayer();
        }

        Destroy(gameObject);
    }
}
