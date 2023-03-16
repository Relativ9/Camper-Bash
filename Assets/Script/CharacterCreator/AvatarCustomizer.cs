using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AvatarCustomizer : AvatarSingletons<AvatarCustomizer>
{
    [Header("Manually assigned variable")]
    public SkinnedMeshRenderer baseSkinMesh;

    [Header("Editable in inspector")]
    public string suffixMax = "Max";
    public string suffixMin = "Min";

    //Assigned in start
    private Mesh mesh;

    public Dictionary<string, Blendshape> blendshapeDatabase = new Dictionary<string, Blendshape>();

    private void Start()
    {
        //Initialize();  
        mesh = baseSkinMesh.sharedMesh;
        ParseBlendShapesToDictionary();
    }

    public void ChangeBlendshapeValue(string blendshapeName, float value)
    {
        if (!blendshapeDatabase.ContainsKey(blendshapeName))
        {
            Debug.LogError("Blendshape " + blendshapeName + " does not exist!");
            return;
        }

        value = Mathf.Clamp(value, -100, 100);

        Blendshape blendshape = blendshapeDatabase[blendshapeName];

        if (value >= 0)
        {
            if (blendshape.posIndex == -1)
            {
                return;
            }
            baseSkinMesh.SetBlendShapeWeight(blendshape.posIndex, value);

            if (blendshape.negIndex == -1)
            {
                return;
            }
            baseSkinMesh.SetBlendShapeWeight(blendshape.negIndex, 0);
        }
        else
        {
            if (blendshape.negIndex == -1)
            {
                return;
            }
            baseSkinMesh.SetBlendShapeWeight(blendshape.negIndex, -value);

            if (blendshape.posIndex == -1)
            {
                return;
            }
            baseSkinMesh.SetBlendShapeWeight(blendshape.posIndex, 0);
        }
    }

    private void ParseBlendShapesToDictionary()
    {
        List<string> blendShapeNames = Enumerable.Range(0, mesh.blendShapeCount).Select(x => mesh.GetBlendShapeName(x)).ToList();

        for (int i = 0; blendShapeNames.Count > 0;)
        {
            string altSuffix;
            string noSuffix;

            noSuffix = blendShapeNames[i].TrimEnd(suffixMax.ToCharArray()).TrimEnd(suffixMin.ToCharArray()).Trim();

            string posName = string.Empty;
            string negName = string.Empty;

            bool exists = false;
            int posIndex = -1;
            int negIndex = -1;

            //If suffix is positive (max)
            if (blendShapeNames[i].EndsWith(suffixMax))
            {
                altSuffix = noSuffix + " " + suffixMin;

                posName = blendShapeNames[i];
                negName = altSuffix;

                if (blendShapeNames.Contains(altSuffix))
                {
                    exists = true;
                }

                posIndex = mesh.GetBlendShapeIndex(posName);


                if (exists == true)
                {
                    negIndex = mesh.GetBlendShapeIndex(altSuffix);
                }

            }

            //if suffix is negative(min)
            else if (blendShapeNames[i].EndsWith(suffixMin))
            {
                altSuffix = noSuffix + " " + suffixMax;

                negName = blendShapeNames[i];
                posName = altSuffix;

                if (blendShapeNames.Contains(altSuffix))
                {
                    exists = true;
                }
                negIndex = mesh.GetBlendShapeIndex(negName);

                if (exists)
                {
                    posIndex = mesh.GetBlendShapeIndex(altSuffix);
                }
            }

            else
            {
                posIndex = mesh.GetBlendShapeIndex(blendShapeNames[i]);
                negName = noSuffix;
            }


            if (blendshapeDatabase.ContainsKey(noSuffix))
            {
                Debug.LogError(noSuffix + " already exists within the Database!");
            }

            blendshapeDatabase.Add(noSuffix, new Blendshape(posIndex, negIndex));


            //Remove Selected Indexes From the List
            if (posName != string.Empty)
            {
                blendShapeNames.Remove(posName);
            }
            if (negName != string.Empty)
            {
                blendShapeNames.Remove(negName);
            }
        }
    }
}
