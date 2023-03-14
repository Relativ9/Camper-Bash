using UnityEngine;

public class RagdollActivator : MonoBehaviour
{

    public Collider playerCol;
    public GameObject playerRig;
    public GameObject player;
    public GrappleHook grapple;
    public Animator playerAnim;

    public PlayerHealth playerHealth;

    private Collider[] rigCols;
    private Rigidbody[] rigRBs;

    // Start is called before the first frame update
    void Start()
    {
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
        foreach (Collider col in rigCols)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rbs in rigRBs)
        {
            rbs.isKinematic = false;
        }

        playerCol.enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = true;

    }

    void ragdollOff()
    {
        foreach(Collider col in rigCols)
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
    }
}
