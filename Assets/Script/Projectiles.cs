using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class Projectiles : MonoBehaviour
{

    class Projectile 
    {
        public float time;
        public Vector3 initPos;
        public Vector3 initVel;
        public TrailRenderer tail;
    }

    [Header("Manually assigned variables")]
    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform fpCamTrans;
    [SerializeField] private Transform aimTrans;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;



    public ParticleSystem[] muzzleFlash;

    [Header("Editable in inspector")]
    [SerializeField] private int maxAmmo = 20;
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float projectileSpeed = 100f;
    [SerializeField] private float projectileDrop = 9.81f;
    [SerializeField] private float maxLifeTime = 3f;
    [SerializeField] private float impactForce;
    [SerializeField] private float projectileDmg = 5f;


    [Header("Visible for debugging")]
    [SerializeField] public bool hasWeapon;
    [SerializeField] public bool hasFired;
    [SerializeField] public int currentAmmo;

    //Assigned in start
    private WeaponPickup weaponPickup;

    List<Projectile> projectilesList = new List<Projectile>();

    Ray ray;
    RaycastHit hitInfo;

    public void Start()
    {
        muzzleFlash = this.gameObject.GetComponentsInChildren<ParticleSystem>();
        weaponPickup = FindAnyObjectByType<WeaponPickup>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasFired && weaponPickup.hasWeapon && currentAmmo > 0)
        {
            Fire();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopFiering();
        }

        UpdateAirTime(Time.deltaTime);
    }

    Vector3 GetPos(Projectile projectile)
    {
        Vector3 gravity = Vector3.down * projectileDrop;
        return projectile.initPos + projectile.initVel * projectile.time + 0.5f * gravity * projectile.time * projectile.time;
    }

    Projectile CreateProjectile(Vector3 pos, Vector3 vel)
    {
        Projectile projectile = new Projectile();
        projectile.initPos = pos;
        projectile.initVel = vel;
        projectile.time = 0f;
        projectile.tail = Instantiate(tracerEffect, pos, Quaternion.identity);
        projectile.tail.AddPosition(pos);
        return projectile;

    }

    
    public void UpdateAirTime(float deltaTime)
    {
        ProjectileSimulation(deltaTime);
        DestroyProjectiles();
    }

    public void ProjectileSimulation(float deltaTime)
    {
        projectilesList.ForEach(projectile =>
        {
            Vector3 currentPos = GetPos(projectile);
            projectile.time += deltaTime;
            Vector3 nextPos = GetPos(projectile);
            RaycastStep(currentPos, nextPos, projectile);
        });
    }

    void RaycastStep(Vector3 start, Vector3 end, Projectile projectilesList)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            hitEffect.transform.parent = hitInfo.transform;
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);


            projectilesList.tail.transform.position = hitInfo.point;
            projectilesList.time = maxLifeTime;

            if(hitInfo.collider.gameObject.GetComponent<Rigidbody>() != null) 
            {
                Vector3 forceDir = hitInfo.point - transform.position;
                var targetRb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                targetRb.AddExplosionForce(impactForce, hitInfo.point, 0.2f, 1f, ForceMode.Impulse);
            }

            if(hitInfo.transform.gameObject.GetComponent<EnemyHealth>() != null)
            {
                EnemyHealth enemyHealth = hitInfo.transform.gameObject.GetComponent<EnemyHealth>();
                enemyHealth.EnemyDamage(projectileDmg);
            }

            if(hitInfo.transform.gameObject.GetComponent<PlayerHealth>() != null)
            {
                PlayerHealth playerHealth = hitInfo.transform.gameObject.GetComponent<PlayerHealth>();
                playerHealth.PlayerDamage(projectileDmg);
            }

        } else
        {
            projectilesList.tail.transform.position = end;
        }
    }

    void DestroyProjectiles()
    {
        projectilesList.RemoveAll(projectile => projectile.time >= maxLifeTime);
    }
    public void Fire()
    {
        hasFired = true;
        foreach(ParticleSystem p in muzzleFlash)
        {
            p.Emit(1);
        }

        Vector3 velocity = (aimTrans.position - gunTip.position).normalized * projectileSpeed;
        var projectile = CreateProjectile(gunTip.position, velocity);
        projectilesList.Add(projectile);

    }

    public void StopFiering()
    {
        hasFired = false;
    }

    public void IncreaseAmmo(int number)
    {
        currentAmmo += number;

        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
    }
}
