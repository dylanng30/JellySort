using System;
using System.Collections.Generic;
using UnityEngine;
using JellySort.Data;

namespace JellySort.Data
{
    [Serializable]
    public struct ShopItemData
    {
        public string ProductName;
        public string InternalID;
        public BoosterType BoosterType;
        public int Price;
        public int Amount;
        public Sprite Icon;
    }

    [CreateAssetMenu(fileName = "ShopConfig", menuName = "JellySort/Shop/Shop Config")]
    public class ShopConfigSO : ScriptableObject
    {
        public List<ShopItemData> Items = new List<ShopItemData>();
    }
}
