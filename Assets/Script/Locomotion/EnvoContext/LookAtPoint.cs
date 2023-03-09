using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPoint : MonoBehaviour
{
    public Camera cameraItself;
    // Update is called once per frame
    void Update()
    {
        Ray ray = cameraItself.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        this.transform.position = Vector3.Slerp(this.transform.position, ray.GetPoint(1000), 1f);
    }
}

