using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class GrappleHook : MonoBehaviour
{
    //TODO change script to be sitting on the actual grapple hook weapon itself instead of always on the character, should only be usable while equipped
    class hookPro
    {
        public float time;
        public Vector3 initPos;
        public Vector3 initVel;
    }

    [Header("Manually assigned variables")]
    [SerializeField] private Transform grappleTip;
    [SerializeField] private Transform playerTrans;
    [SerializeField] private Transform fpCamTrans;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] public Transform hookTrans;
    [SerializeField] private VisualEffect vfxGraph;

    [Header("Editable in inspector")]
    [SerializeField] private float maxGrappleDist = 25f;
    [SerializeField] private float minGrappleDist = 2f;
    [SerializeField] private float retractSpeed = 25f;
    [SerializeField] private float maxLifeTime = 2f;
    [SerializeField] private float hookSpeed = 50f;
    [SerializeField] private float hookDrop = 9.81f;
    [SerializeField] private float straightenTime = 2f;
    [SerializeField] private float curveStrength = 0.1f;
    [SerializeField] private float defaultNoicePower = 0.2f;

    [Header("Visible for debugging")]
    [SerializeField] private float distFromGrapplePoint;
    [SerializeField] private bool hasSpear;
    [SerializeField] private bool hasThrown;
    [SerializeField] public bool isGrappling;
    [SerializeField] private bool pressedGrapple;
    [SerializeField] private bool hasFired;
    [SerializeField] private bool isRetracting;
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private bool isStraightening = false;
    [SerializeField] public GameObject hookObject;


    private LineRenderer lineRend;
    private SpringJoint grappleJoint;
    private Climbing climb;

    Ray ray;
    RaycastHit grappleHit;
    List<hookPro> hookList = new List<hookPro>();

    void Start()
    {
        climb = playerTrans.gameObject.GetComponent<Climbing>();
        lineRend = this.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !climb.isClimbing && !isGrappling && !hasFired)
        {
            hasFired = true;
            hookObject = Instantiate(hookPrefab, grappleTip.position, fpCamTrans.transform.rotation);
            hookObject.transform.LookAt(fpCamTrans.transform.position);
            lineRend.positionCount = 2;
            FireGrapple();
        }
        else if (Input.GetKeyDown(KeyCode.E) && hasFired)
        {
            if (isGrappling && grappleJoint != null)
            {
                StopGrapple();
                hasFired = false;
            }
        }

        UpdateAirTime(Time.deltaTime);


        if (!isGrappling && hasFired && hookObject != null && distFromGrapplePoint >= maxGrappleDist)
        {
            isRetracting = true;
        }

        if(isRetracting)
        {

            hookObject.transform.position = Vector3.Lerp(hookObject.transform.position, grappleTip.position, Time.deltaTime * retractSpeed);
            if(distFromGrapplePoint <= minGrappleDist)
            {
                StopGrapple();
                Debug.Log("Grapple stopped attempt");
            }
        }
        if (hookObject != null)
        {
            distFromGrapplePoint = Vector3.Distance(playerTrans.position, hookObject.transform.position);
        }

        vfxTargets();
    }

    public void lineTension()
    {
        isStraightening = true;
        elapsedTime = 0f;
        vfxGraph.SetFloat("Noice Power", 0f);
    }

    Vector3 GetPos(hookPro hook)
    {
        Vector3 gravity = Vector3.down * hookDrop;
        return hook.initPos + hook.initVel * hook.time + 0.5f * gravity * hook.time * hook.time;
    }

    public void vfxTargets()
    {
        if (hookObject == null) return;

        Vector3 origin = grappleTip.transform.position;
        Vector3 target = hookObject.transform.position;

        Vector3 direction = (target - origin).normalized;
        Vector3 upDirection = Vector3.Cross(direction, Vector3.Cross(Vector3.up, direction)).normalized;

        Vector3 curve1 = Vector3.Lerp(origin + Vector3.right * 1f, target, 0.25f) + upDirection * curveStrength;
        Vector3 curve2 = Vector3.Lerp(origin, target + Vector3.right * -1f, 0.5f) + upDirection * curveStrength;

        if (isStraightening || isRetracting)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / straightenTime);
            curve1 = Vector3.Lerp(curve1, Vector3.Lerp(origin + Vector3.right * 1f, target, 0.25f), t);
            curve2 = Vector3.Lerp(curve2, Vector3.Lerp(origin, target + Vector3.right * -1f, 0.5f), t);

            if (t == 1f)
            {
                isStraightening = false;
            }
        }
        else if (elapsedTime >= straightenTime)
        {
            curve1 = Vector3.Lerp(origin, target, 0.25f);
            curve2 = Vector3.Lerp(origin, target, 0.5f);
        }

        vfxGraph.SetVector3("Origin", origin);
        vfxGraph.SetVector3("Curve1", curve1);
        vfxGraph.SetVector3("Curve2", curve2);
        vfxGraph.SetVector3("Target", target);
    }


    hookPro CreateHook(Vector3 pos, Vector3 vel)
    {
        hookPro hook = new hookPro();
        hook.initPos = pos;
        hook.initVel = vel;
        hook.time = 0f;
        return hook;
    }

    public void UpdateAirTime(float deltaTime)
    {
            hookFlySim(deltaTime);
            DestroyHook();
    }

    public void hookFlySim(float deltaTime)
    {
        Vector3 newPosition = Vector3.zero;

        hookList.ForEach (hook =>
        {
            Vector3 currentPos = GetPos(hook);
            hook.time += deltaTime;
            Vector3 nextPos = GetPos(hook);
            RaycastStep(currentPos, nextPos, hook);
            if(!isGrappling && hookObject != null && !isRetracting)
            {
                newPosition = Vector3.Lerp(currentPos, nextPos, hook.time);
            }
        });

        if(newPosition != Vector3.zero)
        {
            hookObject.transform.position = newPosition;
        }
    }

    void RaycastStep (Vector3 start, Vector3 end, hookPro hookList)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        Vector3 newPosition = grappleHit.point;

        if (Physics.Raycast(ray, out grappleHit, distance) && !isGrappling)
        {
            hookList.time = maxLifeTime;
            distFromGrapplePoint = Vector3.Distance(playerTrans.position, hookObject.transform.position);

            newPosition = grappleHit.point;

            if (grappleHit.transform.gameObject.layer != LayerMask.NameToLayer("Grap Objects")) //Ensures that if we hit a collider but it isn't in the grap-objects layer the hook retracts as if we didn't hit anything.
            {
                isRetracting = true;
                Debug.Log("Has hit the correct object!");
                return;
            }

            grappleJoint = playerTrans.gameObject.AddComponent<SpringJoint>(); // adds a spring joint to the player (what actually makes the grapple function work in the physics)
            grappleJoint.autoConfigureConnectedAnchor = false; // remove preconfigured connected anchor.
            grappleJoint.connectedAnchor = hookObject.transform.position; // sets the new connected anchor to be the grapple point

            // Distance between the grapple point and the player below

            grappleJoint.maxDistance = distFromGrapplePoint * 0.65f;
            grappleJoint.minDistance = distFromGrapplePoint * 0.40f;

            grappleJoint.spring = 150f;
            grappleJoint.damper = 200f;
            grappleJoint.massScale = 4.5f;

            hookTrans.transform.position = grappleHit.point; 
            isGrappling = true;
            lineTension();
        }
        else 
        {
            isGrappling = false;
            elapsedTime = 0f;
        }
        if (newPosition != Vector3.zero)
        {
            hookObject.transform.position = newPosition;
        }
    }
    void DestroyHook()
    {
        hookList.RemoveAll(hookPro => hookPro.time >= maxLifeTime);
    }

    public void FireGrapple()
    {
        vfxGraph.SetFloat("Noice Power", 0.2f);
        vfxGraph.SetFloat("KillParticles", 10000f);
        Vector3 velocity = fpCamTrans.forward * hookSpeed;
        var hook = CreateHook(grappleTip.position, velocity);
        hookList.Add(hook);
        vfxGraph.Play();
    }
    public void StopGrapple()
    {
        vfxGraph.SetFloat("KillParticles", 0f);
        lineRend.positionCount = 0; //Removes the line from the world (by setting it's positions to 0)
        Destroy(grappleJoint);
        Destroy(hookObject);
        isGrappling = false;
        hasFired = false;
        isRetracting = false;
        distFromGrapplePoint = 0f;
        vfxGraph.Stop();
    }
}
