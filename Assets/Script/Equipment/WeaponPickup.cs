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
    //public bool equipped;
    public bool hasWeapon;

    void Start()
    {
        projectileScript = FindAnyObjectByType<Projectiles>();
        weaponRb = this.GetComponent<Rigidbody>();

        coll = this.GetComponent<Collider>();
        playMelee = FindAnyObjectByType<PlayerMelee>();
        meleeWeapon = playMelee.gameObject.transform;

        if (/*!equipped*/ !hasWeapon)
        {
            projectileScript.enabled = false;
            weaponRb.isKinematic = false;
            coll.enabled = true;


        }

        if (/*equipped*/ hasWeapon)
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
        if (/*!equipped && */distanceToPlayer.magnitude <= maxPickUpDist && Input.GetKeyDown(KeyCode.E) && !hasWeapon)
        {
            PickUp();
        }
        if (/*equipped && */Input.GetKeyDown(KeyCode.Q) && hasWeapon)
        {
            Drop();
        }
    }

    public void PickUp()
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
        //projectileScript.gunTip = gunTip;
    }

    public void Drop()
    {
        coll.enabled = true;
        Debug.Log("Drop RAN");
        //equipped = false;
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
