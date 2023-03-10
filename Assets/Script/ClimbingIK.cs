using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingIK : MonoBehaviour
{

    public Climbing climbing;
    public GameObject rightHand;
    public GameObject leftHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rightHand.transform.position = climbing.standingPoint + new Vector3(-0.2f, 0.1f, 0.2f);
        leftHand.transform.position = climbing.standingPoint + new Vector3(-0.2f, 0.1f, -0.2f);
    }
}
