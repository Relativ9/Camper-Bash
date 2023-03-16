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
    private Weapon weapon;
    private Rigidbody weaponRb;
    private Collider coll;
    private PlayerMelee playMelee;
    private Transform meleeWeapon;

    [Header("Editable in inspector")]
    [SerializeField] private float throwDistFor = 5f;
    [SerializeField] private float throwDistUp = 2f;
    [SerializeField] private float maxPickUpDist = 2f;

    [Header("Must remain publicly accessible")]
    public bool equipped;
    public bool slotFull;

    void Start()
    {
        weapon = FindFirstObjectByType<Weapon>();
        weaponRb = this.GetComponent<Rigidbody>();

        coll = this.GetComponent<Collider>();
        playMelee = FindFirstObjectByType<PlayerMelee>();
        meleeWeapon = playMelee.gameObject.transform;

        if (!equipped)
        {
            weapon.enabled = false;
            weaponRb.isKinematic = false;
            coll.enabled = true;


        }

        if (equipped)
        {
            weapon.enabled = true;
            weaponRb.isKinematic = true;
            coll.enabled = false;
            slotFull = true;
        }
    }

    void Update()
    {
        Vector3 distanceToPlayer = player.transform.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= maxPickUpDist && Input.GetKeyDown(KeyCode.E) && !weapon.slotFull)
        {
            PickUp();
        }
        if (equipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }

        if(playMelee.weaponThrown && distanceToPlayer.magnitude <= maxPickUpDist && Input.GetKeyDown(KeyCode.U))
        {
            //MeleePickup();
        }
    }

    public void PickUp()
    {
        coll.enabled = false;
        Debug.Log("Pickup RAN");
        equipped = true;


        weapon.slotFull = true;

        weapon.enabled = true;

        transform.SetParent(weapons);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        weaponRb.isKinematic = true;
        weapon.gunTip = gunTip;
    }

    public void Drop()
    {
        coll.enabled = true;
        Debug.Log("Drop RAN");
        equipped = false;
        weapon.slotFull = false;

        transform.SetParent(null);

        weaponRb.isKinematic = false;

        weaponRb.velocity = player.GetComponent<Rigidbody>().velocity;
        weaponRb.AddForce(fpCamTrans.forward * throwDistFor, ForceMode.Impulse);
        weaponRb.AddForce(fpCamTrans.up * throwDistUp, ForceMode.Impulse);

        float randomSpin = Random.Range(-1f, 1f);
        weaponRb.AddTorque(new Vector3(randomSpin, randomSpin, randomSpin) * 10);
    }

    //public void MeleePickup() 
    //{
    //    playMelee.weaponThrown = false;
    //    meleeWeapon.localPosition = Vector3.zero;
    //    meleeWeapon.localRotation = Quaternion.Euler(Vector3.zero);

    //    meleeWeapon.gameObject.GetComponent<Collider>().enabled = false;
    //    meleeWeapon.gameObject.transform.SetParent(meleeBase);
    //    Destroy(meleeBase.gameObject.GetComponent<Rigidbody>());

    //}
}
