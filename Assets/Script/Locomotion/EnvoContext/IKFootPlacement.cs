using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{

    public Animator anim;
    public PlayerMovement playerMovement;

    [Range(0, 1f)]
    public float groundDist;
    public Vector3 leftFoot;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));

            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));


            //LeftFoot Distance and Angle IK Placement on Ground
            RaycastHit hit;
            Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, groundDist + 2f))
            {
                if (playerMovement.playerOnGround)
                {
                    Vector3 leftFootPos = hit.point;

                    leftFootPos.y += groundDist;

                    anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos);
                    anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(anim.GetIKRotation(AvatarIKGoal.LeftFoot) * Vector3.forward, hit.normal));

                }
            }

            //Rightfoot Distance and Angle IK Placement on Ground
            ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, groundDist + 2f))
            {
                if (playerMovement.playerOnGround)
                {
                    Vector3 rightFootPos = hit.point;

                    rightFootPos.y += groundDist;

                    anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos);
                    anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(anim.GetIKRotation(AvatarIKGoal.RightFoot) * Vector3.forward, hit.normal));

                }
            }
        }
    }
}