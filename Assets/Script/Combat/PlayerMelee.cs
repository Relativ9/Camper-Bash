using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Manually assigned variable")]
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform baseBallBat;
    [SerializeField] private Camera fpCam;

    [Header("Editable in inspector")]
    [SerializeField] public float hitStrength = 10f;

    [Header("Must remain publicly accessible")]
    public bool hasAttacked;
    private Animator anim;
    private BaseBallBatHit baseBat;



    // Start is called before the first frame update
    void Start()
    {
        anim = FindObjectOfType<AnimatorStates>().gameObject.GetComponent<Animator>();
        baseBat = FindObjectOfType<BaseBallBatHit>();
    }

    // Update is called once per frame
    public void Update()
    {
        baseBallBat.transform.position = leftHandTarget.position;
        baseBallBat.transform.rotation = leftHandTarget.rotation;

        if(Input.GetMouseButtonDown(2))
        {
            hasAttacked = true;
            anim.SetTrigger("Attack");
            if(baseBat.validTarget)
            {
                RaycastHit hit;
                if (Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out hit))
                {
                    GameObject objectHit = hit.transform.gameObject;
                    Vector3 forceDir = objectHit.transform.position - this.transform.position;
                    if (objectHit.gameObject.GetComponent<Rigidbody>() != null && objectHit.gameObject.GetComponent<Collider>())
                    {
                        objectHit.GetComponent<Rigidbody>().AddForce(forceDir * hitStrength);
                    }
                }
            }
        }
    }


    //public IEnumerator attackForce()
    //{
            
    //        RaycastHit hit;
    //        if (Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out hit))
    //        {
    //            GameObject objectHit = hit.transform.gameObject;
    //            Vector3 forceDir = objectHit.transform.position - this.transform.position;
    //        if (objectHit.gameObject.GetComponent<Rigidbody>() != null && objectHit.gameObject.GetComponent<Collider>())
    //            {
    //                objectHit.GetComponent<Rigidbody>().AddForce(forceDir * hitStrength);
    //            }
    //        }
    //    yield return new WaitForSeconds(0.1f);
    //}

}
