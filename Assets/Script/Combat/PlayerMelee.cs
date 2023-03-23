using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Manually assigned variable")]
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform weaponBase;
    [SerializeField] private Transform weaponTip;
    [SerializeField] private Transform aimTrans;
    [SerializeField] private Camera fpCam;

    [Header("Editable in inspector")]
    [SerializeField] private float hitStrength = 10f;
    [SerializeField] private float throwStrength = 0.1f;
    [SerializeField] private float spinStrength = 100f;

    [Header("Must remain publicly accessible")]
    public bool weaponThrown;
    public Collider weaponCol;

    [Header("Visible for debugging")]
    [SerializeField] private float weaponLength;

    private Vector3 forceDir;
    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = FindAnyObjectByType<AnimatorStates>().GetComponent<Animator>();
        weaponCol = FindAnyObjectByType<MeleeWeapon>().GetComponent<Collider>();
        weaponThrown = true;
    }

    // Update is called once per frame
    public void Update()
    {
        weaponLength = Vector3.Distance(weaponBase.position, weaponTip.position); //makes the range of the melee attack depend on the lenght of the weapon
        weaponBase.transform.position = leftHandTarget.position;
        weaponBase.transform.rotation = leftHandTarget.rotation;

        if(Input.GetMouseButtonDown(2) && !weaponThrown)
        {
            anim.SetTrigger("Attack");
        }

        if (Input.GetMouseButtonDown(3) && !weaponThrown)
        {
            anim.SetTrigger("Throw");
        }

        anim.SetBool("hasMelee", !weaponThrown);
    }

    public void AttackForce()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out hit, weaponLength + 0.75f)) //the lenght of the ray decides the range of melee attacks, it depends on the weapon lenght +0.75f (to compensate for the offset since ray origin is the camera).
        {
            GameObject objectHit = hit.transform.gameObject;
            Vector3 forceDir = objectHit.transform.position - fpCam.transform.position;
            if (objectHit.gameObject.GetComponent<Rigidbody>() != null && objectHit.gameObject.GetComponent<Collider>())
            {
                objectHit.GetComponent<Rigidbody>().AddForce(forceDir * hitStrength, ForceMode.Impulse);
                objectHit.GetComponent<Rigidbody>().AddForceAtPosition(forceDir, hit.point * hitStrength);
            }

            if(objectHit.GetComponent<EnemyHealth>() != null) //checks if the gameobject has Enemyhealth script on it, if it does apply damage
            {
                objectHit.GetComponent<EnemyHealth>().EnemyDamage(5f);
            }
        }
    }

    public void ThrowWeapon()
    {
        if(!weaponThrown)
        {
            weaponThrown = true;
            weaponCol.enabled = true;
            weaponCol.gameObject.transform.SetParent(null);
            weaponCol.gameObject.AddComponent<Rigidbody>();
            Rigidbody thrownRb = weaponCol.gameObject.GetComponent<Rigidbody>();
            thrownRb.angularDrag = 0f;
            thrownRb.drag = 1f;
            thrownRb.mass = 1f;
            thrownRb.interpolation = RigidbodyInterpolation.Interpolate;
            thrownRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Ray ray = fpCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit throwHit;
            if(Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out throwHit))
            {
                forceDir = throwHit.point - weaponCol.gameObject.transform.position;
                thrownRb.AddForce(forceDir.normalized * throwStrength, ForceMode.Impulse);
            }

            else
            {
                forceDir = aimTrans.position - weaponCol.gameObject.transform.position;
                thrownRb.AddForce(forceDir.normalized * throwStrength, ForceMode.Impulse);
            }
            thrownRb.AddRelativeTorque(Vector3.right * spinStrength, ForceMode.Impulse);
        }
    }

}
