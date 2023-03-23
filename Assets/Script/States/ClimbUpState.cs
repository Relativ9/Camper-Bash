using UnityEngine;

public class ClimbUpState : StateMachineBehaviour
{
    //Assigned in start/onStateEnter
    private Climbing climb;
    private PlayerMovement playerMove;
    private Animator anim;
    private Rigidbody playerRb;

    private Vector3 target;
    public float startTime, endTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) //Root motion animation entered
    {
        climb = FindAnyObjectByType<Climbing>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
        anim = FindAnyObjectByType<AnimatorStates>().GetComponent<Animator>();
        playerRb = playerMove.gameObject.GetComponent<Rigidbody>();
        target = climb.standingPoint;
        anim.applyRootMotion = true;
        climb.climbingUp = true;

    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) //player transform matched to animation clip at the end of the root motion animation clip
    {
        anim.MatchTarget(target, climb.transform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0), startTime, endTime);
        playerRb.position = target + new Vector3(0f, 0.94f, 0f);

        anim.transform.position = climb.transform.position - new Vector3(0f, 0.94f, 0f);
        climb.climbingUp = false;
        anim.applyRootMotion = false;
        playerMove.GetComponent<CapsuleCollider>().enabled = true;
    }
}
