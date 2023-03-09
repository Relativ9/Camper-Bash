using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //TODO fix crosshair positioning

    [SerializeField] private GameObject bulletHitFX;
    //private EnemyHealth enemyHealth;
    //private EnemyMovementScript enemyMovement;
    private PlayerHealth playerHealth;
    public GameObject decalPrefab;
    public GameObject colParent;



    void Start()
    {
        //enemyHealth = FindObjectOfType<EnemyHealth>();
        colParent = GetComponent<GameObject>();
        //enemyMovement = FindObjectOfType<EnemyMovementScript>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Spawns Bullet decal and effect at the point of collision makes decal child of object it collides with. 
        ContactPoint colcon = collision.contacts[0];
        colParent = collision.collider.gameObject;


        //GameObject impactFX = Instantiate(bulletHitFX, colcon.point, Quaternion.LookRotation(colcon.normal));
        //impactFX.SetActive(true);
        //Destroy(impactFX, 0.5f);


        if (collision.gameObject.tag == "Enemy")
        {
            //enemyHealth.enemyDamage(1f);
            //enemyMovement.ChasePlayer();
        }

        if (collision.gameObject.tag == "Player")
        {
            playerHealth.playerDamage(1f);
        }

        if(collision.gameObject.tag != "Projectile")
        {
            Destroy(gameObject);
            SpawnDecal(colcon);
        }


    }

    void SpawnDecal(ContactPoint hitInfo)
    {
        var decal = Instantiate(decalPrefab);
        decal.transform.position = hitInfo.point;
        decal.transform.forward = hitInfo.normal * -1f;
        decal.transform.parent = colParent.transform;
        Destroy(decal, 5f);

    }

}
