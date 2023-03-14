using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Manually assigned in editor")]
    public Transform fpCamTrans;
    public Transform dirParent;

    [Header("Editable in inspector")]
    [SerializeField] private float upForce = 14f;
    [SerializeField] private float sideForce = 20f;
    [SerializeField] private float wallJumpForce = 120f;
    [SerializeField] private int maxWallJumps = 2;
    [SerializeField] private int currentWallJumpNo = 0;
    [SerializeField] private float multiplier = 2f;
    float timeCount = 0.0f;

    [Header("Visible for debugging")]
    [SerializeField] private Quaternion currentCamAng;
    [SerializeField] public bool isLeft;
    [SerializeField] public bool isRight;
    [SerializeField] public bool isFront;

    public bool isWallRunning = false;
    private bool canWallJump;
    private bool jumpBoostRight;
    private bool jumpBoostLeft;
    private bool jumpBoostBack;
    private bool rightSideForceAct;
    private bool leftSideForceAct;
    private bool frontForceAct;

    private Quaternion rightWallRunCamAng = Quaternion.Euler(0f, 0f, 12f);
    private Quaternion leftWallRunCamAng = Quaternion.Euler(0f, 0f, -12f);
    private Quaternion frontWallRunCamAng = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion defaultCamAng = Quaternion.Euler(0f, 0f, 0f);
    private float distLeftWall;
    private float distRightWall;
    private float distFrontWall;


    private Rigidbody playerRigidbody;
    private PlayerMovement playerMovement;
    private Climbing climbing;
    //private bool hasPressedWallJump; //might use for animation state machine integration

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        currentCamAng = defaultCamAng;
        playerMovement = GetComponent<PlayerMovement>();
        climbing = GetComponent<Climbing>();
    }

    void Update()
    {
        WallChecker();
        if (isWallRunning == false)
        {
            timeCount = 0f;
            TiltCameraStop();
        }

        if (isWallRunning == true && (isRight == true || isLeft == true || isFront == true))
        {
            WallJumpCheck();
        }

    }


    private void FixedUpdate()
    {
        WallJump();
        ActivateSideForces();
    }

    private void ActivateSideForces() // Helps the player stick the wall during wallrunning, should be set to be just powerful enough that the player won't accidentally disengage the wallrun state, but not so powerful that they can't leave the mode if they want to.
    {
        if (rightSideForceAct && !playerMovement.isGrounded)
        {
            playerRigidbody.AddRelativeForce(dirParent.right * sideForce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            leftSideForceAct = false;
            frontForceAct = false;
        }

        if (leftSideForceAct && !playerMovement.isGrounded)
        {
            playerRigidbody.AddRelativeForce(-dirParent.right * sideForce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            rightSideForceAct = false;
            frontForceAct = false;
        }

        if (frontForceAct && !playerMovement.isGrounded)
        {
            playerRigidbody.AddRelativeForce(dirParent.forward * sideForce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            rightSideForceAct = false;
            leftSideForceAct = false;
        }


    }

    private void WallJump() //jump off the wall during walljump, if statements check which direction the wall is relative to the player.
    {
        if (canWallJump)
        {
            playerRigidbody.AddRelativeForce(dirParent.up * (upForce * multiplier / 2f), ForceMode.Impulse);
            currentWallJumpNo++;
            canWallJump = false;

            if (jumpBoostLeft)
            {
                playerRigidbody.AddRelativeForce(-dirParent.right * wallJumpForce * multiplier * Time.fixedDeltaTime, ForceMode.Impulse);
                jumpBoostLeft = false;
            }

            if (jumpBoostRight)
            {
                playerRigidbody.AddRelativeForce(dirParent.right * wallJumpForce * multiplier * Time.fixedDeltaTime, ForceMode.Impulse);
                jumpBoostRight = false;
            }

            if (jumpBoostBack)
            {
                playerRigidbody.AddRelativeForce(-dirParent.forward * wallJumpForce * multiplier * Time.fixedDeltaTime, ForceMode.Impulse);
                jumpBoostBack = false;
            }
        }
    }

    private void WallChecker() //checks the different possible wall directions for wallrunning, makes sure the player can't wallrun on two seperate directions at once, also makes sure the player stops when hitting a corner (as that would trigger front and right or left at once, canceling eachother out).
    {
        RaycastHit leftWall;
        RaycastHit rightWall;
        RaycastHit frontWall;
        if (!isWallRunning)
        {
            isFront = false;
            isLeft = false;
            isRight = false;
        }
        if (Physics.Raycast(dirParent.position, dirParent.forward, out frontWall, 10f)) //sends out rays to check if there is a wall in front.
        {

            if (frontWall.transform.tag == "RunnableWall")
            {
                distFrontWall = Vector3.Distance(dirParent.position, frontWall.point);
                if (distFrontWall < 0.6f)
                {
                    isFront = true;
                    isLeft = false;
                    isRight = false;
                }
                else
                {
                    leftSideForceAct = false;
                    rightSideForceAct = false;
                    frontForceAct = false;
                    isFront = false;
                }
            }
        }

        if (Physics.Raycast(dirParent.position, dirParent.right, out rightWall, 10f)) //sends out a ray to check if there is a wall to the right
        {
            if (rightWall.transform.tag == "RunnableWall")
            {
                distRightWall = Vector3.Distance(dirParent.position, rightWall.point);

                if (distRightWall < 0.6f)
                {
                    isFront = false;
                    isRight = true;
                    isLeft = false;

                }
                else
                {
                    leftSideForceAct = false;
                    rightSideForceAct = false;
                    frontForceAct = false;
                    isRight = false;
                }
            }
        }

        if (Physics.Raycast(dirParent.position, -dirParent.right, out leftWall, 10f)) //sends out a ray to check if there is a wall to the left
        {
            if (leftWall.transform.tag == "RunnableWall")
            {
                distLeftWall = Vector3.Distance(dirParent.position, leftWall.point);

                if (distLeftWall < 0.6f)
                {
                    isFront = false;
                    isLeft = true;
                    isRight = false;

                }
                else
                {
                    leftSideForceAct = false;
                    rightSideForceAct = false;
                    frontForceAct = false;
                    isLeft = false;
                }
            }
        }

        if ((isFront || isLeft || isRight) && !playerMovement.isGrounded) 
        {
            isWallRunning = true;
            TiltCameraStart();
        }
        else
        {
            currentWallJumpNo = 0;
            isWallRunning = false;
            leftSideForceAct = false;
            rightSideForceAct = false;
            frontForceAct = false;
        }
    }


    private void TiltCameraStart() //tilts the camera on the x axis to give the impression that the player is leaning away from the wall while wallrunning.
    {
        if (isRight == true)
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpCamTrans.localRotation = Quaternion.Slerp(currentCamAng, rightWallRunCamAng, timeCount);
        }

        if (isLeft == true)
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpCamTrans.localRotation = Quaternion.Slerp(currentCamAng, leftWallRunCamAng, timeCount);
        }

        if (isFront == true)
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpCamTrans.localRotation = Quaternion.Slerp(currentCamAng, frontWallRunCamAng, timeCount);
        }
    }



    private void TiltCameraStop() //returns the camera tilt in the x-axis to the normal horizontal alignement when not wallrunning.
    {

        if (isRight == false && isFront == false && isLeft == false || playerMovement.isGrounded)
        {
            timeCount = timeCount + Time.deltaTime * 3;
        }
    }

    void WallJumpCheck() //makes sure you can only jump from the wall in the correct direction depending on wall perpendicular direction
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentWallJumpNo < maxWallJumps && !climbing.isClimbing)
        {
            canWallJump = true;


            if (isLeft == true && isWallRunning)
            {
                jumpBoostRight = true;
                jumpBoostLeft = false;
                jumpBoostBack = false;

            }
            else
            {
                jumpBoostRight = false;
            }

            if (isRight == true && isWallRunning)
            {
                jumpBoostLeft = true;
                jumpBoostRight = false;
                jumpBoostBack = false;


            }
            else
            {
                jumpBoostLeft = false;
            }
            if (isFront == true && isWallRunning)
            {
                jumpBoostBack = true;
                jumpBoostRight = false;
                jumpBoostLeft = false;
            }
            else
            {
                jumpBoostBack = false;
            }
        }
    }

}
