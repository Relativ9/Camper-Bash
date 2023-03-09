using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbUpState : StateMachineBehaviour
{

    public Climbing climbing;
    public PlayerMovement playerMove;
    public Animator playerAnim;
    public Rigidbody playerRb;
    public Transform player;
    public Vector3 target;
    public Transform ledgeTransform;

    public float startTime, endTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        climbing = FindObjectOfType<Climbing>();
        playerMove = FindObjectOfType<PlayerMovement>();
        playerAnim = FindObjectOfType<AnimatorReference>().GetComponent<Animator>();

        playerRb = playerMove.playerRigidbody;
        target = climbing.standingPoint;

        playerAnim.applyRootMotion = true;
        climbing.climbingUp = true;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    freyjaAnim.MatchTarget(target, climbing.transform.rotation, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), startTime, endTime);
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnim.MatchTarget(target, climbing.transform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0), startTime, endTime);
        playerRb.position = target + new Vector3(0f, 0.94f, 0f);

        playerAnim.transform.position = climbing.transform.position - new Vector3(0f, 0.94f, 0f);
        climbing.climbingUp = false;
        playerAnim.applyRootMotion = false;
        //playerMove.GetComponent<Rigidbody>().isKinematic = false;
        playerMove.GetComponent<CapsuleCollider>().enabled = true;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
