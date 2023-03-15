using UnityEngine;

public class BaseBallBatHit : MonoBehaviour
{
    public bool validTarget;
    private GameObject objectHit;
    private PlayerMelee playMelee;

    private void Start()
    {
        playMelee = FindObjectOfType<PlayerMelee>();
    }

    private void OnTriggerEnter(Collider other)
    {
        objectHit = other.gameObject;
        if (objectHit.GetComponent<Collider>() != null && objectHit.GetComponent<Rigidbody>() != null && playMelee.hasAttacked) 
        {
            validTarget = true;
            Debug.Log("Collider is valid!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        validTarget = false;
    }
}
