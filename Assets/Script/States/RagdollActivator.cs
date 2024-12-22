using UnityEngine;

public class RagdollActivator : MonoBehaviour
{
    [Header("Manually assigned variables")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerRig;
    [SerializeField] private GameObject weaponParent;

    //Assigned in start
    private GrappleHook grapple;
    private Animator playerAnim; //playerAnim instead of anim because enemies will also have ragdolls eventually
    private PlayerHealth playerHealth;
    private Collider playerCol;
    private Collider meleeCol;
    private PlayerMelee playMelee;

    private Collider[] rigCols;
    private Rigidbody[] rigRbs;
    private Vector3 lastVel;


    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        grapple = FindAnyObjectByType<GrappleHook>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playMelee = FindAnyObjectByType<PlayerMelee>();
        playerCol = player.GetComponent<Collider>();
        meleeCol = FindAnyObjectByType<MeleeWeapon>().GetComponent<Collider>();

        RagdollComponents();
        RagdollOff();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerHealth.isAlive)
        {
            RagdollOn();
        } else
        {
            RagdollOff();   
        }
    }

    void RagdollComponents()
    {
        rigCols = playerRig.GetComponentsInChildren<Collider>();
        rigRbs = playerRig.GetComponentsInChildren<Rigidbody>();
    }

    void RagdollOn()
    {
        playerAnim.enabled = false;
        meleeCol.enabled = true;
        if(meleeCol.gameObject.GetComponent<Rigidbody>() != null) //melee col will already have a rigidbody if it's been thrown so no need to add another one
        {
            meleeCol.gameObject.AddComponent<Rigidbody>(); //Must add Rigidbody via script rather than enable/disable kinematic to ensure 
            meleeCol.gameObject.transform.SetParent(null); //makes the player drop the weapon on death
            meleeCol.gameObject.GetComponent<Rigidbody>().linearVelocity = lastVel; //matches the weapon velocity to the player velocity
            meleeCol.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate; //will often be high speed collisions, need interpolate and continous dynamic to ensure the weapon doesn't clip through the ground once dropped
            meleeCol.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        grapple.isGrappling = false;

        foreach (Collider col in rigCols)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rbs in rigRbs)
        {
            rbs.isKinematic = false;
            rbs.linearVelocity = lastVel;
            rbs.interpolation = RigidbodyInterpolation.Interpolate;
            rbs.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        playerCol.enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = true;


    }

    void RagdollOff()
    {
        if(!playMelee.weaponThrown)
        {
            meleeCol.enabled = false;
        }
        foreach (Collider col in rigCols)
        {
            col.enabled = false;
        }

        foreach (Rigidbody rbs in rigRbs)
        {
            rbs.isKinematic = true;
        }

        playerCol.enabled = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        playerAnim.enabled = true;
        lastVel = player.GetComponent<Rigidbody>().linearVelocity; //saves the last velocity of the player before death (before hitting the ground in a fall). 
    }
}
