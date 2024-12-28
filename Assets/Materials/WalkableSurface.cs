using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class WalkableSurface : MonoBehaviour
{
    [SerializeField] Materials.MaterialType materialType = Materials.MaterialType.None;
    //Might add some code here idk, it's mainly just to hold the material type and what step sounds should be made
    public Materials.MaterialType GetMaterialType()
    {
        return materialType;
    }
}
