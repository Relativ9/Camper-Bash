using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingCheck : MonoBehaviour
{

    [SerializeField] public bool canBreathe;
    // Start is called before the first frame update
    void Start()
    {
        canBreathe = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Liquid" || other.gameObject.tag == "Gas" || other.gameObject.tag == "Fire")
        {
            canBreathe = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Liquid" || other.gameObject.tag == "Gas" || other.gameObject.tag == "Fire")
        {
            canBreathe = true;
        }
    }
}
