using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseUIScript : MonoBehaviour
{
    //public SkinnedMeshRenderer bodyMesh;
    public Renderer Skin;
    public Renderer EyesLeftRend;
    public Renderer EyesRightRend;
    public GameObject PlayerParent;


    public Color32 african, european, asian, eastern, northern, currentSkin;


    public bool randomizeBlend;
    public bool africanBool, europeanBool, asianBool, easternBool, northernBool;
    public bool blueEyesBool, greenEyesBool, brownEyesBool, grayEyesBool, yellowEyesBool;
    //public Button africaButton;

    private string complexionValue;
    public GameObject height;
    private InputField heightInput;

    public GameObject BodySliders, HeadSliders, NeckSliders, Brow, Eyes, Cheeks, Nose, Jaw, Mouth, UpperLip, LowerLip, Chin, Ears;
    public TMP_Dropdown complexionDrop;

    public Material Default, Beardless, Freckles1, Freckles1Beardless, Freckles2, Freckles2Beardless, Freckles3, Freckles3Beardless, Freckles4, Freckles4Beardless, Scar, ScarBeardless, Pale, PaleBeardless;
    public Material currentEyes, blueEyes, greenEyes, brownEyes, grayEyes, yellowEyes;


    public void Start()
    {
        heightInput = height.GetComponent<InputField>();
        Skin.material = Default;
        StartCoroutine(InitializeSliders());
        currentEyes = blueEyes;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        DropdownSelection();
        ComplexionSelection();
        Skin.material.SetColor("_BaseColor", currentSkin);
        EyesLeftRend.material = currentEyes;
        EyesRightRend.material = currentEyes;
    }

    public void ComplexionSelection()
    {
        if (complexionValue == "Default")
        {
            Skin.material = Default;
        }
        if (complexionValue == "Shaved")
        {
            Skin.material = Beardless;
        }
        if (complexionValue == "Spots 1")
        {
            Skin.material = Freckles1;
        }
        if (complexionValue == "Spots 1 Shaved")
        {
            Skin.material = Freckles1Beardless;
        }
        if (complexionValue == "Spots 2")
        {
            Skin.material = Freckles2;
        }
        if (complexionValue == "Spots 2 Shaved")
        {
            Skin.material = Freckles2Beardless;
        }
        if (complexionValue == "Spots 3")
        {
            Skin.material = Freckles3;
        }
        if (complexionValue == "Spots 3 Shaved")
        {
            Skin.material = Freckles3Beardless;
        }
        if (complexionValue == "Spots 4")
        {
            Skin.material = Freckles4;
        }
        if (complexionValue == "Spots 4 Shaved")
        {
            Skin.material = Freckles4Beardless;
        }
        if (complexionValue == "Scar")
        {
            Skin.material = Scar;
        }
        if (complexionValue == "Scar Shaved")
        {
            Skin.material = ScarBeardless;
        }
        if (complexionValue == "Pale")
        {
            Skin.material = Pale;
        }
        if (complexionValue == "Pale Shaved")
        {
            Skin.material = PaleBeardless;
        }
    }


    public void DropdownSelection()
    {
        int complexionOption = complexionDrop.value;

        List<TMP_Dropdown.OptionData> menuOptions = complexionDrop.options;

        complexionValue = menuOptions[complexionOption].text;
    }
    public void heightScale(InputField heightFeild)
    {
        //var userInput = new InputField.SubmitEvent();
        //userInput.AddListener(SubmitHeight);
        //heightInput.onEndEdit = userInput;

        heightInput.onEndEdit.AddListener(SubmitHeight);
    }

    private void SubmitHeight(string height)
    {
        Debug.Log(height);
    }

    public void BlueEyes(Button blueEyesButton)
    {

        blueEyesBool = true;
        if (blueEyesBool && !greenEyesBool && !brownEyesBool && !grayEyesBool && !yellowEyesBool)
        {
            currentEyes = blueEyes;
            blueEyesBool = false;
        }
    }

    public void BrownEyes(Button brownEyesButton)
    {
        brownEyesBool = true;
        if (!blueEyesBool && !greenEyesBool && brownEyesBool && !grayEyesBool && !yellowEyesBool)
        {
            currentEyes = brownEyes;
            brownEyesBool = false;
        }
    }

    public void GrayEyes(Button grayEyesButton)
    {

        grayEyesBool = true;
        if (!blueEyesBool && !greenEyesBool && !brownEyesBool && grayEyesBool && !yellowEyesBool)
        {
            currentEyes = grayEyes;
            grayEyesBool = false;
        }
    }

    public void YellowEyes(Button yellowEyesButton)
    {
        yellowEyesBool = true;
        if (!blueEyesBool && !greenEyesBool && !brownEyesBool && !grayEyesBool && yellowEyesBool)
        {
            currentEyes = yellowEyes;
            yellowEyesBool = false;
        }
    }

    public void GreenEyes(Button greenEyesButton)
    {
        greenEyesBool = true;
        if (!blueEyesBool && greenEyesBool && !brownEyesBool && !grayEyesBool && !yellowEyesBool)
        {
            currentEyes = greenEyes;
            greenEyesBool = false;
        }
    }
    public void AfricanSkin(Button africaButton)
    {
        africanBool = true;
        if (africanBool && !europeanBool && !asianBool && !northernBool && !easternBool)
        {
            currentSkin = african;
            africanBool = false;
        }
    }

    public void EuropeanSkin(Button europeButton)
    {
        europeanBool = true;
        if (!africanBool && europeanBool && !asianBool && !northernBool && !easternBool)
        {
            currentSkin = european;
            europeanBool = false;
        }
    }

    public void AsianSkin(Button asiaButton)
    {
        asianBool = true;
        if (!africanBool && !europeanBool && asianBool && !northernBool && !easternBool)
        {
            currentSkin = asian;
            asianBool = false;
        }
    }

    public void EasternSkin(Button easternButton)
    {
        easternBool = true;
        if (!africanBool && !europeanBool && !asianBool && !northernBool && easternBool)
        {
            currentSkin = eastern;
            easternBool = false;
        }
    }

    public void NorthernSkin(Button northernButton)
    {
        northernBool = true;
        if (!africanBool && !europeanBool && !asianBool && !easternBool && northernBool)
        {
            currentSkin = northern;
            northernBool = false;
        }
    }

    IEnumerator InitializeSliders()
    {
        BodySliders.SetActive(true); HeadSliders.SetActive(true); NeckSliders.SetActive(true); Brow.SetActive(true); Eyes.SetActive(true); Cheeks.SetActive(true); Nose.SetActive(true); Jaw.SetActive(true); Mouth.SetActive(true); UpperLip.SetActive(true); LowerLip.SetActive(true); Chin.SetActive(true); Ears.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        BodySliders.SetActive(false); HeadSliders.SetActive(false); NeckSliders.SetActive(false); Brow.SetActive(false); Eyes.SetActive(false); Cheeks.SetActive(false); Nose.SetActive(false); Jaw.SetActive(false); Mouth.SetActive(false); UpperLip.SetActive(false); LowerLip.SetActive(false); Chin.SetActive(false); Ears.SetActive(false);
    }
}
