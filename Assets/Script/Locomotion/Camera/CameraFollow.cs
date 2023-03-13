using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Manually assigned variables")]
    [SerializeField] private Transform camFollowTrans;
    [SerializeField] private Transform deathCamPos;
    [SerializeField] private Transform fpCamTrans;

    [Header("Editable in inspector")]
    [SerializeField] float resetSpeed = 15f;
    [SerializeField] float multiplier = 10f;
    [SerializeField] float deathCamSmooth = 0.1f;

    private Vector3 startPos;
    private PlayerHealth playHealth;

    // Start is called before the first frame update
    void Start()
    {
        playHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Awake()
    {
        startPos = fpCamTrans.localPosition;
    }

    void Update()
    {
        if(!playHealth.isAlive)
        {           
            transform.position = Vector3.Slerp(transform.position, deathCamPos.position, deathCamSmooth * Time.deltaTime);
            Time.timeScale = 0.5f;
        } else
        {
            Time.timeScale = 1f;
            transform.position = Vector3.Slerp(transform.position, camFollowTrans.position, multiplier);
            restartPos();
        }
    }
    private void restartPos()
    {
        if (fpCamTrans.localPosition == startPos) return;
        fpCamTrans.localPosition = Vector3.Lerp(fpCamTrans.localPosition, startPos, resetSpeed * Time.deltaTime);
    }
}
