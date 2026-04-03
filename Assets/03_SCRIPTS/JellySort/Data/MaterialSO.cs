using System;
using System.Collections.Generic;
using Dylanng.Core.Base;
using UnityEngine;

namespace JellySort.Data
{
    [Serializable]
    public struct MaterialColor
    {
        public HexaColor Color;
        public Material Material;
    }
    [CreateAssetMenu(menuName = "JellySort/Material Data")]
    public class MaterialSO : ScriptableData
    {
        public List<MaterialColor> MaterialColors;
        public Material DefaultMaterial;

        public Material GetMaterialForColor(HexaColor color)
        {
            foreach (var mat in MaterialColors)
            {
                if (mat.Color == color)
                {
                    return mat.Material;
                }
            }
            return DefaultMaterial;
        }
    }
}
