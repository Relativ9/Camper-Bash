using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponFire : MonoBehaviour
{

    class Projectile
    {
        public float time;
        public Vector3 initialPos;
        public Vector3 intitialVel;
        public TrailRenderer tracer;
    }

    public TrailRenderer projectileTrail;

    public float proDrop;
    public float proSpeed;

    public Transform raycastStart;
    public Transform raycastEnd;

    Ray ray;
    RaycastHit hitInfo;
    float timePassed;
    List<Projectile> projectiles = new List<Projectile>();

    float maxLifetime = 3f;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            fireProjectile();
        }
    }

    Vector3 GetPosition(Projectile projectile)
    {
        Vector3 gravity = Vector3.down * proDrop;
        return projectile.initialPos + projectile.intitialVel * projectile.time + 0.5f * gravity * projectile.time * projectile.time;
    }

    Projectile CreateProjectile(Vector3 pos, Vector3 vel)
    {
        Projectile projectile =  new Projectile();
        projectile.initialPos = pos;
        projectile.intitialVel = vel;
        projectile.time = 0f;
        projectile.tracer = Instantiate(projectileTrail, pos, Quaternion.identity);
        projectile.tracer.AddPosition(pos);
        return projectile;
    }

    public void UpdateProjectilePos(float deltaTime)
    {
        SimulateProjectile(deltaTime);
        DestroyProjectile();
    }

    void SimulateProjectile(float deltaTime)
    {
        projectiles.ForEach(projectile =>
        {
            Vector3 firstPos = GetPosition(projectile);
            projectile.time += deltaTime;
            Vector3 secondPos = GetPosition(projectile);
            RaycastSegment(firstPos, secondPos, projectile);
        });
    }

    void RaycastSegment(Vector3 start, Vector3 end, Projectile projectile)
    {
        Vector3 direction = end - start;
        float distance = (direction).magnitude;
        ray.origin = start;
        ray.direction = direction;

        if(Physics.Raycast(ray, out hitInfo, distance))
        {
            projectile.tracer.transform.position = hitInfo.point;

        }
    }

    private void fireProjectile()
    {
        Vector3 velocity = (raycastEnd.position - raycastStart.position).normalized * proSpeed;
        var projectile = CreateProjectile(raycastStart.position, velocity);
        projectiles.Add(projectile);

    }

    public void LateUpdate()
    {
        UpdateProjectilePos(Time.deltaTime);
    }

    void DestroyProjectile()
    {
        projectiles.RemoveAll(projectiles => projectiles.time >= maxLifetime);
    }
}
