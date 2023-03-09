using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class disableOnClimbAndWallrun : MonoBehaviour
{

    public Climbing climbing;
    public WallRun wallrun;
    public PlayerMovement playerMove;
    public RigBuilder rig;
    //public GameObject BodyAimRig;
    //public GameObject WeaponRestRig;
    //public GameObject 

    //public List<RigLayer> riglayers = new List<RigLayer>();

    private void Start()
    {
        rig = this.gameObject.GetComponent<RigBuilder>();
        //riglayers.Add(this.gameObject.GetComponent<RigBuilder>().)
    }
    // Update is called once per frame
    void Update()
    {
        if (climbing.isClimbing || wallrun.isWallRunning)
        {

            rig.layers[0].active = false;
            rig.layers[1].active = false;
            rig.layers[2].active = false;
            rig.layers[3].active = true;
            rig.layers[4].active = false;

        }
        else if (playerMove.isRunning)
        {

            rig.layers[0].active = true;
            rig.layers[1].active = true;
            rig.layers[2].active = false;
            rig.layers[3].active = false;
        } else
        {
            rig.layers[0].active = true;
            rig.layers[1].active = false;
            rig.layers[2].active = true;
            rig.layers[3].active = false;
            rig.layers[4].active = true;
        }
    }

}
