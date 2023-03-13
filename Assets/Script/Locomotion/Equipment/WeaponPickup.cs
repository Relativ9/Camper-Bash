using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponPickup : MonoBehaviour
{

    public Weapon weaponScript;
    public Rigidbody weaponRB;
    public BoxCollider coll;
    public Transform Weapons, fpsCamera, gunTip;
    //public ParticleSystem tipMuzzle;
    public GameObject Player;
    public VisualEffect muzzle;

    public float throwDistFor, throwDistUp;
    public float maxPickUpDist;

    public bool equipped;
    public bool slotFull;

    // Start is called before the first frame update
    void Start()
    {
        if (!equipped)
        {
            weaponScript.enabled = false;
            weaponRB.isKinematic = false;
            //coll.isTrigger = false;
            coll.enabled = true;
        }

        if (equipped)
        {
            weaponScript.enabled = true;
            weaponRB.isKinematic = true;
            //coll.isTrigger = true;
            coll.enabled = false;
            slotFull = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = Player.transform.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= maxPickUpDist && Input.GetKeyDown(KeyCode.E) && !weaponScript.slotFull)
        {
            PickUp();
        }
        if (equipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    public void PickUp()
    {
        //coll.isTrigger = true;
        coll.enabled = false;
        Debug.Log("Pickup RAN");
        equipped = true;


        weaponScript.slotFull = true;

        //slotFull = true;
        weaponScript.enabled = true;

        transform.SetParent(Weapons);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        weaponRB.isKinematic = true;
        weaponScript.gunTip = gunTip;
        weaponScript.muzzleEffect = muzzle;
    }

    public void Drop()
    {
        //coll.isTrigger = false;
        coll.enabled = true;
        Debug.Log("Drop RAN");
        equipped = false;
        //slotFull = false;
        weaponScript.slotFull = false;

        transform.SetParent(null);

        weaponRB.isKinematic = false;

        weaponRB.velocity = Player.GetComponent<Rigidbody>().velocity;
        weaponRB.AddForce(fpsCamera.forward * throwDistFor, ForceMode.Impulse);
        weaponRB.AddForce(fpsCamera.up * throwDistUp, ForceMode.Impulse);

        float randomSpin = Random.Range(-1f, 1f);
        weaponRB.AddTorque(new Vector3(randomSpin, randomSpin, randomSpin) * 10);
    }
}
