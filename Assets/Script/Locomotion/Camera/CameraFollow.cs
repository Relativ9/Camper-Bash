using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform cameraFollowPos;
    [SerializeField] Transform actualCamera;

    [SerializeField] bool enableBob = true;
    [SerializeField] float bobSize = 0.005f;
    [SerializeField] float bobSpeed = 15f;
    [SerializeField] float resetSpeed = 15f;
    [SerializeField] float multiplier = 100f;
    //private float velSpeed;
    //private float frequency;

    [SerializeField] float toggleSpeed = 0.1f;
    private Vector3 startPos;
    private Rigidbody playerRB;
    private PlayerMovement playerMovement;
    //public PauseMenuController pauseMenuController;

    [SerializeField] bool tpsCamOn;

    // Start is called before the first frame update
    void Start()
    {
        //tpsCamOn = false;
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerRB = FindObjectOfType<PlayerMovement>().GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        startPos = actualCamera.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, cameraFollowPos.position, multiplier);
        //velSpeed = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z).magnitude;

        restartPos();

        //if (enableBob)
        //{
        //    checkMotion();
        //    actualCamera.LookAt(bobStable());
        //}

    }
    private void restartPos()
    {
        if (actualCamera.localPosition == startPos) return;
        actualCamera.localPosition = Vector3.Lerp(actualCamera.localPosition, startPos, resetSpeed * Time.deltaTime);
    }

    //private void checkMotion()
    //{

    //    if (velSpeed < toggleSpeed) return;
    //    if (!playerMovement.playerOnGround) return;
    //    if (playerMovement.isMoving)
    //    {
    //        playMotion(footStepMotion());
    //    }
    //}

    private Vector3 footStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * bobSpeed) * bobSize;
        pos.x += Mathf.Cos(Time.time * bobSpeed / 2) * bobSize;
        return pos;
    }

    private void playMotion(Vector3 motion)
    {
        actualCamera.localPosition += motion;
    }
}
