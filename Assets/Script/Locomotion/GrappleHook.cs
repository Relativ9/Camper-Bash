using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [Header("Manually assigned variables")]
    [SerializeField] private Transform grappleTip;
    [SerializeField] private Transform dirParent;
    [SerializeField] private Transform playerTrans;
    [SerializeField] private Transform fpCamTrans;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] public Transform hookTrans;

    [Header("Editable in inspector")]
    [SerializeField] private float maxGrappleDist = 50f;

    [Header("Visible for debugging")]
    [SerializeField] private float distFromGrapplePoint;
    [SerializeField] private float timeToHit;
    [SerializeField] private bool hasSpear;
    [SerializeField] private bool hasThrown;
    [SerializeField] public bool isGrappling;
    [SerializeField] private bool pressedGrapple;

    private LineRenderer lineRend;
    private Vector3 grapplePoint;
    private SpringJoint grappleJoint;
    private Climbing climb;
    private GameObject hookInstance;

    // Start is called before the first frame update
    void Start()
    {
        climb = playerTrans.gameObject.GetComponent<Climbing>();
        lineRend = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !climb.isClimbing && !isGrappling)
        {
            StartCoroutine("GrappleDelay");
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            StopGrapple();
        }
    }


    private void LateUpdate()
    {

        DrawRope();
    }

    public void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpCamTrans.position, fpCamTrans.forward, out hit, maxGrappleDist, grappleLayer) && hookInstance != null)
        {
            grapplePoint = hookInstance.transform.position; //declares the transform to be the same as the instantiated hook.

            grappleJoint = playerTrans.gameObject.AddComponent<SpringJoint>(); // adds a spring joint to the player (what actually makes the grapple function work in the physics)
            grappleJoint.autoConfigureConnectedAnchor = false; // remove preconfigured connected anchor.
            grappleJoint.connectedAnchor = grapplePoint; // sets the new connected anchor to be the grapple point

            // Distance between the grapple point and the player below

            distFromGrapplePoint = Vector3.Distance(playerTrans.position, grapplePoint);

            grappleJoint.maxDistance = distFromGrapplePoint * 0.65f;
            grappleJoint.minDistance = distFromGrapplePoint * 0.40f;

            grappleJoint.spring = 150f;
            grappleJoint.damper = 200f;
            grappleJoint.massScale = 4.5f;

            lineRend.positionCount = 2; // Number of vertices for the line (2 - one for grapple tip (origin point) and one for the grapple point (end point)) // TODO - Consider building a system for mid-line collision of the grapple line (swing around horizontal poles etc, add 1 or more points and set the new joint position to that depending on collision).
            isGrappling = true;
        }
    }

    public void StopGrapple()
    {
        if (isGrappling && grappleJoint != null && hookInstance != null)
        {
            lineRend.positionCount = 0; //Removes the line from the world (by setting it's positions to 0)
            Destroy(grappleJoint);
            isGrappling = false;
            Destroy(hookInstance.gameObject);
        }

    }

    private void DrawRope()
    {

        if (!grappleJoint)        // -- SPRING JOINT-- if we havent grappled, dont draw line.
        {
            return;
        }

        else  //sets the positions of start and end point of the line (won't draw without it). 
        {
            lineRend.SetPosition(0, grappleTip.position);
            lineRend.SetPosition(1, grapplePoint);
        }
    }

    IEnumerator GrappleDelay()
    {
        RaycastHit hookHit;
        if (Physics.Raycast(fpCamTrans.position, fpCamTrans.forward, out hookHit, maxGrappleDist))
        {
            yield return new WaitForSeconds(timeToHit);
            hookInstance = Instantiate(hookPrefab, hookHit.point, fpCamTrans.transform.rotation);
            hookInstance.transform.LookAt(fpCamTrans.transform.position);
            hookTrans.position = hookInstance.transform.position;
            if (hookHit.transform.tag != "GrappleSpot") // Sets the hook's parent as the transform it hits when not hitting a grapple spot, ensures it tracks with moving objects.
            {
                hookInstance.transform.SetParent(hookHit.transform);
            }
        }
/*      yield return new WaitForSeconds(0.2f);*/ // wait some time to simulate "tension" being achieved on the grapple line.
        StartGrapple();
    }
}
