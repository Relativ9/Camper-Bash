using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Manually assigned in editor")]
    public Transform fpCamTrans;
    public Transform dirParent;

    [Header("Editable in inspector")]
    [SerializeField] private float upforce = 14f;
    [SerializeField] private float sideforce = 20f;
    [SerializeField] private float wallJumpForce = 120f;
    [SerializeField] private int maxWallJumps = 2; // This is to allow us do any ONLY two jumps while wall running
    [SerializeField] private int currentWallJumpNo = 0; // stores number of wall jumps we do
    [SerializeField] private float multiplier = 2f;
    float timeCount = 0.0f; // For Camera Tilt Slerp


    [Header("Visible for debugging")]
    private Quaternion currentCameraAngle;
    [SerializeField] public bool isLeft;
    [SerializeField] public bool isRight;
    [SerializeField] public bool isFront;
    [SerializeField] private float distanceFromLeftWall;
    [SerializeField] private float distanceFromRightWall;
    [SerializeField] private float distanceFromFrontWall;


    public bool isWallRunning = false;
    private bool canWallJump;

    private Quaternion rightWallRunCameraAngle = Quaternion.Euler(0f, 0f, 12f);
    private Quaternion leftWallRunCameraAngle = Quaternion.Euler(0f, 0f, -12f);
    private Quaternion frontWallRunCameraAngle = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion defaultCameraAngle = Quaternion.Euler(0f, 0f, 0f);


    private Rigidbody playerRigidbody;
    private PlayerMovement playerMovement;
    private Climbing climbing;

    private bool jumpBoostRight;
    private bool jumpBoostLeft;
    private bool jumpBoostBack;
    private bool rightSideForceActive;
    private bool leftSideForceActive;
    private bool frontForceActive;
    private bool hasPressedWallJump;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        currentCameraAngle = defaultCameraAngle;
    }

    void Start()
    {
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

    private void ActivateSideForces()
    {
        if (rightSideForceActive && !playerMovement.playerOnGround)
        {
            playerRigidbody.AddRelativeForce(dirParent.right * sideforce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            leftSideForceActive = false;
            frontForceActive = false;
        }

        if (leftSideForceActive && !playerMovement.playerOnGround)
        {
            playerRigidbody.AddRelativeForce(-dirParent.right * sideforce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            rightSideForceActive = false;
            frontForceActive = false;
        }

        if (frontForceActive && !playerMovement.playerOnGround)
        {
            playerRigidbody.AddRelativeForce(dirParent.forward * sideforce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            rightSideForceActive = false;
            leftSideForceActive = false;
        }


    }

    private void WallJump()
    {
        if (canWallJump)
        {
            playerRigidbody.AddRelativeForce(dirParent.up * (upforce * multiplier / 2f), ForceMode.Impulse);
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

    private void WallChecker()
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
        if (Physics.Raycast(dirParent.position, dirParent.forward, out frontWall, 10f))
        {

            if (frontWall.transform.tag == "RunnableWall")
            {
                distanceFromFrontWall = Vector3.Distance(dirParent.position, frontWall.point);
                if (distanceFromFrontWall < 0.6f) // (this had an && isWallrunning if included)
                {
                    isFront = true;
                    isLeft = false;
                    isRight = false;
                }
                else
                {
                    leftSideForceActive = false;
                    rightSideForceActive = false;
                    frontForceActive = false;
                    isFront = false;
                }
            }
        }

        if (Physics.Raycast(dirParent.position, dirParent.right, out rightWall, 10f))
        {
            if (rightWall.transform.tag == "RunnableWall")
            {
                distanceFromRightWall = Vector3.Distance(dirParent.position, rightWall.point);

                if (distanceFromRightWall < 0.6f) // (this had an && isWallrunning if included)
                {
                    isFront = false;
                    isRight = true;
                    isLeft = false;

                }
                else
                {
                    leftSideForceActive = false;
                    rightSideForceActive = false;
                    frontForceActive = false;
                    isRight = false;
                }
            }
        }

        if (Physics.Raycast(dirParent.position, -dirParent.right, out leftWall, 10f))
        {
            if (leftWall.transform.tag == "RunnableWall")
            {
                distanceFromLeftWall = Vector3.Distance(dirParent.position, leftWall.point);

                if (distanceFromLeftWall < 0.6f) // (this had an && isWallrunning if included)
                {
                    isFront = false;
                    isLeft = true;
                    isRight = false;

                }
                else
                {
                    leftSideForceActive = false;
                    rightSideForceActive = false;
                    frontForceActive = false;
                    isLeft = false;
                }
            }
        }

        if ((isFront || isLeft || isRight) && !playerMovement.playerOnGround)
        {
            isWallRunning = true;
            TiltCameraStart();
        }
        else
        {
            currentWallJumpNo = 0;
            isWallRunning = false;
            leftSideForceActive = false;
            rightSideForceActive = false;
            frontForceActive = false;
        }
    }


    private void TiltCameraStart()
    {
        if (isRight == true) // this is set in our raycast
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpCamTrans.localRotation = Quaternion.Slerp(currentCameraAngle, rightWallRunCameraAngle, timeCount);
        }

        if (isLeft == true) // this is set in our raycast
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpCamTrans.localRotation = Quaternion.Slerp(currentCameraAngle, leftWallRunCameraAngle, timeCount);
        }

        if (isFront == true)
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpCamTrans.localRotation = Quaternion.Slerp(currentCameraAngle, frontWallRunCameraAngle, timeCount);
        }

        // camera tilt works in update function because of time.deltatime so you cant put it in OnCollisionExit
    }



    private void TiltCameraStop()
    {

        if (isRight == false && isFront == false && isLeft == false)
        {
            timeCount = timeCount + Time.deltaTime * 3;
        }
        // camera tilt works in update function because of time.deltatime so you cant put it in OnCollisionExit
    }

    void WallJumpCheck()
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
