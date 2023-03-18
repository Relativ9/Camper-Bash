using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRagdollActivator : MonoBehaviour
{
    [Header("Manually assigned variables")]
    [SerializeField] private GameObject enemyRig;
    [SerializeField] private GameObject enemyParent;

    //Assigned in start
    private EnemyHealth enemyHealth;
    private Collider enemyCol;
    private Animator enemyAnim;

    private Collider[] rigCols;
    private Rigidbody[] rigRbs;
    private Vector3 lastVel;
    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = FindAnyObjectByType<EnemyHealth>();
        enemyAnim = GetComponent<Animator>();
        enemyCol = enemyParent.GetComponent<Collider>();

        RagdollComponents();
        RagdollOff();
    }

    // Update is called once per frame
    void Update() 
    {
        if (!enemyHealth.isAlive)
        {
            RagdollOn();
        }
        else
        {
            RagdollOff();
        }
    }

    void RagdollComponents()
    {
        rigCols = enemyRig.GetComponentsInChildren<Collider>();
        rigRbs = enemyRig.GetComponentsInChildren<Rigidbody>();
    }

    void RagdollOn()
    {
        enemyAnim.enabled = false;

        foreach(Collider col in rigCols)
        {
            col.enabled = true;
        }
        
        foreach(Rigidbody rbs in rigRbs)
        {
            rbs.isKinematic = false;
            //rbs.velocity = lastVel;
            rbs.interpolation = RigidbodyInterpolation.Interpolate;
            rbs.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        enemyCol.enabled = false;
        enemyParent.GetComponent<Rigidbody>().isKinematic = true;
    }

    void RagdollOff()
    {
        foreach(Collider col in rigCols)
        {
            col.enabled = false;
        }

        foreach(Rigidbody rbs in rigRbs)
        {
            rbs.isKinematic = true;
        }

        enemyCol.enabled = true;
        enemyParent.GetComponent<Rigidbody>().isKinematic = false;
        enemyAnim.enabled = true;
        //lastVel = enemyParent.GetComponent<Rigidbody>().velocity;
    }
}
