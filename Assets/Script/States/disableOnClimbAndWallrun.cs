using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class disableOnClimbAndWallrun : MonoBehaviour
{

    public Climbing climbing;
    public WallRun wallrun;

    // Update is called once per frame
    void Update()
    {
        if (climbing.isClimbing || wallrun.isWallRunning)
        {
            //this.gameObject.SetActive(false);
            this.gameObject.GetComponent<MultiAimConstraint>().weight = 0;
            //this.gameObject.GetComponent<MultiAimConstraintData>().sourceObjects.SetWeight(0, 0);
            //this.gameObject.GetComponent<Rig>().weight = 0;
        }
        else
        {
            //this.gameObject.SetActive(true);
            this.gameObject.GetComponent<MultiAimConstraint>().weight = 1;
            //this.gameObject.GetComponent<MultiAimConstraintData>().sourceObjects.SetWeight(0, 1);
            //this.gameObject.GetComponent<Rig>().weight = 1;
        }
    }
}
