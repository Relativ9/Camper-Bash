using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Manually assigned variable")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform meleeBase;

    [Header("Editabled in inspector")]
    [SerializeField] private float maxPickUpDist = 2f;

    //Assigned at start
    private PlayerMelee playMelee;

    [Header("Visible for debugging")]
    [SerializeField] private float weapVel;
    [SerializeField] private float damageVal;

    // Start is called before the first frame update
    void Start()
    {
        playMelee = FindAnyObjectByType<PlayerMelee>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = player.transform.position - transform.position;
        if (Input.GetKeyDown(KeyCode.E) && playMelee.weaponThrown && distanceToPlayer.magnitude <= maxPickUpDist)
        {
            MeleePickup();
        }

        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            weapVel = gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude;
            damageVal = weapVel / 2;
        }

    }

    public void MeleePickup()
    {
        playMelee.weaponThrown = false;
        playMelee.weaponCol.enabled = false;
        Rigidbody thrownRb = gameObject.GetComponent<Rigidbody>();
        Destroy(thrownRb);
        transform.gameObject.GetComponent<Collider>().enabled = false;
        transform.SetParent(meleeBase);
        transform.localPosition = new Vector3(0f, 0.5f, 0f);
        transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<EnemyHealth>() != null)
        {
            collision.gameObject.GetComponent<EnemyHealth>().EnemyDamage(damageVal);
        }
    }
}
