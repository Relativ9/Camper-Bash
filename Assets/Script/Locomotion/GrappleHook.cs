using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrappleHook : MonoBehaviour
{
    //TODO change script to be sitting on the actual grapple hook weapon itself instead of always on the character, should only be usable while equipped
    class hookPro
    {
        public float time;
        public Vector3 initPos;
        public Vector3 initVel;
        public TrailRenderer tail;
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
    public Vector3 grappleEnd;

    public bool gamePaused;

    public bool hasFired;


    Ray ray;
    RaycastHit grappleHit;


    List<hookPro> hookList = new List<hookPro>();

    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    public Transform aimTrans;
    public float maxLifeTime = 2f;

    public float hookSpeed = 100f;
    public float hookDrop = 9.81f;
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
            hookObject = Instantiate(hookPrefab, grappleTip.position, fpCamTrans.transform.rotation);
            hookObject.transform.LookAt(fpCamTrans.transform.position);
            lineRend.positionCount = 2;
            hasFired = true;
            FireGrapple();
        }
        else if (Input.GetKeyDown(KeyCode.E) && hasFired)
        {
            StopGrapple();
            hasFired = false;
        }

        if (Input.GetKeyDown(KeyCode.Y) && !gamePaused)
        {
            gamePaused = true;
            Time.timeScale = 0f;
        }
        if(Input.GetKeyDown(KeyCode.P) && gamePaused)
        {
            Time.timeScale = 1f;
            gamePaused = false;
        }

        UpdateAirTime(Time.deltaTime);

        if(lineRend.positionCount == 2)
        {
            lineRend.SetPosition(0, grappleTip.position);
            lineRend.SetPosition(1, hookObject.transform.position);
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
        hook.tail = Instantiate(tracerEffect, pos, Quaternion.identity);
        hook.tail.AddPosition(pos);
        return hook;
    }

    public void UpdateAirTime(float deltaTime)
    {
        hookFlySim(deltaTime);
        DestroyHook();
    }

    public void hookFlySim(float deltaTime)
    {
        hookList.ForEach (hook =>
        {
            Vector3 currentPos = GetPos(hook);
            hook.time += deltaTime;
            Vector3 nextPos = GetPos(hook);
            RaycastStep(currentPos, nextPos, hook);
            if(!isGrappling && hookObject != null)
            {
                hookObject.transform.position = Vector3.Lerp(currentPos, nextPos, hook.time);
            }
        });
    }

    void RaycastStep (Vector3 start, Vector3 end, hookPro hookList)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out grappleHit, distance, grappleLayer) && !isGrappling)
        {
            hitEffect.transform.parent = grappleHit.transform;
            hitEffect.transform.position = grappleHit.point;
            hitEffect.transform.forward = grappleHit.normal;
            hitEffect.Emit(1);

            hookList.tail.transform.position += grappleHit.point;
            hookList.time = maxLifeTime;

            hookTrans.transform.position = hookList.tail.transform.position;

            if (grappleHit.transform.tag != "GrappleSpot") // Sets the hook's parent as the transform it hits when not hitting a grapple spot, ensures it tracks with moving objects.
            {
                hookObject.transform.SetParent(grappleHit.transform);
            }

            grappleJoint = playerTrans.gameObject.AddComponent<SpringJoint>(); // adds a spring joint to the player (what actually makes the grapple function work in the physics)
            grappleJoint.autoConfigureConnectedAnchor = false; // remove preconfigured connected anchor.
            grappleJoint.connectedAnchor = hookObject.transform.position; // sets the new connected anchor to be the grapple point

            // Distance between the grapple point and the player below

            distFromGrapplePoint = Vector3.Distance(playerTrans.position, hookObject.transform.position);

            grappleJoint.maxDistance = distFromGrapplePoint * 0.65f;
            grappleJoint.minDistance = distFromGrapplePoint * 0.40f;

            grappleJoint.spring = 150f;
            grappleJoint.damper = 200f;
            grappleJoint.massScale = 4.5f;

            isGrappling = true;
            hookObject.transform.position = grappleHit.point;
        }
        else
        {
            hookList.tail.transform.position = end;
        }
    }
    void DestroyHook()
    {
        hookList.RemoveAll(hookPro => hookPro.time >= maxLifeTime);
    }

    public void FireGrapple()
    {
        Vector3 velocity = (aimTrans.position - grappleTip.position).normalized * hookSpeed;
        var hook = CreateHook(grappleTip.position, velocity);
        hookList.Add(hook);
    }
    public void StopGrapple()
    {
        if (isGrappling && grappleJoint != null)
        {
            lineRend.positionCount = 0; //Removes the line from the world (by setting it's positions to 0)
            Destroy(grappleJoint);
            Destroy(hookObject);
            isGrappling = false;
            //hookTrans.transform.position = Vector3.zero;
        }

    }

}
