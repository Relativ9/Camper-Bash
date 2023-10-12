using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderRandomGrowth : MonoBehaviour
{

    public Renderer spots;
    [SerializeField] private float growthStrength;

    // Start is called before the first frame update
    void Start()
    {
        spots = GetComponent<Renderer>();
        spots.material.SetFloat("_Seed", Random.Range(-10f, 10f));
        growthStrength = Random.Range(0.01f, 0.3f);
        this.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float maxIncrease = spots.material.GetFloat("_Progression") + Random.Range(0f, growthStrength);
        spots.material.SetFloat("_Progression", Mathf.Lerp(spots.material.GetFloat("_Progression"), maxIncrease, Time.deltaTime));
        spots.material.SetFloat("_Progression", Mathf.Clamp(spots.material.GetFloat("_Progression"), 0f, 0.7f));

        var scaleGrowth = Vector3.Lerp(this.transform.localScale, new Vector3(growthStrength, growthStrength, growthStrength), Time.deltaTime);
        this.transform.localScale = scaleGrowth;
    }

}
