using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Transform dirParent;
    [SerializeField] private Transform playerTrans;
    [SerializeField] private Transform fpCamTrans;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] public Transform hookTrans;

    [Header("Editable in inspector")]
    [SerializeField] private float maxGrappleDist = 25f;
    [SerializeField] private float minGrappleDist = 2f;
    [SerializeField] private float retractSpeed = 25f;
    [SerializeField] private float maxLifeTime = 2f;
    [SerializeField] private float hookSpeed = 100f;
    [SerializeField] private float hookDrop = 9.81f;
    [SerializeField] private float lineCurveStrength = 1f;
    [SerializeField] private int lineSegmentCount = 20;

    [Header("Visible for debugging")]
    [SerializeField] private float distFromGrapplePoint;
    [SerializeField] private bool hasSpear;
    [SerializeField] private bool hasThrown;
    [SerializeField] public bool isGrappling;
    [SerializeField] private bool pressedGrapple;
    [SerializeField] private bool gamePaused;
    [SerializeField] private bool hasFired;
    [SerializeField] private bool isRetracting;

    [Header("Must remain publicly accessible")]
    private LineRenderer lineRend;
    private SpringJoint grappleJoint;
    private Climbing climb;



    Ray ray;
    RaycastHit grappleHit;


    List<hookPro> hookList = new List<hookPro>();

    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    public Transform aimTrans;

    public GameObject hookObject;


    void Start()
    {
        climb = playerTrans.gameObject.GetComponent<Climbing>();
        lineRend = this.GetComponent<LineRenderer>();
        gamePaused = false;
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
            Debug.Log("GrappleTip position (Update): " + grappleTip.position);
            Debug.Log("HookObject position (Update): " + hookObject.transform.position);
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

        if (lineRend.positionCount >= 2)
        {
            //DrawCurvedLine();
            DrawLine();
            //lineRend.SetPositions(new Vector3[] { grappleTip.position, hookObject.transform.position });
        }
    }

    Vector3 GetPos(hookPro hook)
    {
        Vector3 gravity = Vector3.down * hookDrop;
        return hook.initPos + hook.initVel * hook.time + 0.5f * gravity * hook.time * hook.time;
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
            //return;
        }
        else 
        {
            isGrappling = false;
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
        Vector3 velocity = fpCamTrans.forward * hookSpeed;
        var hook = CreateHook(grappleTip.position, velocity);
        hookList.Add(hook);
    }
    public void StopGrapple()
    {
        lineRend.positionCount = 0; //Removes the line from the world (by setting it's positions to 0)
        Destroy(grappleJoint);
        Destroy(hookObject);
        isGrappling = false;
        hasFired = false;
        isRetracting = false;
        distFromGrapplePoint = 0f;
    }

    private void DrawLine()
    {
        Vector3 midPoint = Vector3.Lerp(grappleTip.transform.position, hookObject.transform.position, 0.5f);
        midPoint = midPoint + Vector3.up * lineCurveStrength;

        lineRend.SetPosition(0, grappleTip.position);
        lineRend.SetPosition(1, hookObject.transform.position);
        //lineRend.SetPosition(2, hookObject.transform.position);
    }

    //private void DrawCurvedLine()
    //{
    //    Vector3 a = grappleTip.position;
    //    Vector3 c = hookObject.transform.position;
    //    Vector3 midPoint = (a + c) / 2;

    //    // Adjust the height of the control point based on the curve strength
    //    Vector3 b = midPoint + Vector3.up * lineCurveStrength;

    //    lineRend.positionCount = lineSegmentCount + 1;

    //    for (int i = 0; i <= lineSegmentCount; i++)
    //    {
    //        float t = (float)i / lineSegmentCount;
    //        Vector3 point = QuadraticBezier(a, b, c, t);
    //        lineRend.SetPosition(i, point);
    //    }

    //    Debug.Log("GrappleTip position: " + grappleTip.position);
    //    Debug.Log("HookObject position: " + hookObject.transform.position);
    //}

    //void DrawCurvedLine()
    //{
    //    Vector3 p0 = grappleTip.position;
    //    Vector3 p1 = hookObject.transform.position;

    //    Vector3 midPoint = (p0 + p1) / 2;
    //    midPoint.y += p0.y < p1.y ? -lineCurveStrength : lineCurveStrength;

    //    lineRend.positionCount = lineSegmentCount + 1;

    //    for (int i = 0; i <= lineSegmentCount; i++)
    //    {
    //        float t = i / (float)lineSegmentCount;
    //        Vector3 position = QuadraticBezier(p0, midPoint, p1, t);
    //        Debug.Log($"Position {i}: {position}");
    //        lineRend.SetPosition(i, position);
    //    }
    //}

    //Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    //{
    //    Vector3 p0 = Vector3.Lerp(a, b, t);
    //    Vector3 p1 = Vector3.Lerp(b, c, t);
    //    Vector3 result = Vector3.Lerp(p0, p1, t);
    //    Debug.Log($"QuadraticBezier: a={a}, b={b}, c={c}, t={t}, result={result}");
    //    return result;
    //}
    //void DrawCurvedLine()
    //{

    //    Vector3 p0 = grappleTip.position;
    //    Vector3 p1 = hookObject.transform.position;

    //    Vector3 midPoint = (p0 + p1) / 2;
    //    midPoint.y += p0.y < p1.y ? -lineCurveStrength : lineCurveStrength;

    //    lineRend.positionCount = lineSegmentCount + 1;

    //    for (int i = 0; i <= lineSegmentCount; i++)
    //    {
    //        float t = i / (float)lineSegmentCount;
    //        lineRend.SetPosition(i, QuadraticBezier(p0, midPoint, p1, t));
    //    }
    //}

    //Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    //{
    //    Vector3 p0 = Vector3.Lerp(a, b, t);
    //    Vector3 p1 = Vector3.Lerp(b, c, t);
    //    return Vector3.Lerp(p0, p1, t);
    //}
}
