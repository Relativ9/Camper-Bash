using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FaceSaveLoadRandomReset : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] string objectName;
    [SerializeField] FacePresetManager fPM;
    private float sliderValue1, sliderValue2, sliderValue3, sliderValue4, sliderValue5, sliderValue6, sliderValue7, sliderValue8;



    // Start is called before the first frame update
    void Awake()
    {
        slider = GetComponent<Slider>();
        fPM = FindObjectOfType<FacePresetManager>();
        objectName = gameObject.name;
    }

    public void FacePresetSave()
    {
        if (fPM.Face1)
        {
            sliderValue1 = slider.value;
            PlayerPrefs.SetFloat(objectName + "Face1", sliderValue1);
        }

        if (fPM.Face2)
        {
            sliderValue2 = slider.value;
            PlayerPrefs.SetFloat(objectName + "Face2", sliderValue2);
        }

        if (fPM.Face3)
        {
            sliderValue3 = slider.value;
            PlayerPrefs.SetFloat(objectName + "Face3", sliderValue3);
        }

        if (fPM.Face4)
        {
            sliderValue4 = slider.value;
            PlayerPrefs.SetFloat(objectName + "Face4", sliderValue4);
        }

        if (fPM.Face5)
        {
            sliderValue5 = slider.value;
            PlayerPrefs.SetFloat(objectName + "Face5", sliderValue5);
        }

        if (fPM.Face6)
        {
            sliderValue6 = slider.value;
            PlayerPrefs.SetFloat(objectName + "Face6", sliderValue6);
        }

        if (fPM.Face7)
        {
            sliderValue7 = slider.value;
            PlayerPrefs.SetFloat(objectName + "Face7", sliderValue7);
        }

        if (fPM.Face8)
        {
            sliderValue8 = slider.value;
            PlayerPrefs.SetFloat(objectName + "Face8", sliderValue8);
        }
    }

    public void FacePresetLoad()
    {
        if (fPM.Face1)
        {
            sliderValue1 = PlayerPrefs.GetFloat(objectName + "Face1");
            slider.value = sliderValue1;
        }
        if (fPM.Face2)
        {
            sliderValue2 = PlayerPrefs.GetFloat(objectName + "Face2");
            slider.value = sliderValue2;
        }
        if (fPM.Face3)
        {
            sliderValue3 = PlayerPrefs.GetFloat(objectName + "Face3");
            slider.value = sliderValue3;
        }
        if (fPM.Face4)
        {
            sliderValue4 = PlayerPrefs.GetFloat(objectName + "Face4");
            slider.value = sliderValue4;
        }
        if (fPM.Face5)
        {
            sliderValue5 = PlayerPrefs.GetFloat(objectName + "Face5");
            slider.value = sliderValue5;
        }
        if (fPM.Face6)
        {
            sliderValue6 = PlayerPrefs.GetFloat(objectName + "Face6");
            slider.value = sliderValue6;
        }
        if (fPM.Face7)
        {
            sliderValue7 = PlayerPrefs.GetFloat(objectName + "Face7");
            slider.value = sliderValue7;
        }
        if (fPM.Face8)
        {
            sliderValue8 = PlayerPrefs.GetFloat(objectName + "Face8");
            slider.value = sliderValue8;
        }
    }

    public void presetRandom()
    {
        slider.value = Random.Range(slider.minValue + Random.Range(0, 50), slider.maxValue - Random.Range(0, 50));
    }

    public void sliderReset()
    {
        slider.value = 0;
    }
}
