using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float mouseSens = 100f;

    [SerializeField] public Transform cameraHolder;
    [SerializeField] public Transform cameraItself;
    [SerializeField] Transform cameraFollowTrans;
    [SerializeField] Transform directionParent;

    public RaycastHit lookHit;


    private float mouseX;
    private float mouseY;

    public float xRotation;
    public float yRotation;

    private float ClampedyRotation;
    private float ClampedxRotation;

    public bool unlockCursor;
    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] private Climbing climbing;
    [SerializeField] private WallRun wallrun;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;
        cameraHolder = GameObject.Find("CameraHolder").transform;
        playerMovement = FindObjectOfType<PlayerMovement>();
        wallrun = FindObjectOfType<WallRun>();
        cameraItself = Camera.main.transform;


    }

    void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSens * Time.fixedDeltaTime;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSens * Time.fixedDeltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;


        ClampedxRotation = Mathf.Clamp(xRotation, -80f, 70f);
        ClampedyRotation = Mathf.Clamp(yRotation, -80f, 70f);


            if (climbing.isClimbing)
            {
                var ledgeDir = climbing.hitpointLedge.normal;
                ledgeDir.y = 0;
                directionParent.transform.rotation = Quaternion.Slerp(directionParent.transform.rotation, Quaternion.LookRotation(-ledgeDir), Time.deltaTime * 10f);

                cameraItself.transform.localRotation = Quaternion.Euler(ClampedxRotation, ClampedyRotation, 0);
            }
            else
            {

                Quaternion defaultCameraTilt = Quaternion.Euler(ClampedxRotation, 0, 0);

                if (!wallrun.isRight && !wallrun.isLeft || !wallrun.isWallRunning)
                {
                    Vector3 tiltedCamera = cameraItself.transform.eulerAngles;
                    tiltedCamera = new Vector3(ClampedxRotation, 0, tiltedCamera.z);
                    Quaternion tiltedCameraQuat = Quaternion.Euler(tiltedCamera.x, tiltedCamera.y, tiltedCamera.z);

                    cameraItself.transform.localRotation = Quaternion.Slerp(tiltedCameraQuat, defaultCameraTilt, Time.deltaTime * 2f);

                }
                cameraHolder.transform.Rotate(mouseX * Vector3.up, Space.World); // rotate camera left right
                directionParent.transform.Rotate(Vector3.up * mouseX);
            }
    }
}

