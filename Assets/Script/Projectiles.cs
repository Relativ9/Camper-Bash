using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectiles : MonoBehaviour
{

    public bool hasFired;

    public ParticleSystem[] muzzleFlash;

    public Transform gunTip;
    public Transform fpCamTrans;
    public Transform aimTrans;

    Ray ray;
    RaycastHit hitInfo;

    public void Start()
    {
        muzzleFlash = this.gameObject.GetComponentsInChildren<ParticleSystem>();
    }



    public void LateUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartFiering();
        }

        if(Input.GetMouseButtonUp(0))
        {
            StopFiering();
        }
    }
    public void StartFiering()
    {
        hasFired = true;
        foreach(ParticleSystem p in muzzleFlash)
        {
            p.Emit(1);
        }

        ray.origin = gunTip.position;
        ray.direction = aimTrans.position - ray.origin;
        Vector3 rayDir = aimTrans.position - gunTip.position;

        Debug.DrawRay(gunTip.position, rayDir, Color.blue, 20f);
    }

    public void StopFiering()
    {
        hasFired = false;
    }
}
