using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    [Header("Manually assigned variable")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform gunTip;
    [SerializeField] private GameObject projectilePrefab;


    [Header("Editable in inspector")]
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float bulletspeed = 200f;
    [SerializeField] private float maxShootDistance = 500f;

    //Assigned in start
    private EnemyHealth enemyHealth;

    private Vector3 aimRot;
    private float playerDist;
    private GameObject bulletInstance;
    private bool hasShot;
    private bool hasThrown;

    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!hasShot && enemyHealth.isAlive)
        {
            StartCoroutine("ShootPlayer");
        }

    }

    IEnumerator ThrowGranade() //Rigidbody projectile, must be slow moving, can have things like bouncing, a timed fuse, etc.
    {
        var shootDir = player.position - this.transform.position;
        RaycastHit hit;
        Physics.Raycast(transform.position, shootDir, out hit, maxShootDistance);
        if (hit.distance <= maxShootDistance)
        {

            if (hit.collider.tag == "Player")
            {
                hasThrown = true;
                bulletInstance = Instantiate(projectilePrefab, gunTip.position, Quaternion.Euler(Vector3.zero));
                aimRot = hit.point - bulletInstance.gameObject.transform.position;
                var bulletRot = Quaternion.Euler(aimRot);
                bulletInstance.transform.rotation = bulletRot;
                bulletInstance.GetComponent<Rigidbody>().AddForce(aimRot.normalized * bulletspeed, ForceMode.Impulse);
                yield return new WaitForSeconds(1f);
                hasThrown = false;
            }
        }
    }

    IEnumerator ShootPlayer() //TODO replace with segment raycast projectile system like in the player shooting.
    {
        var shootDir = player.position - this.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, shootDir, out hit, maxShootDistance))
        {
            if(hit.collider.tag == "Player")
            {
                hasShot = true;
                bulletInstance = Instantiate(projectilePrefab, gunTip.position, Quaternion.Euler(Vector3.zero));
                aimRot = hit.point - bulletInstance.gameObject.transform.position;
                var bulletRot = Quaternion.Euler(aimRot);
                bulletInstance.transform.rotation = bulletRot;
                bulletInstance.GetComponent<Rigidbody>().AddForce(aimRot.normalized * bulletspeed, ForceMode.Impulse);
                yield return new WaitForSeconds(1f);
                hasShot = false;
            }
        }

    }
}
