using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public Rigidbody playerRigidbody;
    [SerializeField] public Camera fpsCam;
    [SerializeField] public Transform cameraFollowTrans;
    [SerializeField] public Transform directionParent;
    [SerializeField] public GrapplingGun grapplingGun;
    [SerializeField] public GameObject leftFoot;
    [SerializeField] public GameObject rightFoot;
    [SerializeField] public GameObject frontFoot;
    [SerializeField] public GameObject backFoot;
    [SerializeField] private VolumeTrigger volTrig;
    [SerializeField] private BreathingCheck breathCheck;
    [SerializeField] private WallRun wallRun;
    [SerializeField] private Climbing climbing;
    [SerializeField] public CapsuleCollider playerCol;

    [SerializeField] public bool isMoving;
    [SerializeField] public bool forwardMove;
    [SerializeField] public bool leftMove;
    [SerializeField] public bool backMove;
    [SerializeField] public bool rightMove;
    [SerializeField] public bool playerOnGround; // for jumping
    [SerializeField] public bool isRunning;
    [SerializeField] public bool sliding;
    [SerializeField] public bool crouching;
    [SerializeField] public bool groundSlide;




    [SerializeField] public bool onStairs;

    [SerializeField] public float horizontal;
    [SerializeField] public float vertical;
    public float groundSlopeDetected;

    [Header("Ground Check")]
    public float leftFootSlope;
    [SerializeField] public bool leftGrounded;
    public float rightFootSlope;
    [SerializeField] public bool rightGrounded;
    public float frontFootSlope;
    [SerializeField] public bool frontGrounded;
    public float backFootSlope;
    [SerializeField] public bool backGrounded;

    public bool canJump;
    private float moveSpeed;
    [SerializeField] public float currentStaminaValue;
    [SerializeField] public float maxStamina = 10f;

    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float crouchSpeed = 2f;
    [SerializeField] float swimSpeed = 2f;
    [SerializeField] float wallRunSpeed = 1f;
    [SerializeField] float airSpeed = 50f;

    [SerializeField] float jumpHeight = 3.5f;
    [SerializeField] public float maxStepHeight = 0.6f;
    [SerializeField] public float maxSlopeAngle = 45f;
    [SerializeField] public float stepDifference = 0f;
    [SerializeField] public float raycastLength = 3f; // length of raycast to ground
    [SerializeField] public float vaultPower = 200f;

    [SerializeField] public float normalGravityStrength = -9.8f;
    [SerializeField] public float noGravityStrength = 0f;
    [SerializeField] public float wallRunGravityStrength = -6f;
    [SerializeField] public float grapplingGravityStrength = -30f;
    [SerializeField] public float ethereumGravityStrength = -50f;

    public float gravityStrength;
    private Vector3 systemGravity;
    public Vector3 currentGravity;

    public Vector3 moveDirection;
    public Vector3 moveDirectionFlat;
    public Vector3 moveDirectionSlope;
    public Vector3 moveDirectionSliding;
    public Vector3 moveDirectionSwimming;
    public Vector3 currentVel;
    public Vector3 stopVel;

    public Vector3 stepDirection;

    public Vector3 viewVel;
    public float viewVelfloat;
    public float distanceToGround;
    public float airTime;
    public float groundTime;


    private Vector3 crouchScale;
    private Vector3 normalScale;
    private Vector3 forward;
    private Vector3 right;

    public Vector3 refVel = Vector3.zero;

    [SerializeField] public float secondsSinceWallRun;
    [SerializeField] public bool recentlyWallRan;
    RaycastHit rightHit;
    RaycastHit leftHit;
    RaycastHit groundRay;
    RaycastHit frontHit;
    RaycastHit backHit;
    [SerializeField] float groundAngle;
    [SerializeField] private bool debug;
    public float multiplier = 4.5f;
    public float jumpMultiplier = 4.5f;

    [SerializeField] public Animator animator;

    public GameObject playerHead;
    public LayerMask layerMask;

    void Start()
    {
        playerRigidbody = FindObjectOfType<PlayerMovement>().GetComponent<Rigidbody>();
        playerHead = GameObject.Find("Head");
        wallRun = GetComponent<WallRun>();
        climbing = GetComponentInChildren<Climbing>();
        this.gameObject.GetComponent<Collider>().material.staticFriction = 100f;

        currentStaminaValue = 10f;
        playerOnGround = false;
        recentlyWallRan = false;
    }



    void Update()
    {
        inputMethod();

        CheckGround();
        CalculateForward();
        CalculateRight();
        DrawDebugLines();
        Crouch();
        didWallRun();
        isKinematic();

        animator.SetBool("animGrounded", playerOnGround);
        animator.SetBool("animFalling", !playerOnGround);
        animator.SetBool("animClimbing", climbing.isClimbing);
        //animator.SetBool("animSurfaceSwimming", volTrig.surfaceSwimming);
        //animator.SetBool("animUnderwaterSwimming", volTrig.underwaterSwimming);
        animator.SetBool("animClimbUp", climbing.climbingUp);
        animator.SetBool("canJump", canJump);

        Debug.DrawRay(transform.position, currentVel * 20f, Color.red);
        Debug.DrawRay(transform.position, moveDirection * 20f, Color.yellow);
    }
    private void FixedUpdate()
    {
        viewVel = playerRigidbody.velocity;
        viewVelfloat = playerRigidbody.velocity.magnitude;
        Walk();
        ApplyGravity();
        Jump();
        //stepForce();
    }

    private void LateUpdate()
    {
        checkJump();
    }

    public void inputMethod()
    {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        moveDirectionSliding = directionParent.right * horizontal + directionParent.forward * vertical;
        moveDirectionSlope = Vector3.ClampMagnitude(directionParent.right * horizontal + directionParent.forward * vertical, 1f);
        moveDirectionFlat = Vector3.ClampMagnitude(directionParent.right * horizontal + directionParent.forward * vertical, 1f);
        moveDirectionSwimming = fpsCam.transform.right * horizontal + fpsCam.transform.forward * vertical;

        if (groundAngle >= 105 || groundAngle <= 75 || !playerOnGround && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
        {
            moveDirection = Vector3.Slerp(moveDirection, moveDirectionFlat, 10f);
        }
        else if (playerOnGround || grapplingGun.isGrappling && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
        {
            moveDirection = Vector3.Slerp(moveDirection, moveDirectionSlope, 10f);
        }
        else if (playerOnGround && sliding && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
        {
            moveDirection = Vector3.Slerp(moveDirection, moveDirectionSliding, 10f);
        }

        if (volTrig.surfaceSwimming || volTrig.underwaterSwimming)
        {
            moveDirection = Vector3.Slerp(moveDirection, moveDirectionSwimming, 10f);
        }

        if (Input.GetKey(KeyCode.LeftShift) && currentStaminaValue > 0f && isMoving && !wallRun.isWallRunning && playerOnGround && breathCheck.canBreathe)
        {
            moveSpeed = runSpeed;
            isRunning = true;
            StopCoroutine("CatchBreath");
        }
        else if (isMoving && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming && !wallRun.isWallRunning)
        {
            moveSpeed = walkSpeed;
            isRunning = false;
        }
        else if (isMoving && volTrig.surfaceSwimming || volTrig.underwaterSwimming)
        {
            moveSpeed = swimSpeed;
            isRunning = false;
        }
        else if (wallRun.isWallRunning && !grapplingGun.isGrappling)
        {
            moveSpeed = wallRunSpeed;
            isRunning = false;
        }
        else if (wallRun.isWallRunning && grapplingGun.isGrappling)
        {
            moveSpeed = walkSpeed;
            isRunning = false;
        }
        else if (!isMoving)
        {
            moveSpeed = 0f;
            isRunning = false;
        }

        if (isRunning || !breathCheck.canBreathe)
        {
            currentStaminaValue -= Time.deltaTime;
        }

        if (!isRunning && playerOnGround && breathCheck.canBreathe)
        {
            StartCoroutine("CatchBreath");
        }

        if (Input.GetKey(KeyCode.C))
        {
            crouching = true;

            crouching = true;
            if (isRunning)
            {
                StartCoroutine("slidingTime");
            }
            else
            {
                moveSpeed = crouchSpeed;
            }
        }
        else
        {
            crouching = false;
        }
    }

    private void ApplyGravity()
    {

        //application of gravity depending on what type of movement or 'state' the player is in, also some other forces acting on the player like increased drag.
        playerRigidbody.useGravity = false;


        if (!playerOnGround && !wallRun.isWallRunning && !grapplingGun.isGrappling && !climbing.isClimbing && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
        {
            gravityStrength = normalGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            playerRigidbody.AddForce(currentGravity, ForceMode.Acceleration);
        }


        if (wallRun.isWallRunning)
        {
            gravityStrength = wallRunGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            playerRigidbody.AddForce(currentGravity, ForceMode.Acceleration);
        }


        if (grapplingGun.isGrappling)
        {
            gravityStrength = noGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            playerRigidbody.useGravity = true;
            playerRigidbody.AddForce(Physics.gravity = systemGravity * playerRigidbody.mass * playerRigidbody.mass, ForceMode.Acceleration);
            systemGravity = Physics.gravity = new Vector3(0f, grapplingGravityStrength, 0f);
        }

        if (climbing.isClimbing)
        {
            playerCol.enabled = false;
            gravityStrength = noGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
        }
        else
        {
            playerCol.enabled = true;
        }

        if (playerOnGround)
        {
            gravityStrength = normalGravityStrength;
            //gravityStrength = noGravityStrength;
            //currentGravity = -groundRay.normal;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            Vector3 gravityDirection = Vector3.Slerp(currentGravity, -groundRay.normal, 0.6f);
            playerRigidbody.AddForce(gravityDirection, ForceMode.Acceleration);
        }

        if (volTrig.surfaceSwimming || volTrig.underwaterSwimming)
        {
            gravityStrength = noGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            playerRigidbody.drag = 1f;
        }
        else if (!climbing.isClimbing)
        {
            playerRigidbody.drag = 0.25f;
        }

        if (volTrig.inGas)
        {
            playerRigidbody.drag = 10f;
        }
        else
        {
            playerRigidbody.drag = 0.25f;
        }

    }

    private void isKinematic()
    {
        if (climbing.isClimbing)
        {
            playerRigidbody.isKinematic = true;
        }
        else
        {
            playerRigidbody.isKinematic = false;
        }
    }

    private void Walk() //countains all the grounded movement code
    {

        if (!climbing.isClimbing || grapplingGun.isGrappling || wallRun.isWallRunning || recentlyWallRan || volTrig.surfaceSwimming || volTrig.underwaterSwimming)
        {
            if (!playerOnGround && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
            {
                playerRigidbody.AddForce(moveDirection * moveSpeed * airSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
            }

            float velocityZ = Vector3.Dot(moveDirection.normalized, currentVel);
            float velocityX = Vector3.Dot(moveDirection.normalized, currentVel);

            if (volTrig.surfaceSwimming || volTrig.underwaterSwimming)
            {
                Vector3 swimLine = Vector3.Lerp(playerRigidbody.velocity, moveDirection * moveSpeed, Time.fixedDeltaTime * 10f);
                currentVel = swimLine;
                playerRigidbody.velocity = currentVel;
            }

            if (playerOnGround)
            {
                if (!groundSlide && !onStairs)
                {
                    Vector3 moveLine = Vector3.Lerp(playerRigidbody.velocity, moveDirection * moveSpeed, Time.fixedDeltaTime * 10f);
                    moveLine.y = playerRigidbody.velocity.y;
                    currentVel = new Vector3(moveLine.x, moveLine.y, moveLine.z);
                    playerRigidbody.velocity = currentVel;

                }

                if (!groundSlide && onStairs)
                {
                    Vector3 moveLine = Vector3.Lerp(playerRigidbody.velocity, moveDirection * moveSpeed, Time.fixedDeltaTime * 10f);
                    moveLine.y = playerRigidbody.velocity.y;

                    currentVel = new Vector3(moveLine.x, moveLine.y, moveLine.z);
                    playerRigidbody.velocity = currentVel;
                }
            }
        }

        if (horizontal <= 0.1 && horizontal > -0.1 && vertical <= 0.1 && vertical > -0.1)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        if (!isMoving && playerOnGround)
        {
            stopVel = new Vector3(0, playerRigidbody.velocity.y, 0);
            playerRigidbody.velocity = stopVel;
            this.gameObject.GetComponent<Collider>().material.staticFriction = 100f;
        }
        else
        {
            this.gameObject.GetComponent<Collider>().material.staticFriction = 0.1f;
        }

        var vel = playerRigidbody.velocity;

        var localVel = directionParent.transform.InverseTransformDirection(vel);

        animator.SetFloat("VerticalVel", localVel.z);
        animator.SetFloat("HorizontalVel", localVel.x);
    }

    private void Crouch()
    {
        normalScale = new Vector3(1f, 1f, 1f);
        crouchScale = new Vector3(1f, 1f * 0.5f, 1f);

        if (crouching)
        {
            this.transform.localScale = crouchScale;
        }
        else
        {
            this.transform.localScale = normalScale;
        }
    }


    private void checkJump()
    {

        if (Input.GetKeyDown(KeyCode.Space) && playerOnGround && groundTime >= 0.1 && !volTrig.inGas)
        {
            if (climbing.canVault == false)
            {
                animator.SetBool("jumpPressed", true);
                playerOnGround = false; //needs to be triggered here instantly as well because we have a Ienumerator giving a slight delay when just moving off the ground without jumping (too stop jitter when moving over surfaces with holes in them (plank bridges ect).
                canJump = true;
            }
            if (climbing.canVault == true)
            {
                StartCoroutine("vaultUp");
            }

        }
        else if (!playerOnGround)
        {
            canJump = false;

        }

    }

    public void Jump() //because jump is running in fixed update, every time there is a physics step it will check if canjump is true and apply the force. 
    {
        if (canJump)
        {
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
            playerRigidbody.AddRelativeForce(Vector3.up * jumpHeight * multiplier, ForceMode.Impulse);
            canJump = false;
        }
    }

    private void CheckGround()
    {
        if (Physics.Raycast(leftFoot.transform.position, Vector3.down, out leftHit, maxStepHeight))
        {
            leftGrounded = true;
            leftFootSlope = Vector3.Angle(leftHit.normal, Vector3.up);
        }
        else
        {
            leftGrounded = false;
            leftFootSlope = 0f;
        }
        if (Physics.Raycast(rightFoot.transform.position, Vector3.down, out rightHit, maxStepHeight))
        {
            rightGrounded = true;
            rightFootSlope = Vector3.Angle(rightHit.normal, Vector3.up);
        }
        else
        {
            rightGrounded = false;
            rightFootSlope = 0f;
        }
        if (Physics.Raycast(backFoot.transform.position, Vector3.down, out backHit, maxStepHeight))
        {
            backGrounded = true;
            backFootSlope = Vector3.Angle(backHit.normal, Vector3.up);
            Debug.DrawLine(backFoot.transform.position, backFoot.transform.position + Vector3.down * 10f, Color.yellow);
        }
        else
        {
            backGrounded = false;
            backFootSlope = 0;
        }
        if (Physics.Raycast(frontFoot.transform.position, Vector3.down, out frontHit, maxStepHeight))
        {
            frontGrounded = true;
            frontFootSlope = Vector3.Angle(frontHit.normal, Vector3.up);
            Debug.DrawLine(frontFoot.transform.position, frontFoot.transform.position + Vector3.down * 10f, Color.green);
        }
        else
        {
            frontGrounded = false;
            frontFootSlope = 0;
        }
        RaycastHit groundRay;
        if (Physics.Raycast(this.transform.position, Vector3.down, out groundRay, 2f))
        {
            if (groundRay.transform.tag == "Stairs")
            {
                onStairs = true;
            }
            else
            {
                onStairs = false;
            }
        }
        else
        {
            onStairs = false;
        }

        if (rightGrounded && leftGrounded)
        {
            //groundSlopeDetected = (rightFootSlope + leftFootSlope) / 2f;
            if (frontGrounded || backGrounded)
            {
                groundSlopeDetected = Mathf.Min(rightFootSlope, leftFootSlope);
            }
            else
            {
                groundSlopeDetected = (rightFootSlope + leftFootSlope) / 2f;
            }
        }
        else if (frontGrounded && backGrounded)
        {
            groundSlopeDetected = (frontFootSlope + backFootSlope) / 2f;
            //groundSlopeDetected = Mathf.Min(frontFootSlope, backFootSlope);
        }

        else
        {
            groundSlopeDetected = Vector3.Angle(groundRay.normal, Vector3.up);
        }

        if (groundSlopeDetected > maxSlopeAngle || groundSlopeDetected < -maxSlopeAngle)
        {
            groundSlide = true;
        }
        else
        {
            groundSlide = false;
        }

        if ((rightGrounded && leftGrounded) || (frontGrounded && backGrounded))
        {
            if (!groundSlide && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
            {
                playerOnGround = true;
                airTime = 0f;
                groundTime += Time.deltaTime;
                animator.SetBool("jumpPressed", false);
            }
            else
            {
                playerOnGround = false;
                //StartCoroutine("unGroundedDelay");
                airTime += Time.deltaTime;
                groundTime = 0f;
            }
        }
        else
        {
            playerOnGround = false;
            //StartCoroutine("unGroundedDelay");
            airTime += Time.deltaTime;
            groundTime = 0f;
        }
    }


    public void CalculateForward() //calculates the forward vector from our player so that it's always parallell with the ground normal (unless the slope is too steep (ie: we're not grounded)
    {
        if (!playerOnGround)
        {
            forward = directionParent.forward;
            return;
        }
        if (playerOnGround)
        {
            forward = Vector3.Cross(directionParent.right, groundRay.normal);
        }
    }

    private void CalculateRight() //does the same thing as forward calculation but for the right vector, used for smoothing diagnoal movement up and down slopes.
    {
        if (!playerOnGround)
        {
            right = -directionParent.right;
            return;
        }

        right = Vector3.Cross(directionParent.forward, groundRay.normal);
    }

    private void DrawDebugLines()
    {
        if (!debug) { return; }
        Debug.DrawLine(transform.position, transform.position + forward * 7f, Color.blue);
        Debug.DrawLine(transform.position, transform.position + right * 7f, Color.green);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * 2f, Color.green);
        Debug.DrawLine(rightFoot.transform.position, transform.position - Vector3.up * raycastLength, Color.green);
    }


    private void didWallRun()
    {
        if (!wallRun.isWallRunning)
        {
            secondsSinceWallRun += Time.deltaTime;
        }
        else
        {
            secondsSinceWallRun = 0f;
        }

        if (secondsSinceWallRun < 2f)
        {
            recentlyWallRan = true;
        }
        else
        {
            recentlyWallRan = false;
        }
    }

    IEnumerator CatchBreath()
    {
        // refresh stamina

        yield return new WaitForSeconds(0.5f);

        while (currentStaminaValue < maxStamina)
        {
            currentStaminaValue += 0.01f * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator slidingTime()
    {
        sliding = true;
        yield return new WaitForSeconds(2f);
        if (!groundSlide)
        {
            sliding = false;
            moveSpeed = crouchSpeed;
        }
    }

    IEnumerator vaultUp()
    {

        playerRigidbody.AddRelativeForce(0f, vaultPower * Time.fixedDeltaTime, 0f, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
        playerRigidbody.AddRelativeForce(Vector3.forward * 600f * Time.fixedDeltaTime);
    }
}
