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

    //private int justClimbed = 1;

    public bool unlockCursor;
    //[SerializeField] public Npc npc;
    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] private Climbing climbing;
    [SerializeField] private WallRun wallrun;

    //ToggleInventoryUI toggleInventoryUI;

    //public PauseMenuController pauseMenuController;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;
        //npc = FindObjectOfType<Npc>();
        cameraHolder = GameObject.Find("CameraHolder").transform;
        playerMovement = FindObjectOfType<PlayerMovement>();
        //toggleInventoryUI = FindObjectOfType<ToggleInventoryUI>();
        //pauseMenuController = FindObjectOfType<PauseMenuController>();
        wallrun = FindObjectOfType<WallRun>();
        cameraItself = Camera.main.transform;


    }

    void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSens * Time.fixedDeltaTime;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSens * Time.fixedDeltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;


        ClampedxRotation = Mathf.Clamp(xRotation, -80f, 80f);
        ClampedyRotation = Mathf.Clamp(yRotation, -80f, 80f);

        //cameraHolder.transform.rotation = Quaternion.Euler(ClampedxRotation, yRotation, 0);
        //orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        //if (unlockCursor == true)
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Time.timeScale = 0f;
        //    Cursor.visible = true;

        //}

        //else if (unlockCursor == false)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Time.timeScale = 1f;
        //    Cursor.visible = false;
        //}

        //if (playerMovement.inChatwithNpc == true || toggleInventoryUI.showingInventory == true || pauseMenuController.isGamePaused == true)
        //{
        //    unlockCursor = true;
        //}
        //if (playerMovement.inChatwithNpc == false && toggleInventoryUI.showingInventory == false && pauseMenuController.isGamePaused == false)
        //{
        //    unlockCursor = false;

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
                    //cameraItself.transform.localRotation = Quaternion.Euler(ClampedxRotation, 0, 0); // rotatate camera up down

                    cameraItself.transform.localRotation = Quaternion.Slerp(tiltedCameraQuat, defaultCameraTilt, Time.deltaTime * 2f);

                }
                cameraHolder.transform.Rotate(mouseX * Vector3.up, Space.World); // rotate camera left right
                directionParent.transform.Rotate(Vector3.up * mouseX);
                //cameraItself.transform.localRotation = defaultCameraTilt;
            }


        //}
        //HeadTurn();
    }

    //public void HeadTurn()
    //{
    //    RaycastHit lookHit;
    //    Physics.Raycast(cameraItself.transform.position, cameraItself.transform.forward, out lookHit, 1000f);
    //}
}

