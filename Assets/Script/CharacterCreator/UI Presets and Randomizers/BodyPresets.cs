using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyPresets : MonoBehaviour
{
    //bool buttonPressed;
    public int startingIndex = 0;

    //private Button button;
    public List<Slider> sliderList = new List<Slider>();
    public List<float> sliderValue = new List<float>();


    private void OnEnable()
    {
        if (startingIndex < sliderList.Count)
        {
            foreach (Slider slider in sliderList)
            {
                sliderList[startingIndex].value = sliderValue[startingIndex];
                startingIndex = startingIndex + 1;
            }
        }
    }

    private void OnDisable()
    {
        startingIndex = 0;
    }
}
