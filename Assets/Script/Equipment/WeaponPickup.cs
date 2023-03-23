using UnityEngine;
using UnityEngine.VFX;

public class WeaponPickup : MonoBehaviour
{
    [Header("Manually assigned variables")]
    [SerializeField] private Transform weapons;
    [SerializeField] private Transform fpCamTrans;
    [SerializeField] private Transform gunTip;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform meleeBase;
    [SerializeField] private Animator anim;

    //Assigned in start
    private Projectiles projectileScript;
    private Rigidbody weaponRb;
    private Collider coll;
    private PlayerMelee playMelee;
    private Transform meleeWeapon;

    [Header("Editable in inspector")]
    [SerializeField] private float throwDistFor = 5f;
    [SerializeField] private float throwDistUp = 2f;
    [SerializeField] private float maxPickUpDist = 2f;

    [Header("Must remain publicly accessible")]
    public bool hasWeapon;

    

    void Start()
    {
        projectileScript = FindAnyObjectByType<Projectiles>();
        weaponRb = this.GetComponent<Rigidbody>();

        coll = this.GetComponent<Collider>();
        playMelee = FindAnyObjectByType<PlayerMelee>();
        meleeWeapon = playMelee.gameObject.transform;

        if (!hasWeapon)
        {
            projectileScript.enabled = false;
            weaponRb.isKinematic = false;
            coll.enabled = true;
        }

        if (hasWeapon)
        {
            projectileScript.enabled = true;
            weaponRb.isKinematic = true;
            coll.enabled = false;
            hasWeapon = true;
        }
    }

    void Update()
    {
        Vector3 distanceToPlayer = player.transform.position - transform.position;
        if (distanceToPlayer.magnitude <= maxPickUpDist && Input.GetKeyDown(KeyCode.E) && !hasWeapon)
        {
            PickUp();
        }
        if (Input.GetKeyDown(KeyCode.Q) && hasWeapon)
        {
            Drop();
        }

        anim.SetBool("hasWeapon", hasWeapon);
    }

    public void PickUp() //Picks up the weapon, but should be changed to be just the "shooting module" with the grapple always equipped.
    {
        coll.enabled = false;
        Debug.Log("Pickup RAN");
        //equipped = true;
        hasWeapon = true;
        projectileScript.currentAmmo = projectileScript.currentAmmo + 5;

        projectileScript.enabled = true;

        transform.SetParent(weapons);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        weaponRb.isKinematic = true;
    }

    public void Drop() //Dropping weapon/grapple, will change in the future to only drop "shooting module" and not the grapple itself, need the modular 3d model first
    {
        coll.enabled = true;
        Debug.Log("Drop RAN");
        hasWeapon = false;

        transform.SetParent(null);

        weaponRb.isKinematic = false;

        weaponRb.velocity = player.GetComponent<Rigidbody>().velocity;
        weaponRb.AddForce(fpCamTrans.forward * throwDistFor, ForceMode.Impulse);
        weaponRb.AddForce(fpCamTrans.up * throwDistUp, ForceMode.Impulse);

        float randomSpin = Random.Range(-1f, 1f);
        weaponRb.AddTorque(new Vector3(randomSpin, randomSpin, randomSpin) * 10);
    }


}
