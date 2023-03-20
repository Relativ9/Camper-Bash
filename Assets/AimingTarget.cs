using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingTarget : MonoBehaviour
{

    [SerializeField] private Camera fpCam;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = fpCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out hit))
        {
            transform.position = hit.point;
        } else
        {
            //transform.position = fpCam.transform.position + fpCam.transform.forward * 1000.0f;
            transform.position = ray.GetPoint(100f);
        }
 
    }
}
