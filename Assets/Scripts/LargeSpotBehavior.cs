using UnityEngine;

public class LargeSpotBehavior : MonoBehaviour
{

    [SerializeField] private Renderer spots;
    private float growthStrength;
    private float smallGrowthStrength;

    [SerializeField] private bool isSmallSpot;

    void Start()
    {
        spots = GetComponent<Renderer>();
        spots.material.SetFloat("_Seed", Random.Range(-10f, 10f));
        growthStrength = Random.Range(0.01f, 0.3f);
        smallGrowthStrength = Random.Range(0.01f, 0.03f);
        this.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    void FixedUpdate()
    {
        float maxIncrease = spots.material.GetFloat("_Progression") + Random.Range(0f, growthStrength);
        spots.material.SetFloat("_Progression", Mathf.Lerp(spots.material.GetFloat("_Progression"), maxIncrease, Time.deltaTime));
        spots.material.SetFloat("_Progression", Mathf.Clamp(spots.material.GetFloat("_Progression"), 0f, 0.7f));

        if(isSmallSpot)
        {
            var smallGrowth = Vector3.Lerp(this.transform.localScale, new Vector3(smallGrowthStrength, smallGrowthStrength, smallGrowthStrength), Time.deltaTime);
            this.transform.localScale = smallGrowth;
            return;
        }
        var largeGrowth = Vector3.Lerp(this.transform.localScale, new Vector3(growthStrength, growthStrength, growthStrength), Time.deltaTime);
        this.transform.localScale = largeGrowth;
    }
}
