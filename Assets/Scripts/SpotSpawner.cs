using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class SpotSpawner : MonoBehaviour
{
    [SerializeField] private Camera eyeCam;
    [SerializeField] private GameObject spotPrefab;
    [SerializeField] private Transform spawnSurfaceTarget;

    private Vector3 targetPoint;
    [SerializeField] private float spawnInterval = 2f;
    private Collider targetCollider;
    private bool hasSpawned;


    void Start()
    {
        targetCollider = spawnSurfaceTarget.GetComponent<Collider>();
    }

    void Update()
    {
        if(!hasSpawned)
        {
            GetSpawnPoint();
            StartCoroutine("spawnThenWait");
        }
    }

    public void GetSpawnPoint()
    {
        Bounds b = targetCollider.bounds;

        targetPoint = new Vector3(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y), Random.Range(b.min.z, b.max.z) + 10f);
    }

    IEnumerator spawnThenWait()
    {
        hasSpawned = true;
        yield return new WaitForSeconds(spawnInterval);

        RaycastHit hit;
        if(Physics.Raycast(eyeCam.transform.position, targetPoint, out hit))
        {
            var decalInstance = Instantiate(spotPrefab);
            decalInstance.transform.position = hit.point;
            decalInstance.transform.Rotate(new Vector3(0f, Random.Range(0f, 360f), 0f));
            decalInstance.transform.parent = spawnSurfaceTarget;
        }
        hasSpawned = false;
    }

}
