using UnityEngine;

public class AimingTarget : MonoBehaviour
{

    [SerializeField] private Camera fpCam;
    void Update()
    {
        RaycastHit hit;
        Ray ray = fpCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out hit)) //Puts the game object wherever in the world the player is aiming, useful for drawing things like lines and setting impact vfx ( alwayss moving the same object instead of instantiating a new one)
        {
            transform.position = hit.point;
        } else
        {
            transform.position = ray.GetPoint(100f);
        }
 
    }
}
