using System.Collections;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimatorStates : MonoBehaviour
{
    //Assigned in start
    private Climbing climb;
    private WallRun wallRun;
    private PlayerMovement playerMove;
    private RigBuilder rigLayers;
    private Weapon weapon;
    private Animator anim;

    [Header("Editable in inspector")]
    public float multiplier = 10;

    [Header("Visible for debugging")]
    public Rig BodyHead;
    public Rig WeaponRest;
    public Rig WeaponAiming;
    public Rig ShootPose;
    public Rig WeaponOnBack;
    public Rig GrappleRifleIK;
    public Rig ClimbingIK;

    public void Start()
    {
        climb = FindObjectOfType<Climbing>();
        wallRun = FindObjectOfType<WallRun>();
        playerMove = FindObjectOfType<PlayerMovement>();
        weapon = FindObjectOfType<Weapon>();
        anim = this.gameObject.GetComponent<Animator>();

        rigLayers = this.gameObject.GetComponent<RigBuilder>();
        BodyHead = rigLayers.layers[0].rig;
        WeaponRest = rigLayers.layers[1].rig;
        ShootPose = rigLayers.layers[2].rig;
        WeaponOnBack = rigLayers.layers[3].rig;
        WeaponAiming = rigLayers.layers[4].rig;
        GrappleRifleIK = rigLayers.layers[6].rig;
        ClimbingIK = rigLayers.layers[5].rig;
    }

    public void Update()
    {
        if (climb.isClimbing)
        {
            anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
        }
        else
        {
            StartCoroutine(setAnimatorNormal());
        }

        if (climb.isClimbing)
        {
            BodyHead.weight = 0f;
            WeaponRest.weight = 0f * Time.deltaTime * multiplier;
            ShootPose.weight = 0f * Time.deltaTime * multiplier;
            WeaponOnBack.weight = 1f;
            WeaponAiming.weight = 0f;
            ClimbingIK.weight = 1f;
            GrappleRifleIK.weight = 0f;

        }
        else if (wallRun.isWallRunning && weapon.slotFull)
        {
            BodyHead.weight = 1f;
            WeaponRest.weight = 1f * Time.deltaTime * multiplier;
            ShootPose.weight = 0f * Time.deltaTime * multiplier;
            WeaponOnBack.weight = 0f;
            WeaponAiming.weight = 0f;
            ClimbingIK.weight = 0f;
            GrappleRifleIK.weight = 1f;
        }
        else if (!weapon.slotFull)
        {
            BodyHead.weight = 1f;
            WeaponRest.weight = 0f * Time.deltaTime * multiplier;
            ShootPose.weight = 0f * Time.deltaTime * multiplier;
            WeaponOnBack.weight = 1f;
            WeaponAiming.weight = 0f;
            ClimbingIK.weight = 0f;
            GrappleRifleIK.weight = 0f;

        }
        else if (playerMove.isRunning)
        {
            BodyHead.weight = 1f;
            WeaponRest.weight = 1f * Time.deltaTime * multiplier;
            ShootPose.weight = 0f * Time.deltaTime * multiplier;
            WeaponOnBack.weight = 0f;
            WeaponAiming.weight = 0f;
            ClimbingIK.weight = 0f;
            GrappleRifleIK.weight = 1f;

        }
        else
        {
            BodyHead.weight = 1f;
            WeaponRest.weight = 0f * Time.deltaTime * multiplier;
            ShootPose.weight = 1f * Time.deltaTime * multiplier;
            WeaponOnBack.weight = 0f;
            WeaponAiming.weight = 1f;
            ClimbingIK.weight = 0f;
            GrappleRifleIK.weight = 1f;
        }
    }

    IEnumerator setAnimatorNormal()
    {
        yield return new WaitForSeconds(0.25f);
        anim.updateMode = AnimatorUpdateMode.Normal;
    }
}
