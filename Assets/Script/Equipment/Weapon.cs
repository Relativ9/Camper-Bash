using UnityEngine;
using UnityEngine.VFX;

public class Weapon : MonoBehaviour
{
    [Header("Manually assigned variables")]
    [SerializeField] public Transform gunTip;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Camera fpCam;
    [SerializeField] private VisualEffect muzzleEffect;


    [Header("Editable in inspector")]
    [SerializeField] private int maxAmmo = 20;
    [SerializeField] private int pelletCount = 5;
    [SerializeField] public float bulletspeed = 200f; // TODO: set this based on distance far = lower value, close higher value // would decrease bullet speed for stronger bullet drop effect, can also make it effect damage done. If we want a more realistic bullistics model this is ready

    [Header("Visible for debugging")]
    [SerializeField] private int ammoRemaining;
    //[SerializeField] private bool isShooting;  //enable in the future if we need certain actions to be inaccesible while shooting, mostly relevant for fully automatic weapons (which we don't have right now).
    [SerializeField] private bool shotGunEquip;
    [SerializeField] private bool pistolEquip;
    [SerializeField] private bool rifleEquip;
    [SerializeField] private bool cannonEquip;

    private PlayerMovement playerMovement;
    private Climbing climbing;
    private Transform weaponChild;
    private GameObject bulletInstance;
    private Vector3 aimRot;


    private Vector3 bulletCurrentPos;
    private Vector3 bulletCurrentVel;


    // intial position of bullet + velocity of the pulet * time.deltatime + 0.5 * gravity * time * time

    private GameObject bulletObj;
    public GameObject bulletPrefab;


    void Start()
    {
        //muzzleEffect = GetComponentInChildren<VisualEffect>();
        ammoRemaining = maxAmmo;
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        climbing = FindAnyObjectByType<Climbing>();

    }
    void LateUpdate()
    {
        checkWeaponType();

        if (pistolEquip)
        {
            //FirePistol();
        }

        if (shotGunEquip)
        {
            //FireShotgun();
        }

        if (rifleEquip)
        {

        }
    }

    //private void FireRifle()
    //{
    //    Ray aimRay = fpCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0);
    //    RaycastHit aimHit;
    //    Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out aimHit);

    //    Vector3 startPoint = fpCam.transform.position;
    //    Vector3 endPoint = aimRay.GetPoint(1f);
 
    //}


    private void FirePistol()
    {
        if (Time.timeScale >= 0 && (climbing.isPeaking || !climbing.isClimbing)) // TimeScale added so that we automatically know that the game is paused or not
        {
            Ray ray = fpCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out hit))
            {
                if (Input.GetMouseButtonDown(0) && ammoRemaining > 0 && !playerMovement.isRunning)
                {
                    Debug.Log("FIRE!");
                    muzzleEffect.Play();
                    Debug.DrawLine(gunTip.transform.position, hit.point, Color.red, 2f);
                    bulletInstance = Instantiate(projectilePrefab, gunTip.position, fpCam.transform.rotation);
                    bulletInstance.transform.LookAt(hit.point);
                    bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletspeed;
                    ammoRemaining -= 1;

                }
                //isShooting = false;
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && ammoRemaining > 0 && !playerMovement.isRunning)
                {
                    Debug.Log("FIRE!");
                    muzzleEffect.Play();
                    Debug.DrawLine(gunTip.transform.position, ray.GetPoint(100000), Color.red, 2f);
                    bulletInstance = Instantiate(projectilePrefab, gunTip.position, gunTip.rotation);
                    bulletInstance.transform.LookAt(ray.GetPoint(100000));
                    bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletspeed;
                    ammoRemaining -= 1;

                }
                //isShooting = false;
            }
        }
    }

    private void FireShotgun()
    {
        if (Time.timeScale != 0 && (climbing.isPeaking || !climbing.isClimbing)) // TimeScale added so that we automatically know that the game is paused
        {
            Ray ray = fpCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out hit))
            {
                if (Input.GetMouseButtonDown(0) && ammoRemaining > 0 && !playerMovement.isRunning)
                {
                    //isShooting = true;
                    for (var i = 0; i < pelletCount; i++)
                    {
                        Transform forwardLook = fpCam.transform;
                        bulletInstance = Instantiate(projectilePrefab, gunTip.position, Quaternion.Euler(Vector3.zero));
                        aimRot = hit.point - bulletInstance.gameObject.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                        var bulletRot = Quaternion.Euler(aimRot);
                        bulletInstance.transform.rotation = bulletRot;
                        bulletInstance.GetComponent<Rigidbody>().AddRelativeForce(aimRot * bulletspeed, ForceMode.Impulse);

                        Debug.Log("FIRE SHOTGUN!");
                    }
                    ammoRemaining -= 1;

                    muzzleEffect.Play();
                    Debug.DrawLine(gunTip.transform.position, hit.point, Color.red, 2f);
                }
                //isShooting = false;
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && ammoRemaining > 0 && !playerMovement.isRunning)
                {
                    //isShooting = true;
                    for (var i = 0; i < pelletCount; i++)
                    {
                        bulletInstance = Instantiate(projectilePrefab, gunTip.position, Quaternion.Euler(Vector3.zero));
                        aimRot = ray.GetPoint(10000f) - bulletInstance.gameObject.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                        var bulletRot = Quaternion.Euler(aimRot);
                        bulletInstance.transform.rotation = bulletRot;
                        bulletInstance.GetComponent<Rigidbody>().AddRelativeForce(aimRot * bulletspeed, ForceMode.Impulse);
                        Debug.Log("FIRE SHOTGUN!");
                    }
                    ammoRemaining -= 1;

                    muzzleEffect.Play();
                    Debug.DrawLine(gunTip.transform.position, ray.GetPoint(100000), Color.red, 2f);
                }
            }
        }
    }

    public void IncreaseAmmo(int number)
    {
        ammoRemaining += number;

        if (ammoRemaining > maxAmmo)
        {
            ammoRemaining = maxAmmo;
        }
    }

    public void checkWeaponType()
    {
        if (this.transform.childCount > 2)
        {
            weaponChild = this.transform.GetChild(2);
            if (weaponChild.tag == "shotgun")
            {
                shotGunEquip = true;
                pistolEquip = false;
            }

            if (weaponChild.tag == "pistol")
            {
                pistolEquip = true;
                shotGunEquip = false;
            }
        }
    }
}