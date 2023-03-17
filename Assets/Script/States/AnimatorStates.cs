using System.Collections;
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
    private GrappleHook grapple;

    [Header("Editable in inspector")]
    public float multiplier = 10;

    [Header("Visible for debugging")]
    public Rig BodyHead;
    public Rig WeaponRestPose;
    public Rig WeaponAiming;
    public Rig ShootPose;
    public Rig WeaponOnBack;
    public Rig GrappleRifleIK;
    public Rig ClimbingIK;
    public Rig GrappleAiming;

    public void Start()
    {
        climb = FindAnyObjectByType<Climbing>();
        wallRun = FindAnyObjectByType<WallRun>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
        weapon = FindAnyObjectByType<Weapon>();
        anim = GetComponent<Animator>();
        grapple = FindAnyObjectByType<GrappleHook>();

        rigLayers = GetComponent<RigBuilder>();
        BodyHead = rigLayers.layers[0].rig;
        WeaponRestPose = rigLayers.layers[1].rig;
        ShootPose = rigLayers.layers[2].rig;
        WeaponOnBack = rigLayers.layers[3].rig;
        WeaponAiming = rigLayers.layers[4].rig;
        GrappleRifleIK = rigLayers.layers[6].rig;
        ClimbingIK = rigLayers.layers[5].rig;
        GrappleAiming = rigLayers.layers[7].rig;
    }

    public void Update()
    {
        if (climb.isClimbing)
        {
            anim.updateMode = AnimatorUpdateMode.Fixed;
        }
        else
        {
            StartCoroutine(setAnimatorNormal());
        }

        ClimbRig();
        AimRig();
        RestRig();
        GrappleRig();
        DefaultRig();
    }

    public void ClimbRig()
    {
        if(!climb.isClimbing)
        {
            return;
        }

        if(weapon.slotFull)
        {
            ClimbingIK.weight = 1f;
            WeaponOnBack.weight = 1f;

            BodyHead.weight = 0f;
            WeaponRestPose.weight = 0f;
            ShootPose.weight = 0f;
            WeaponAiming.weight = 0f;
            GrappleRifleIK.weight = 0f;
            GrappleAiming.weight = 0f;
        } else
        {
            ClimbingIK.weight = 1f;
            WeaponOnBack.weight = 0f;

            BodyHead.weight = 0f;
            WeaponRestPose.weight = 0f;
            ShootPose.weight = 0f;
            WeaponAiming.weight = 0f;
            GrappleRifleIK.weight = 0f;
            GrappleAiming.weight = 0f;
        }
    }

    public void AimRig()
    {
        if(!weapon.slotFull || grapple.isGrappling || playerMove.isRunning || climb.isClimbing)
        {
            return;
        }

        WeaponAiming.weight = 1f;
        GrappleRifleIK.weight = 1f;

        BodyHead.weight = 1f;
        WeaponRestPose.weight = 0f * Time.deltaTime * multiplier;
        ShootPose.weight = 1f * Time.deltaTime * multiplier;
        WeaponOnBack.weight = 0f;
        GrappleAiming.weight = 0f;
        ClimbingIK.weight = 0f;
    }

    public void RestRig()
    {
        if(!weapon.slotFull || grapple.isGrappling || !playerMove.isRunning || climb.isClimbing)
        {
            return;
        }

        WeaponRestPose.weight = 1f * Time.deltaTime * multiplier;
        GrappleRifleIK.weight = 1f;

        BodyHead.weight = 1f;
        ShootPose.weight = 0f * Time.deltaTime * multiplier;
        WeaponAiming.weight = 0f; 
        WeaponOnBack.weight = 0f;
        GrappleAiming.weight = 0f;
        ClimbingIK.weight = 0f;
    }

    public void GrappleRig()
    {
        if(!weapon.slotFull || !grapple.isGrappling || climb.isClimbing)
        {
            return;
        }
        WeaponAiming.weight = 0f;
        GrappleAiming.weight = 1f;
        ShootPose.weight = 1f;
        GrappleRifleIK.weight = 1f;

        BodyHead.weight = 0f;
        WeaponRestPose.weight = 0f;
        WeaponAiming.weight = 0f;
        WeaponOnBack.weight = 0f;
        ClimbingIK.weight = 0f;
    }

    public void DefaultRig()
    {
        if (weapon.slotFull || grapple.isGrappling || climb.isClimbing)
        {
            return;
        }

        BodyHead.weight = 1f;
        WeaponRestPose.weight = 0f;
        WeaponAiming.weight = 0f;
        WeaponOnBack.weight = 0f;
        ShootPose.weight = 0f;
        GrappleRifleIK.weight = 0f;
        ClimbingIK.weight = 0f;
        GrappleAiming.weight = 0f;
    }

    IEnumerator setAnimatorNormal()
    {
        yield return new WaitForSeconds(0.25f);
        anim.updateMode = AnimatorUpdateMode.Normal;
    }
}
