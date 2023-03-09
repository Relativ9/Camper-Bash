using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wall Run")]
    [SerializeField] private float upforce = 14f;
    [SerializeField] private float sideforce = 20f;
    [SerializeField] public bool isWallRunning = false;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float wallJumpForce = 120f;

    [Header("Camera Rotation for WallRun")]
    private Quaternion currentCameraAngle;
    private Quaternion rightWallRunCameraAngle = Quaternion.Euler(0f, 0f, 12f);
    private Quaternion leftWallRunCameraAngle = Quaternion.Euler(0f, 0f, -12f);
    private Quaternion frontWallRunCameraAngle = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion defaultCameraAngle = Quaternion.Euler(0f, 0f, 0f);


    private Rigidbody playerRigidbody;
    public Transform directionParent;
    public Transform fpsCamera;


    public bool isLeft;
    public bool isRight;
    public bool isFront;

    public float distanceFromLeftWall;
    public float distanceFromRightWall;
    public float distanceFromFrontWall;

    float timeCount = 0.0f; // For Camera Tilt Slerp
    [SerializeField] private float multiplier = 2f;

    [Header("Jump Limit During WallRun")]

    [SerializeField] private int maxWallJumps = 2; // This is to allow us do any ONLY two jumps while wall running
    [SerializeField] private int currentWallJumpNo = 0; // stores number of wall jumps we do
    private bool canWallJump;
    public bool jumpBoostRight;
    public bool jumpBoostLeft;
    public bool jumpBoostBack;
    private bool rightSideForceActive;
    private bool leftSideForceActive;
    private bool frontForceActive;

    public bool hasPressedWallJump;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        currentCameraAngle = defaultCameraAngle;
    }

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
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
            playerRigidbody.AddRelativeForce(directionParent.right * sideforce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            leftSideForceActive = false;
            frontForceActive = false;
        }

        if (leftSideForceActive && !playerMovement.playerOnGround)
        {
            playerRigidbody.AddRelativeForce(-directionParent.right * sideforce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            rightSideForceActive = false;
            frontForceActive = false;
        }

        if (frontForceActive && !playerMovement.playerOnGround)
        {
            playerRigidbody.AddRelativeForce(directionParent.forward * sideforce * multiplier * Time.fixedDeltaTime); // allows player to stick to wall during wallrun
            rightSideForceActive = false;
            leftSideForceActive = false;
        }


    }

    private void WallJump()
    {
        if (canWallJump)
        {
            playerRigidbody.AddRelativeForce(directionParent.up * (upforce * multiplier / 2f), ForceMode.Impulse);
            currentWallJumpNo++;
            canWallJump = false;

            if (jumpBoostLeft)
            {
                playerRigidbody.AddRelativeForce(-directionParent.right * wallJumpForce * multiplier * Time.fixedDeltaTime, ForceMode.Impulse);
                jumpBoostLeft = false;
            }

            if (jumpBoostRight)
            {
                playerRigidbody.AddRelativeForce(directionParent.right * wallJumpForce * multiplier * Time.fixedDeltaTime, ForceMode.Impulse);
                jumpBoostRight = false;
            }

            if (jumpBoostBack)
            {
                playerRigidbody.AddRelativeForce(-directionParent.forward * wallJumpForce * multiplier * Time.fixedDeltaTime, ForceMode.Impulse);
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
        if (Physics.Raycast(directionParent.position, directionParent.forward, out frontWall, 10f))
        {

            if (frontWall.transform.tag == "RunnableWall")
            {
                distanceFromFrontWall = Vector3.Distance(directionParent.position, frontWall.point);
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

        if (Physics.Raycast(directionParent.position, directionParent.right, out rightWall, 10f))
        {
            if (rightWall.transform.tag == "RunnableWall")
            {
                distanceFromRightWall = Vector3.Distance(directionParent.position, rightWall.point);

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

        if (Physics.Raycast(directionParent.position, -directionParent.right, out leftWall, 10f))
        {
            if (leftWall.transform.tag == "RunnableWall")
            {
                distanceFromLeftWall = Vector3.Distance(directionParent.position, leftWall.point);

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



    //private void OnCollisionEnter(Collision whatWeCollidedWith)
    //{

    //    if (playerMovement.playerOnGround && whatWeCollidedWith.gameObject.tag != "RunnableWall") // Prevents player from doing double or jumping in the air
    //    {
    //       currentWallJumpNo = 0;
    //        isWallRunning = false;
    //    }
    //    //else if (playerMovement.playerOnGround && whatWeCollidedWith.gameObject.tag == "RunnableWall") // Prevents player from doing double or jumping in the air
    //    //{
    //    //   currentWallJumpNo = 0;
    //    //    isWallRunning = false;
    //    //}

    //    if (playerMovement.playerOnGround || whatWeCollidedWith.gameObject.tag == "RunnableWall")
    //    {
    //        currentWallJumpNo = 0;
    //        isWallRunning = false;
    //    }

    //}



    //private void OnCollisionStay(Collision whatWeCollidedWith)
    //{

    //    if (whatWeCollidedWith.transform.CompareTag("RunnableWall") && playerMovement.playerOnGround == false)
    //    {
    //        isWallRunning = true;
    //        TiltCameraStart();

    //        if (isRight == true)
    //        {
    //            rightSideForceActive = true;
    //        }

    //        if (isLeft == true)
    //        {
    //            leftSideForceActive = true;
    //        }

    //        if (isFront == true)
    //        {
    //            frontForceActive = true;
    //        }

    //    } else
    //    {
    //        leftSideForceActive = false;
    //        rightSideForceActive = false;
    //        frontForceActive = false;
    //    }


    //    if (whatWeCollidedWith.transform.CompareTag("RunnableWall") && playerMovement.playerOnGround == true)
    //    {
    //        isWallRunning = false;
    //        currentWallJumpNo = 0;
    //    }

    //}



    //private void OnCollisionExit(Collision whatWeCollidedWith)
    //{

    //    if (whatWeCollidedWith.transform.CompareTag("RunnableWall"))
    //    {
    //        timeCount = 0f;
    //        currentWallJumpNo = 0;
    //        isWallRunning = false; 
    //    }

    //}


    private void TiltCameraStart()
    {
        if (isRight == true) // this is set in our raycast
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpsCamera.localRotation = Quaternion.Slerp(currentCameraAngle, rightWallRunCameraAngle, timeCount);
        }

        if (isLeft == true) // this is set in our raycast
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpsCamera.localRotation = Quaternion.Slerp(currentCameraAngle, leftWallRunCameraAngle, timeCount);
        }

        if (isFront == true)
        {
            timeCount = timeCount + Time.deltaTime * 3;
            fpsCamera.localRotation = Quaternion.Slerp(currentCameraAngle, frontWallRunCameraAngle, timeCount);
        }

        // camera tilt works in update function because of time.deltatime so you cant put it in OnCollisionExit
    }



    private void TiltCameraStop()
    {

        if (isRight == false && isFront == false && isLeft == false)
        {
            timeCount = timeCount + Time.deltaTime * 3;
            //fpsCamera.localRotation = Quaternion.Slerp(fpsCamera.localRotation, defaultCameraAngle, timeCount);

            // initially didnt work because i had soo many non-slerp rotations (snapping back into position) happening.
            // so had to look for each and every instance of it and replace with the proper slerp codes.
        }
        // camera tilt works in update function because of time.deltatime so you cant put it in OnCollisionExit
    }

    void WallJumpCheck()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentWallJumpNo < maxWallJumps)
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
