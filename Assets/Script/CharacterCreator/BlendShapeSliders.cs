using UnityEngine;
using UnityEngine.UI;

public class BlendShapeSliders : MonoBehaviour
{
    //[RequireComponent(typeof(Slider))]
    //Do not need suffix
    [Header("Do not include the suffixes of the BlendShape Name")]
    public string blendShapeName;
    private Slider slider;
    public bool baseChanged;
    private void Awake()
    {
        blendShapeName = blendShapeName.Trim();
        slider = GetComponent<Slider>();

        //When slider is moved, then call function based on the blendshape name and pass float of slider

        slider.onValueChanged.AddListener(value => AvatarCustomizer.Instance.ChangeBlendshapeValue(blendShapeName, value));

    }
}