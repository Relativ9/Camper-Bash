using UnityEngine;

public class ClimbUpState : StateMachineBehaviour
{
    //Assigned in start/onStateEnter
    private Climbing climb;
    private PlayerMovement playerMove;
    private Animator playerAnim;
    private Rigidbody playerRb;

    private Vector3 target;
    public float startTime, endTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        climb = FindObjectOfType<Climbing>();
        playerMove = FindObjectOfType<PlayerMovement>();
        playerAnim = FindObjectOfType<AnimatorStates>().GetComponent<Animator>();
        playerRb = playerMove.gameObject.GetComponent<Rigidbody>();
        target = climb.standingPoint;
        playerAnim.applyRootMotion = true;
        climb.climbingUp = true;

    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerAnim.MatchTarget(target, climb.transform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0), startTime, endTime);
        playerRb.position = target + new Vector3(0f, 0.94f, 0f);

        playerAnim.transform.position = climb.transform.position - new Vector3(0f, 0.94f, 0f);
        climb.climbingUp = false;
        playerAnim.applyRootMotion = false;
        playerMove.GetComponent<CapsuleCollider>().enabled = true;
    }
}
