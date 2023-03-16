using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeTrigger : MonoBehaviour
{
    

    [Header("Must remain publicly accessible")]
    public bool surfaceSwimming;
    public bool underwaterSwimming;
    public bool inGas;

    //Assgined in start
    private PlayerMovement playerMovement;
    private BreathingCheck breathingCheck;

    private bool swimLevelHit;


    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        breathingCheck = FindAnyObjectByType<BreathingCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!breathingCheck.canBreathe) //gas volume, removes stamina regen and deals damage over time
        {
            inGas = true;
        }
        else
        {
            inGas = false;
        }

        Swimming();
    }

    public void Swimming()
    {
        if (swimLevelHit && !playerMovement.isGrounded)
        {

            if (breathingCheck.canBreathe)
            {
                surfaceSwimming = true;
                underwaterSwimming = false;
            }
            else if (!breathingCheck.canBreathe)
            {
                surfaceSwimming = false;
                underwaterSwimming = true;
            }

        }
        else if (!swimLevelHit)
        {
            surfaceSwimming = false;
            underwaterSwimming = false;
        }
        else if (swimLevelHit && breathingCheck.canBreathe)
        {
            if (breathingCheck.canBreathe)
            {
                surfaceSwimming = true;
                underwaterSwimming = false;
            }
        }
        else
        {
            surfaceSwimming = false;
            underwaterSwimming = true;
        }
    }

    private void OnTriggerEnter(Collider other) //checks if player is submerged up to a certain level in a liquid (or fire/lava), move this game object higher to adjust what "swimmable" level should be defined as.
    {
        //Picking up Items for Inventory

        if (other.gameObject.tag == "Liquid" || other.gameObject.tag == "Fire")
        {
            swimLevelHit = true;
        }
        else
        {
            swimLevelHit = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Liquid" || other.gameObject.tag == "Fire")
        {
            swimLevelHit = false;
        }
    }
}
