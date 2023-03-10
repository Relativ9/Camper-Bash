using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmoothWeaponAnchor : MonoBehaviour
{

    public Transform rightShoulder;
    public float multiplier = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = Vector3.Slerp(this.gameObject.transform.position, rightShoulder.position, multiplier);
    }
}
