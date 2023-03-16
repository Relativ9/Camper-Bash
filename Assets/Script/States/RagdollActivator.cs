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
    private Collider weaponCol;
    private PlayerMelee playMelee;

    private Collider[] rigCols;
    private Rigidbody[] rigRBs;
    private Vector3 fallingVel;


    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        grapple = FindFirstObjectByType<GrappleHook>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        playMelee = FindFirstObjectByType<PlayerMelee>();
        playerCol = player.GetComponent<Collider>();
        weaponCol = weaponParent.GetComponentInChildren<Collider>();

        ragdollComponents();
        ragdollOff();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerHealth.isAlive)
        {
            ragdollOn();
        } else
        {
            ragdollOff();   
        }
    }

    void ragdollComponents()
    {
        rigCols = playerRig.GetComponentsInChildren<Collider>();
        rigRBs = playerRig.GetComponentsInChildren<Rigidbody>();
    }

    void ragdollOn()
    {
        playerAnim.enabled = false;
        weaponCol.enabled = true;

        weaponCol.gameObject.AddComponent<Rigidbody>(); //Must add Rigidbody via script rather than enable/disable kinematic to ensure 
        weaponCol.gameObject.transform.SetParent(null); //makes the player drop the weapon on death
        weaponCol.gameObject.GetComponent<Rigidbody>().velocity = fallingVel; //matches the weapon velocity to the player velocity
        weaponCol.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate; //will often be high speed collisions, need interpolate and continous dynamic to ensure the weapon doesn't clip through the ground once dropped
        weaponCol.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        grapple.isGrappling = false;

        foreach (Collider col in rigCols)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rbs in rigRBs)
        {
            rbs.isKinematic = false;
            rbs.velocity = fallingVel;
            rbs.interpolation = RigidbodyInterpolation.Interpolate;
            rbs.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        playerCol.enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = true;


    }

    void ragdollOff()
    {
        if(!playMelee.weaponThrown)
        {
            weaponCol.enabled = false;
        }
        foreach (Collider col in rigCols)
        {
            col.enabled = false;
        }

        foreach (Rigidbody rbs in rigRBs)
        {
            rbs.isKinematic = true;
        }

        playerCol.enabled = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        playerAnim.enabled = true;
        fallingVel = player.GetComponent<Rigidbody>().velocity; //saves the last velocity of the player before death (before hitting the ground in a fall). 
    }
}
