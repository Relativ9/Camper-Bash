using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{

    [Header("Manually assigned variables")]
    [SerializeField] private Camera fpsCam;
    [SerializeField] private Transform camFollowTrans;
    [SerializeField] private Transform dirParent;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject leftFoot;
    [SerializeField] private GameObject rightFoot;
    [SerializeField] private GameObject frontFoot;
    [SerializeField] private GameObject backFoot;

    //Assigned in start
    private Rigidbody playerRb;
    private GrappleHook grapHook;
    private VolumeTrigger volTrig;
    private BreathingCheck breathCheck;
    private WallRun wallRun;
    private Climbing climb;
    private CapsuleCollider playerCol;

    [Header("Editable in inspector")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float airSpeed = 50f;
    [SerializeField] private float wallRunSpeed = 1f;
    [SerializeField] private float swimSpeed = 2f;
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float jumpHeight = 3.5f;
    //[SerializeField] private float vaultPower = 200f;
    [SerializeField] private float multiplier = 4.5f;
    //[SerializeField] private float jumpMultiplier = 4.5f;
    [SerializeField] private float maxStepHeight = 0.6f;
    //[SerializeField] private float stepDifference = 0f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float raycastLength = 3f; // length of raycast to ground (check isGrounded sensitivity)
    [SerializeField] private float normalGravityStrength = -9.8f;
    [SerializeField] private float noGravityStrength = 0f;
    [SerializeField] private float wallRunGravityStrength = -6f;
    [SerializeField] private float grapplingGravityStrength = -30f;

    [Header("Visible for debugging")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravityStrength;
    [SerializeField] private Vector3 currentGravity;
    [SerializeField] private float groundAngle;
    [SerializeField] private float groundSlopeDetected;
    [SerializeField] private bool leftGrounded;
    [SerializeField] private bool rightGrounded;
    [SerializeField] private bool frontGrounded;
    [SerializeField] private bool backGrounded;

    [Header("Must remain publicly accessible")]
    public bool isGrounded;
    public bool isMoving;
    public bool isRunning;
    public Vector3 currentVel;
    public Vector3 stopVel;
    public float currentStaminaValue;
    public float airTime;

    private bool canJump;
    private bool sliding;
    private bool crouching;
    private bool groundSlide;
    private Vector3 systemGravity;
    private float groundTime;
    private Vector3 crouchScale;
    private Vector3 normalScale;
    private Vector3 forward;
    private Vector3 right;
    private bool stepOver;
    private float secondsSinceWallRun;
    private bool recentlyWallRan;
    private bool debug;

    [Header("Input stuff")]
    [SerializeField] public float horizontal;
    [SerializeField] public float vertical;

    //Ground angle checks
    private float leftFootSlope;
    private float rightFootSlope;
    private float frontFootSlope;
    private float backFootSlope;
    private RaycastHit rightHit;
    private RaycastHit leftHit;
    private RaycastHit groundRay;
    private RaycastHit frontHit;
    private RaycastHit backHit;

    //Various context sensitive velocity directions 
    private Vector3 moveDirection;
    private Vector3 moveDirectionFlat;
    private Vector3 moveDirectionSlope;
    private Vector3 moveDirectionSliding;
    private Vector3 moveDirectionSwimming;

    void Start()
    {
        playerRb = this.GetComponent<Rigidbody>();
        grapHook = FindAnyObjectByType<GrappleHook>();
        volTrig = FindAnyObjectByType<VolumeTrigger>();
        wallRun = FindAnyObjectByType<WallRun>();
        climb = FindAnyObjectByType<Climbing>();
        playerCol = this.GetComponent<CapsuleCollider>();
        breathCheck = FindAnyObjectByType<BreathingCheck>();
        anim = FindAnyObjectByType<AnimatorStates>().GetComponent<Animator>();

        this.gameObject.GetComponent<Collider>().material.staticFriction = 100f;

        currentStaminaValue = 10f;
        isGrounded = false;
        recentlyWallRan = false;
    }

    void Update()
    {
        InputMethod();
        CheckJump();
        CheckGround();
        CalculateForward();
        CalculateRight();
        DrawDebugLines();
        Crouch();
        DidWallRun();
        IsKinematic();

        anim.SetBool("animGrounded", isGrounded);
        anim.SetBool("animFalling", !isGrounded);
        anim.SetBool("animClimbing", climb.isClimbing);
        anim.SetBool("animClimbUp", climb.climbingUp);
        anim.SetBool("canJump", canJump);
        anim.SetBool("animMoving", isMoving);
        //anim.SetBool("animSurfaceSwimming", volTrig.surfaceSwimming); // will replace swimming animation with something better, disable for now
        //anim.SetBool("animUnderwaterSwimming", volTrig.underwaterSwimming); // will replace swimming animation with something better, disable for now

        Debug.DrawRay(transform.position, currentVel * 20f, Color.red);
        Debug.DrawRay(transform.position, moveDirection * 20f, Color.yellow);
    }
    private void FixedUpdate()
    {
        Walk();
        ApplyGravity();
        Jump();
    }

    public void InputMethod() //Input method including context sensitive speed adjustment for different states //TODO clean this method up a bit, could probably be split into 3 or more speerate methods more narrow in focus.
    {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        moveDirectionSliding = dirParent.right * horizontal + dirParent.forward * vertical;
        moveDirectionSlope = Vector3.ClampMagnitude(dirParent.right * horizontal + dirParent.forward * vertical, 1f);
        moveDirectionFlat = Vector3.ClampMagnitude(dirParent.right * horizontal + dirParent.forward * vertical, 1f);
        moveDirectionSwimming = fpsCam.transform.right * horizontal + fpsCam.transform.forward * vertical;

        if (groundAngle >= 105 || groundAngle <= 75 || !isGrounded && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
        {
            moveDirection = Vector3.Slerp(moveDirection, moveDirectionFlat, 10f);
        }
        else if (isGrounded || grapHook.isGrappling && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
        {
            moveDirection = Vector3.Slerp(moveDirection, moveDirectionSlope, 10f);
        }
        else if (isGrounded && sliding && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
        {
            moveDirection = Vector3.Slerp(moveDirection, moveDirectionSliding, 10f);
        }

        if (volTrig.surfaceSwimming || volTrig.underwaterSwimming)
        {
            moveDirection = Vector3.Slerp(moveDirection, moveDirectionSwimming, 10f);
        }

        if (Input.GetKey(KeyCode.LeftShift) && currentStaminaValue > 0f && isMoving && !wallRun.isWallRunning && isGrounded && breathCheck.canBreathe)
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
        else if (wallRun.isWallRunning && !grapHook.isGrappling)
        {
            moveSpeed = wallRunSpeed;
            isRunning = false;
        }
        else if (wallRun.isWallRunning && grapHook.isGrappling)
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

        if (!isRunning && isGrounded && breathCheck.canBreathe)
        {
            StartCoroutine("CatchBreath");
        }

        if (Input.GetKey(KeyCode.C))
        {
            crouching = true;

            crouching = true;
            if (isRunning)
            {
                StartCoroutine("SlidingTime");
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

    private void ApplyGravity() //Checks the state of the player and applies gravity wuth different modifiers, context senstive.
    {
        playerRb.useGravity = false; //Using my own gravity strenght and gravity force direction. It is not always straight down, instead if is perpendicular to the ground to avoid the player sliding (so long as the slope angle isn't too steep).
        if (!isGrounded && !wallRun.isWallRunning && !grapHook.isGrappling && !climb.isClimbing && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
        {
            gravityStrength = normalGravityStrength;
            // The below code adds directionality to to the downwards force in the opposite direction if only a single side is grounded, helps getting unstuck when only partially grounded (looks and feels like sliding of a ledge).
            if (leftGrounded)
            {
                currentGravity = new Vector3(10f, gravityStrength * multiplier, 0f);
                playerRb.AddForce(currentGravity, ForceMode.Acceleration);
            } else if (rightGrounded)
            {
                currentGravity = new Vector3(-10f, gravityStrength * multiplier, 0f);
                playerRb.AddForce(currentGravity, ForceMode.Acceleration);
            }
            else if (frontGrounded)
            {
                currentGravity = new Vector3(0f, gravityStrength * multiplier, -10f);
                playerRb.AddForce(currentGravity, ForceMode.Acceleration);
            }
            else if (backGrounded)
            {
                currentGravity = new Vector3(0f, gravityStrength * multiplier, 10f);
                playerRb.AddForce(currentGravity, ForceMode.Acceleration);
            } else
            {
                currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
                playerRb.AddForce(currentGravity, ForceMode.Acceleration);
            }
        }


        if (wallRun.isWallRunning)
        {
            gravityStrength = wallRunGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            playerRb.AddForce(currentGravity, ForceMode.Acceleration);
        }


        if (grapHook.isGrappling)
        {
            gravityStrength = noGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            playerRb.useGravity = true;
            playerRb.AddForce(Physics.gravity = systemGravity * playerRb.mass * playerRb.mass, ForceMode.Acceleration);
            systemGravity = Physics.gravity = new Vector3(0f, grapplingGravityStrength, 0f);
        }

        if (climb.isClimbing)
        {
            playerCol.enabled = false;
            gravityStrength = noGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
        }
        else
        {
            playerCol.enabled = true;
        }

        if (isGrounded)
        {
            gravityStrength = normalGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            Vector3 gravityDirection = Vector3.Slerp(currentGravity, -groundRay.normal, 0.6f);
            playerRb.AddForce(gravityDirection, ForceMode.Acceleration);
        }

        if (volTrig.surfaceSwimming || volTrig.underwaterSwimming)
        {
            gravityStrength = noGravityStrength;
            currentGravity = new Vector3(0f, gravityStrength * multiplier, 0f);
            playerRb.drag = 1f;
        }
        else if (!climb.isClimbing)
        {
            playerRb.drag = 0.25f;
        }

        if (volTrig.inGas)
        {
            playerRb.drag = 10f;
        }
        else
        {
            playerRb.drag = 0.25f;
        }

    }

    private void IsKinematic() // turns on kinematic mode for the playerRb when the player is climbing, will also be used for other situations in the future
    {
        if (climb.isClimbing)
        {
            playerRb.isKinematic = true;
        }
        else
        {
            playerRb.isKinematic = false;
        }
    }

    private void Walk() //countains all the grounded movement code uses physical forces instead of cordinate movement, this is essential for the physics based enviromental puzzles I want to design.
    {

        if (!climb.isClimbing || grapHook.isGrappling || wallRun.isWallRunning || recentlyWallRan || volTrig.surfaceSwimming || volTrig.underwaterSwimming)
        {
            if (!isGrounded && !volTrig.surfaceSwimming && !volTrig.underwaterSwimming)
            {
                playerRb.AddForce(moveDirection * moveSpeed * airSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
            }

            if (volTrig.surfaceSwimming || volTrig.underwaterSwimming)
            {
                Vector3 swimLine = Vector3.Lerp(playerRb.velocity, moveDirection * moveSpeed, Time.fixedDeltaTime * 10f);
                currentVel = swimLine;
                playerRb.velocity = currentVel;
            }

            if (isGrounded)
            {
                if (!groundSlide && !stepOver)
                {
                    Vector3 moveLine = Vector3.Lerp(playerRb.velocity, moveDirection * moveSpeed, Time.fixedDeltaTime * 10f);
                    moveLine.y = playerRb.velocity.y;
                    currentVel = new Vector3(moveLine.x, moveLine.y, moveLine.z);
                    playerRb.velocity = currentVel;

                }

                if (!groundSlide && stepOver)
                {
                    Vector3 moveLine = Vector3.Lerp(playerRb.velocity, moveDirection * moveSpeed, Time.fixedDeltaTime * 10f);
                    moveLine.y = playerRb.velocity.y;

                    currentVel = new Vector3(moveLine.x, moveLine.y, moveLine.z);
                    playerRb.velocity = currentVel;
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

        if (!isMoving && isGrounded)
        {
            stopVel = new Vector3(0, playerRb.velocity.y, 0);
            playerRb.velocity = stopVel;
            this.gameObject.GetComponent<Collider>().material.staticFriction = 100f;
        }
        else
        {
            this.gameObject.GetComponent<Collider>().material.staticFriction = 0.1f;
        }

        var vel = playerRb.velocity;

        var localVel = dirParent.transform.InverseTransformDirection(vel);

        anim.SetFloat("VerticalVel", localVel.z);
        anim.SetFloat("HorizontalVel", localVel.x);
    }

    private void Crouch() //crouch, right now it sets the transform scale of the whole player, instead in the future this will use crouch animations and set height of collider to top of head bone.
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


    private void CheckJump() //sets canjump to true if the player has been on the ground long enough, isn't on a "sticky" enviroment, and of course presses jump.
    {

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && groundTime >= 0.1 && !volTrig.inGas)
        {
            if (climb.canVault == false)
            {
                isGrounded = false; //needs to be triggered here instantly as well because we have a Ienumerator giving a slight delay when just moving off the ground without jumping (too stop jitter when moving over surfaces with holes in them (plank bridges ect).
                canJump = true;
                anim.SetBool("jumpPressed", true);
            }
            if (climb.canVault == true)
            {
                StartCoroutine("VaultUp");
            }

        }
        else if (!isGrounded)
        {
            canJump = false;

        }

    }

    public void Jump() //because CheckJump is running in update, every time there is a physics step it will check if canjump is true and apply the force, must run in fixed update
    {
        if (canJump)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
            playerRb.AddRelativeForce(Vector3.up * jumpHeight * multiplier, ForceMode.Impulse);
            canJump = false;
        }
    }

    private void CheckGround() //checks for grounded status from 4 different transforms around the player, ensure that the player can not get stuck on extremely uneven geometry, can be replaced by spherecasts but this is more economical.
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
            if (groundRay.transform.tag == "StepOver")
            {
                stepOver = true;
            }
            else
            {
                stepOver = false;
            }
        }
        else
        {
            stepOver = false;
        }

        if (rightGrounded && leftGrounded)
        {
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
                isGrounded = true;
                airTime = 0f;
                groundTime += Time.deltaTime;
                anim.SetBool("jumpPressed", false);
            }
            else
            {
                isGrounded = false;
                airTime += Time.deltaTime;
                groundTime = 0f;
                currentVel = playerRb.velocity;
            }
        }
        else
        {
            isGrounded = false;
            airTime += Time.deltaTime;
            groundTime = 0f;
            currentVel = playerRb.velocity;
        }
    }


    public void CalculateForward() //calculates the forward vector from our player so that it's always parallell with the ground normal (unless the slope is too steep (ie: we're not grounded)
    {
        if (!isGrounded)
        {
            forward = dirParent.forward;
            return;
        }
        if (isGrounded)
        {
            forward = Vector3.Cross(dirParent.right, groundRay.normal);
        }
    }

    private void CalculateRight() //does the same thing as forward calculation but for the right vector, used for smoothing diagnoal movement up and down slopes.
    {
        if (!isGrounded)
        {
            right = -dirParent.right;
            return;
        }

        right = Vector3.Cross(dirParent.forward, groundRay.normal);
    }

    private void DrawDebugLines()
    {
        if (!debug) { return; }
        Debug.DrawLine(transform.position, transform.position + forward * 7f, Color.blue);
        Debug.DrawLine(transform.position, transform.position + right * 7f, Color.green);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * 2f, Color.green);
        Debug.DrawLine(rightFoot.transform.position, transform.position - Vector3.up * raycastLength, Color.green);
        Debug.DrawRay(transform.position, currentGravity, Color.blue, 10f);
    }


    private void DidWallRun() //ensures the player can't reenter wallrun mode mid-air if falling along an uneven wall
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

    IEnumerator CatchBreath() //returns stamina to the player
    {

        yield return new WaitForSeconds(0.5f);

        while (currentStaminaValue < maxStamina && !volTrig.inGas)
        {
            currentStaminaValue += 0.01f * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SlidingTime() // lets the player slide on the ground for 2f, sliding lowers friction between the player and the ground
    {
        sliding = true;
        yield return new WaitForSeconds(2f);
        if (!groundSlide)
        {
            sliding = false;
            moveSpeed = crouchSpeed;
        }
    }

    //IEnumerator VaultUp() // Code for vaulting, might use again, but looking to use root motion animations instead as I don't like how it feels right now.
    //{

    //    playerRb.AddRelativeForce(0f, vaultPower * Time.fixedDeltaTime, 0f, ForceMode.Impulse);
    //    yield return new WaitForSeconds(0.1f);
    //    playerRb.AddRelativeForce(Vector3.forward * 600f * Time.fixedDeltaTime);
    //}
}
