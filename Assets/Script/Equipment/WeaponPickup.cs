using UnityEngine;
using UnityEngine.VFX;

public class WeaponPickup : MonoBehaviour
{
    [Header("Manually assigned variables")]
    [SerializeField] private Transform Weapons, fpsCamera, gunTip;
    [SerializeField] private GameObject Player;
    [SerializeField] private VisualEffect muzzle;

    //Assigned in start
    public Weapon weapon;
    public Rigidbody weaponRb;
    public BoxCollider coll;

    [Header("Editable in inspector")]
    public float throwDistFor, throwDistUp;
    public float maxPickUpDist;

    [Header("Must remain publicly accessible")]
    public bool equipped;
    public bool slotFull;

    void Start()
    {
        weapon = FindObjectOfType<Weapon>();
        weaponRb = this.GetComponent<Rigidbody>();
        coll = this.GetComponent<BoxCollider>();

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
        Vector3 distanceToPlayer = Player.transform.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= maxPickUpDist && Input.GetKeyDown(KeyCode.E) && !weapon.slotFull)
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
        coll.enabled = false;
        Debug.Log("Pickup RAN");
        equipped = true;


        weapon.slotFull = true;

        weapon.enabled = true;

        transform.SetParent(Weapons);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        weaponRb.isKinematic = true;
        weapon.gunTip = gunTip;
        weapon.muzzleEffect = muzzle;
    }

    public void Drop()
    {
        coll.enabled = true;
        Debug.Log("Drop RAN");
        equipped = false;
        weapon.slotFull = false;

        transform.SetParent(null);

        weaponRb.isKinematic = false;

        weaponRb.velocity = Player.GetComponent<Rigidbody>().velocity;
        weaponRb.AddForce(fpsCamera.forward * throwDistFor, ForceMode.Impulse);
        weaponRb.AddForce(fpsCamera.up * throwDistUp, ForceMode.Impulse);

        float randomSpin = Random.Range(-1f, 1f);
        weaponRb.AddTorque(new Vector3(randomSpin, randomSpin, randomSpin) * 10);
    }
}
