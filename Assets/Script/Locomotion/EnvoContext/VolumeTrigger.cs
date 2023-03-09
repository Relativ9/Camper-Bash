using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeTrigger : MonoBehaviour
{
    private bool swimLevelHit;
    [SerializeField] public bool surfaceSwimming;
    [SerializeField] public bool underwaterSwimming;
    [SerializeField] public bool inGas;

    private PlayerMovement playerMovement;
    private BreathingCheck breathingCheck;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        breathingCheck = FindObjectOfType<BreathingCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!breathingCheck.canBreathe)
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
        if (swimLevelHit && !playerMovement.playerOnGround)
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

    private void OnTriggerEnter(Collider other)
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
