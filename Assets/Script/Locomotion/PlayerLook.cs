using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Manually assigned variables")]
    [SerializeField] public Transform camParent;
    [SerializeField] private Transform fpCamTrans;
    [SerializeField] private Transform camFollowTrans;
    [SerializeField] private Transform dirParent;

    [Header("Editable in inspector")]
    [SerializeField] public float mouseSens = 100f;

    [Header("Visible for debugging")]
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;

    [Header("Input stuff")]
    public float xRotation;
    public float yRotation;
    private float ClampedyRotation;
    private float ClampedxRotation;
    private PlayerHealth playHealth;
    private Climbing climbing;
    private WallRun wallrun;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playHealth = FindObjectOfType<PlayerHealth>();
        climbing = FindObjectOfType<Climbing>();
        wallrun = FindObjectOfType<WallRun>();
    }

    void Update()
    {
        getInputs();

        if (playHealth.isAlive) 
        {
            if (climbing.isClimbing)
            {
                var ledgeDir = climbing.hitpointLedge.normal;
                ledgeDir.y = 0;
                dirParent.transform.rotation = Quaternion.Slerp(dirParent.transform.rotation, Quaternion.LookRotation(-ledgeDir), Time.deltaTime * 10f);

                fpCamTrans.transform.localRotation = Quaternion.Euler(ClampedxRotation, ClampedyRotation, 0);
            }
            else
            {
                Quaternion defaultCameraTilt = Quaternion.Euler(ClampedxRotation, 0, 0);

                if (!wallrun.isRight && !wallrun.isLeft || !wallrun.isWallRunning)
                {
                    Vector3 tiltedCamera = fpCamTrans.transform.eulerAngles;
                    tiltedCamera = new Vector3(ClampedxRotation, 0, tiltedCamera.z);
                    Quaternion tiltedCameraQuat = Quaternion.Euler(tiltedCamera.x, tiltedCamera.y, tiltedCamera.z);

                    fpCamTrans.transform.localRotation = Quaternion.Slerp(tiltedCameraQuat, defaultCameraTilt, Time.deltaTime * 2f);

                }
                camParent.transform.Rotate(mouseX * Vector3.up, Space.World); // rotate camera left right
                dirParent.transform.Rotate(Vector3.up * mouseX);
            }
        }
    }

    public void getInputs()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSens * Time.fixedDeltaTime;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSens * Time.fixedDeltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;


        ClampedxRotation = Mathf.Clamp(xRotation, -80f, 70f);
        ClampedyRotation = Mathf.Clamp(yRotation, -80f, 70f);
    }
}

