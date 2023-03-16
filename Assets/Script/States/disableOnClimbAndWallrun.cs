using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class disableOnClimbAndWallrun : MonoBehaviour
{

    public Climbing climbing;
    public WallRun wallrun;
    public PlayerMovement playerMove;
    public RigBuilder rigLayers;
    public Weapon weapon;
    public Rig BodyHead;
    public Rig WeaponRest;
    public Rig WeaponAiming;
    public Rig ShootPose;
    public Rig WeaponOnBack;
    public Rig GrappleRifleIK;
    public Rig ClimbingIK;
    public float multiplier = 10;

    public Animator animator;

    public void Start()
    {
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
        if (climbing.isClimbing)
        {
            animator.updateMode = AnimatorUpdateMode.Fixed;
        }
        else
        {
            StartCoroutine(setAnimatorNormal());
        }

        if (climbing.isClimbing)
        {
            BodyHead.weight = 0f;
            WeaponRest.weight = 0f * Time.deltaTime * multiplier;
            ShootPose.weight = 0f * Time.deltaTime * multiplier;
            WeaponOnBack.weight = 1f;
            WeaponAiming.weight = 0f;
            ClimbingIK.weight = 1f;
            GrappleRifleIK.weight = 0f;

        } else if (wallrun.isWallRunning && weapon.slotFull)
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
        animator.updateMode = AnimatorUpdateMode.Normal;
    }
}
