using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    public Vector3 grappleToPoint; // where we grapple on to in our world

    public LayerMask whatIsGrappleable; // what can be grappled and what cant 
    public Transform gunTip; // for drawing rope (draw rope from gun tip to where the grapple hit.
    public Transform playerCamera; // for Raycasting.  where are we raycasting from?
    public Transform player; // For Joint/ Rope - on Raycastm add a joint to the player objects components (through inspector)
    [SerializeField] public Transform directionParent;
    public Rigidbody PlayerRigidbody; // TODO - seems it's never used. Delete?
    public GameObject rightHand;
    public Vector3 grappleDirection;
    public float playerDistanceFromGrappleToPoint;

    public Climbing climbingCheck;

    public float maxDistanceRC = 50f; // maximum Raycast distance

    public SpringJoint joint;

    private GameObject grappleSpearInstance; // Instantiated spear which will be impaled on the wall, the pummel of which the grappling will anchor too.
    private GameObject throwSpearInstance; // Instantiated spear which will fly from the player using RB forces.
    public GameObject returnSpearInstance; // Instantiated spear which will fly from the impale point back to the player.
    public GameObject spearThrow; // reference to the actual prefab
    public GameObject spearImpale; // referemce pt tje actual prefab

    //public Transform spearPummel;

    public float throwForce = 1000f; // Impulse force which proppels the throw spear forward.
    private float timeToHit;
    public bool hasSpear; // Determines if you can throw or not, spear has to be collected once for the first time in the first level, then press E while looking at the impaled spear to retrieve after throwing.
    private bool hasThrown;

    public bool lookingAtSpear;
    public bool isGrappling;
    public bool hitEnemy;
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        hasSpear = true;
    }

    void Update()
    {
        RaycastHit returnSpearhit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out returnSpearhit, maxDistanceRC * 10f) && returnSpearhit.transform.tag == "Spear")
        {
            lookingAtSpear = true;
        }
        else
        {
            lookingAtSpear = false;
        }

        if (Input.GetMouseButton(1) && !climbingCheck.isClimbing && hasSpear) // when holding right-click
        {
            Debug.Log("Grapple!");
            StartCoroutine("grappleDelay");
            hasSpear = false;
            hasThrown = true;
        }

        else if (Input.GetKeyDown(KeyCode.E)) // when release right-click
        {
            StopGrapple(); //
        }
    }

    private void FixedUpdate()
    {
        if (hasThrown)
        {
            throwSpear(); // Throw spear instantiation in fixedUpdate since we're dealing with RB forces.
            hasThrown = false;
        }
    }

    void LateUpdate()
    {
        DrawRope(); // put the linerenderer into lateUpdate to make sure it definitly happens after the spear is impaled and not before.
    }

    public void throwSpear() // Instantiates a rigidbody spear above the players right shoulder and sends it with an Addforce towards the crosshairs destination. Destroys on impact.
    {
        timeToHit = 0f; // resets the time it takes for the spear to reach its destination at the beginning of each throw.
        RaycastHit aimHit;
        if (Physics.Raycast(gunTip.transform.position, playerCamera.forward, out aimHit, maxDistanceRC))
        {
            throwSpearInstance = Instantiate(spearThrow, gunTip.transform.position, playerCamera.transform.rotation);
            throwSpearInstance.transform.LookAt(aimHit.point); // makes sure the spear is always pointing towards its target destination.
            throwSpearInstance.GetComponent<Rigidbody>().AddForce(throwSpearInstance.transform.forward * throwForce * Time.fixedDeltaTime, ForceMode.Impulse);
            timeToHit = throwSpearInstance.GetComponent<Spear>().timeToHit; // adds the time it takes for the spear to hit each time you throw it, varies on lenght of throw, set by the spear prefab.
        }
    }

    private void returnSpear()
    {
        returnSpearInstance = Instantiate(spearThrow, grappleSpearInstance.transform.position, grappleSpearInstance.transform.rotation);
        returnSpearInstance.transform.LookAt(player);
        returnSpearInstance.GetComponent<Rigidbody>().AddForce(returnSpearInstance.transform.forward * throwForce, ForceMode.Impulse);
        timeToHit = returnSpearInstance.GetComponent<Spear>().timeToHit;
        Destroy(returnSpearInstance.gameObject, timeToHit);
    }

    public void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistanceRC, whatIsGrappleable))
        {
            grappleToPoint = grappleSpearInstance.transform.GetComponentInChildren<Transform>().Find("SpearPummel").GetComponent<Transform>().position; //sets the target transform in world space of where the line renderer will draw, and grapple joint will initiate.

            // SPRING JOINT SETTINGS

            joint = rightHand.gameObject.AddComponent<SpringJoint>(); // add joint component to player in unity inspector
            joint.autoConfigureConnectedAnchor = false; // some joint settings visible at front-end
            joint.connectedAnchor = grappleToPoint; // same as above

            // Distance between the grapple point and player below

            playerDistanceFromGrappleToPoint = Vector3.Distance(player.position, grappleToPoint);

            joint.maxDistance = playerDistanceFromGrappleToPoint * 0.65f;
            joint.minDistance = playerDistanceFromGrappleToPoint * 0.40f;

            joint.spring = 150f;
            joint.damper = 200f;
            joint.massScale = 4.5f;

            lr.positionCount = 2; // Number of vertices for the line (2 - one for grapple point one for guntip)
            isGrappling = true;
        }
    }


    public void StopGrapple()
    {
        if (isGrappling || lookingAtSpear || hitEnemy)
        {
            lr.positionCount = 0; //Number of vertices for the line (0 - no more line drawn)
            Destroy(joint);
            isGrappling = false;
            returnSpear();
            Destroy(grappleSpearInstance.gameObject);
            hasSpear = true;
        }

    }

    private void DrawRope()
    {

        if (!joint)        // -- SPRING JOINT-- if we havent grappled, dont draw line.
        {
            return;
        }

        else  // draw a rope from guntip to grapple point
        {
            lr.SetPosition(0, gunTip.position); // starts from lr.positionCount = 2. So 1st point is guntip
            lr.SetPosition(1, grappleToPoint); // starts from lr.positionCount = 2. So 2nd point is grapple point
        }

    }

    public Vector3 GetGrappleToPoint()
    {
        return grappleToPoint;
    }

    IEnumerator grappleDelay()
    {
        RaycastHit spearHit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out spearHit, maxDistanceRC)) // After the throwSpear method has finished and hit a collider, this insantiates a spear (without a rigidbody) to be impaled into the rigidbody.
        {
            yield return new WaitForSeconds(timeToHit);
            grappleSpearInstance = Instantiate(spearImpale, spearHit.point, playerCamera.transform.rotation);
            grappleSpearInstance.transform.LookAt(playerCamera.transform.position);
            if (spearHit.transform.tag != "GrappleSpot") // if the target hit is an enemy, sets the spear as parent so it follows it (staying impaled)
            {
                grappleSpearInstance.transform.SetParent(spearHit.transform);
                hitEnemy = true;
            }
            else
            {
                hitEnemy = false;
            }

        }
        //yield return new WaitForSeconds(0.2f); // wait some time to simulate "tension" being achieved on the grapple line.
        StartGrapple();
    }
}