using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingIK : MonoBehaviour
{

    public Climbing climbing;
    public GameObject rightHand;
    public GameObject leftHand;

    public Transform rightHandTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rightHand.transform.position = climbing.standingPoint + transform.right * 0.3f + transform.up * 0.1f + transform.forward * 0.3f;
        leftHand.transform.position = climbing.standingPoint + -transform.right * 0.3f + transform.up * 0.1f + transform.forward * 0.3f;
    }
}
