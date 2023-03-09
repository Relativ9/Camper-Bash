using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] public Transform guntip; // depending on weapong guntip might defer right?
    [SerializeField] public Camera crossfire;
    [SerializeField] public VisualEffect muzzleEffect;
    [SerializeField] public int ammoRemaining;
    [SerializeField] public int maxAmmo = 20;
    [SerializeField] public bool isShooting;
    //[SerializeField] public NpcInteractionVisuals npcInteractionVisuals;
    //[SerializeField] public Npc npc;
    [SerializeField] public PlayerMovement playerMovement;
    //[SerializeField] public PauseMenuController pauseMenuController;
    [SerializeField] public Climbing climbing;

    public bool shotGunEquip;
    public bool pistolEquip;
    public bool rifleEquip;
    public bool cannonEquip;

    public int bulletCount = 5;

    public bool slotFull = false;

    public Transform weaponChild;

    public Vector3 crosshairs;

    //public Transform gunRotation;

    private GameObject bulletInstance;
    public float bulletspeed = 200f; // TODO: set this based on vector3.disctance .. close = lower value, far higher value



    void Start()
    {
        muzzleEffect = GetComponentInChildren<VisualEffect>();
        ammoRemaining = maxAmmo;
        //npcInteractionVisuals = FindObjectOfType<NpcInteractionVisuals>();
        //npc = FindObjectOfType<Npc>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        //pauseMenuController = FindObjectOfType<PauseMenuController>();
        climbing = FindObjectOfType<Climbing>();

    }
    void Update()
    {
        //aimWeapon();
        checkWeaponType();

        if (pistolEquip)
        {
            FirePistol();
        }

        if (shotGunEquip)
        {
            FireShotgun();
        }
    }

    private void FirePistol()
    {
        if (/*playerMovement.inChatwithNpc == false  && */Time.timeScale == 1 && (climbing.isPeaking || !climbing.isClimbing)) // TimeScale added so that we automatically know that the game is paused
        {
            Ray ray = crossfire.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(crossfire.transform.position, crossfire.transform.forward, out hit))
            {
                if (Input.GetMouseButtonDown(0) && ammoRemaining > 0)
                {
                    Debug.Log("FIRE!");
                    muzzleEffect.Play();
                    Debug.DrawLine(guntip.transform.position, hit.point, Color.red, 2f);
                    bulletInstance = Instantiate(bullet, guntip.position, crossfire.transform.rotation);
                    bulletInstance.transform.LookAt(hit.point);
                    bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletspeed;
                    ammoRemaining -= 1;
                }
                crosshairs = hit.point;
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && ammoRemaining > 0)
                {
                    Debug.Log("FIRE!");
                    muzzleEffect.Play();
                    Debug.DrawLine(guntip.transform.position, ray.GetPoint(100000), Color.red, 2f);
                    bulletInstance = Instantiate(bullet, guntip.position, guntip.rotation);
                    bulletInstance.transform.LookAt(ray.GetPoint(100000));
                    bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletspeed;
                    ammoRemaining -= 1;
                }
                Destroy(bulletInstance, 0.1f);
                crosshairs = ray.GetPoint(100000);
            }
        }
    }

    private void FireShotgun()
    {
        if (Time.timeScale != 0 && (climbing.isPeaking || !climbing.isClimbing)) // TimeScale added so that we automatically know that the game is paused
        {
            Ray ray = crossfire.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(crossfire.transform.position, crossfire.transform.forward, out hit))
            {
                if (Input.GetMouseButtonDown(0) && ammoRemaining > 0)
                {
                    for (var i = 0; i < bulletCount; i++)
                    {
                        var bulletRot = crossfire.transform.rotation;
                        bulletRot.x += Random.Range(-0.03f, 0.03f);
                        bulletRot.y += Random.Range(-0.03f, 0.03f);
                        //bulletRot.z += Random.Range(-0.03f, 0.03f);
                        bulletInstance = Instantiate(bullet, guntip.position + (new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f))), bulletRot);
                        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletspeed;
                        //bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transfrom.localEulerAngles(Vector3.forward) * bulletspeed;
                        //bulletInstance.GetComponent<Rigidbody>().velocity = Vector3.forward * bulletspeed;
                        Debug.Log("FIRE SHOTGUN!");
                    }
                    ammoRemaining -= 1;

                    muzzleEffect.Play();
                    Debug.DrawLine(guntip.transform.position, hit.point, Color.red, 2f);
                }
                crosshairs = hit.point;
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && ammoRemaining > 0)
                {

                    for (var i = 0; i < bulletCount; i++)
                    {
                        var bulletRot = guntip.transform.rotation;
                        bulletRot.x += Random.Range(-0.03f, 0.03f);
                        bulletRot.y += Random.Range(-0.03f, 0.03f);
                        //bulletRot.z += Random.Range(-0.03f, 0.03f);
                        bulletInstance = Instantiate(bullet, guntip.position + (new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f))), bulletRot);
                        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.forward * bulletspeed;
                        //bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transfrom.localEulerAngles(Vector3.forward) * bulletspeed;
                        //bulletInstance.GetComponent<Rigidbody>().velocity = Vector3.forward * bulletspeed;
                        Debug.Log("FIRE SHOTGUN!");
                    }
                    ammoRemaining -= 1;

                    muzzleEffect.Play();
                    Debug.DrawLine(guntip.transform.position, ray.GetPoint(100000), Color.red, 2f);
                }
                //Destroy(bulletInstance, 0.1f);
                crosshairs = ray.GetPoint(100000);
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

    //public void aimWeapon()
    //{
    //    var aimDir = this.transform.position - crosshairs;
    //    Quaternion targetDir = Quaternion.LookRotation(-aimDir);
    //    transform.LookAt(crosshairs);

    //}

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