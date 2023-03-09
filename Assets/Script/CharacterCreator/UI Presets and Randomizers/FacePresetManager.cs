using UnityEngine;
using TMPro;


public class FacePresetManager : MonoBehaviour
{

    public TMP_Dropdown faceSelect;

    public bool Face1, Face2, Face3, Face4, Face5, Face6, Face7, Face8;

    // Start is called before the first frame update
    void Start()
    {
        faceSelect = this.gameObject.GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        if (faceSelect.value == 0)
        {
            Face1 = true;
        }
        else
        {
            Face1 = false;
        }
        if (faceSelect.value == 1)
        {
            Face2 = true;
        }
        else
        {
            Face2 = false;
        }

        if (faceSelect.value == 2)
        {
            Face3 = true;
        }
        else
        {
            Face3 = false;
        }
        if (faceSelect.value == 3)
        {
            Face4 = true;
        }
        else
        {
            Face4 = false;
        }

        if (faceSelect.value == 4)
        {
            Face5 = true;
        }
        else
        {
            Face5 = false;
        }
        if (faceSelect.value == 5)
        {
            Face6 = true;
        }
        else
        {
            Face6 = false;
        }

        if (faceSelect.value == 6)
        {
            Face7 = true;
        }
        else
        {
            Face7 = false;
        }
        if (faceSelect.value == 7)
        {
            Face8 = true;
        }
        else
        {
            Face8 = false;
        }
    }
}
