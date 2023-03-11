using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private GrapplingGun grapplingGun;
    [SerializeField] private Transform directionParent;
    [SerializeField] private Transform actualCam;
    [SerializeField] private CapsuleCollider playerCol;

    public Rigidbody playerRB;
    public Transform topOfPlayer;
    public Transform middleOfPlayer;
    public Transform targetPositionLeft;
    public Transform player;
    public Quaternion ledgeCurrentRotation;
    public Quaternion normalRot;
    public Vector3 ledgeFace;
    public Vector3 targetHeight;
    public GameObject ledgeTopRayOrigin;
    public Animator freyjaAnimator;
    public AvatarTarget rootTransform;
    public MatchTargetWeightMask weightMask;

    public float rotationToLedgeTimeCount;
    public float releaseCamera = 0.1f;
    public float ledgeRotationDelay;

    public float climbLedgePower = 1100f;
    public float peakHeight = 200f;

    public bool isClimbing;
    public bool canClimb;
    public bool middleHit;
    public bool topHit;
    public bool canVault;
    public bool hasClimbed;
    public bool isPeaking;

    public bool climbingUp;
    public bool climbCollider;

    public Vector3 wallNormal;

    public PlayerLook playerLook;
    public float direcyEuler;
    public float faceLedgeSpeed;
    public RaycastHit hitpointLedge;

    public Transform ledgeEdge;
    public Vector3 standingPoint;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        grapplingGun = FindObjectOfType<GrapplingGun>();
        playerLook = FindObjectOfType<PlayerLook>();

        middleHit = false;
        topHit = false;
        isClimbing = false;
        canClimb = true;
        canVault = false;
        hasClimbed = false;
        faceLedgeSpeed = 0;
    }

    // Update is called once per frame
    void Update()
    {

        TopCheck();
        MiddleCheck();
        hangDist();
        ledgeCheck();
        climbing();
        vaultCheck();
    }

    private void LateUpdate()
    {
        ledgeMovement();
    }

    public void ledgeCheck()
    {
        RaycastHit ledgeHeightHit;
        if (Physics.Raycast(ledgeTopRayOrigin.transform.position, -ledgeTopRayOrigin.transform.up, out ledgeHeightHit, 1.5f, climbableLayer))
        {
            Debug.DrawRay(ledgeTopRayOrigin.transform.position, -ledgeTopRayOrigin.transform.up, Color.cyan);
            var hitHeight = ledgeHeightHit.point;
            hitHeight.y = ledgeHeightHit.point.y - 1f;
            hitHeight.x = directionParent.position.x;
            hitHeight.z = directionParent.position.z;

            targetHeight = new Vector3(hitHeight.x, hitHeight.y, hitHeight.z);

            standingPoint = ledgeHeightHit.point;
            if (ledgeHeightHit.collider.transform.Find("LedgeEdge"))
            {
                ledgeEdge = ledgeHeightHit.collider.transform.GetChild(0);
            }
            else
            {
                ledgeEdge = null;
            }
        }
        else
        {
            standingPoint = Vector3.zero;
        }
    }

    public void TopCheck()
    {
        RaycastHit hitTop;
        if (Physics.Raycast(topOfPlayer.position, topOfPlayer.forward, out hitTop, 2f, climbableLayer))
        {
            Debug.DrawRay(topOfPlayer.position, topOfPlayer.forward, Color.yellow);
            topHit = true;
        }
        else
        {
            topHit = false;
        }
    }

    public void MiddleCheck()
    {
        RaycastHit hitMiddle;
        if (Physics.Raycast(middleOfPlayer.position, middleOfPlayer.forward, out hitMiddle, 2f, climbableLayer))
        {
            Debug.DrawRay(middleOfPlayer.position, middleOfPlayer.forward, Color.cyan);
            hitpointLedge = hitMiddle;
            middleHit = true;
            ledgeFace = hitMiddle.normal;

            if (isClimbing && !climbingUp)
            {
                player.transform.localPosition = Vector3.MoveTowards(player.transform.position, hitMiddle.point - new Vector3(0f, 0f, 0.3f), 1f * Time.deltaTime);
            }
        }
        else
        {
            middleHit = false;
        }
    }

    public void vaultCheck()
    {
        if (topHit)
        {
            canVault = false;
        }
        else if (!topHit && !middleHit)
        {
            canVault = false;
        }

        else if (topHit && middleHit && playerMovement.playerOnGround)
        {
            canVault = true;
        }
    }

    public void ledgeMovement()
    {
        if (isClimbing)
        {
            if (Input.GetKey(KeyCode.S))
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartCoroutine("justClimbed");
                }
            }
            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartCoroutine("climbUp");
                }
            }
        }
    }

    void climbing()
    {
        if (middleHit && !topHit && !playerMovement.playerOnGround && canClimb && !grapplingGun.isGrappling && !isClimbing)
        {
            playerLook.yRotation = 0;
            playerLook.cameraHolder.transform.rotation = directionParent.transform.rotation;
            if (standingPoint != Vector3.zero)
            {
                isClimbing = true;
            }
        }
    }


    public void hangDist()
    {
        if (isClimbing)
        {
            if (Input.GetMouseButton(1) && !playerMovement.playerOnGround)
            {
                if (!isPeaking)
                {
                    isPeaking = true;
                    if (player.transform.position.y <= 0f)
                    {
                        Vector3 ledgePeak = new Vector3(player.transform.position.x, player.transform.position.y + 0.85f, player.transform.position.z);
                        player.transform.position = Vector3.Slerp(player.transform.position, ledgePeak, peakHeight * Time.deltaTime);
                        Debug.Log("THIS ISN*T WORKING 2!");
                    }
                    else if (player.transform.position.y >= 0f)
                    {
                        Vector3 ledgePeak = new Vector3(player.transform.position.x, player.transform.position.y + 0.85f, player.transform.position.z);
                        player.transform.position = Vector3.Slerp(player.transform.position, ledgePeak, peakHeight * Time.deltaTime);
                        Debug.Log("THIS ISN*T WORKING!");
                    }
                }
            }
            else if (!climbingUp)
            {
                isPeaking = false;
                playerRB.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetHeight, 300f * Time.deltaTime);
            }
            playerLook.cameraHolder.transform.rotation = directionParent.transform.rotation;
        }
    }
    IEnumerator justClimbed()
    {
        canClimb = false;
        isClimbing = false;
        yield return new WaitForSeconds(0.5f);
        canClimb = true;
    }

    IEnumerator climbUp()
    {
        canClimb = false;
        climbingUp = true;
        yield return new WaitForSeconds(1.13f);
        canClimb = true;
        isClimbing = false;
        climbingUp = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 13)
        {
            climbCollider = true;

        }
        else
        {
            climbCollider = false;
        }
    }
}
