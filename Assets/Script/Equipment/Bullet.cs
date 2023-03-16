using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private GameObject bulletHitFX;
    private PlayerHealth playerHealth;
    public GameObject decalPrefab;
    public GameObject colParent;



    void Start()
    {
        colParent = GetComponent<GameObject>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    void Update()
    {
       StartCoroutine(DestroyBullet());
    }

    void OnCollisionEnter(Collision collision)
    {
        // Spawns Bullet decal and effect at the point of collision makes decal child of object it collides with. 
        ContactPoint colcon = collision.contacts[0];
        colParent = collision.collider.gameObject;


        //GameObject impactFX = Instantiate(bulletHitFX, colcon.point, Quaternion.LookRotation(colcon.normal));
        //impactFX.SetActive(true);
        //Destroy(impactFX, 0.5f);


        if (collision.gameObject.GetComponent<EnemyHealth>() != null)
        {
            collision.gameObject.GetComponent<EnemyHealth>().EnemyDamage(3f);
        }

        if (collision.gameObject.tag == "Player")
        {
            playerHealth.PlayerDamage(1f);
        }

        if(collision.gameObject.tag != "Projectile")
        {
            Destroy(gameObject);
            SpawnDecal(colcon);
        }


    }

    void SpawnDecal(ContactPoint hitInfo) //instantiates a bullet hole/or effect on the collision point of the bullet
    {
        var decal = Instantiate(decalPrefab);
        decal.transform.position = hitInfo.point;
        decal.transform.forward = hitInfo.normal * -1f;
        decal.transform.parent = colParent.transform;
        Destroy(decal, 5f);
    }

    IEnumerator DestroyBullet() //if the bullet doesn't hit anything it is eventually destroyed, in place of a garbage collection/ system (for now).
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

}
