using System;
using System.Collections.Generic;
using UnityEngine;
using JellySort.Data;

namespace JellySort.Data
{
    [CreateAssetMenu(fileName = "NewHexaLevel", menuName = "JellySort/Level Setup SO")]
    public class LevelSetupSO : ScriptableObject
    {
        [Header("Level Info")]
        public int LevelId;
        public int RequiredPoints;
        public int MovesLimit;
        public int TimeLimit;
        
        [Header("Spawn Settings")]
        public List<HexaColor> AllowedColors = new List<HexaColor>() { HexaColor.Red, HexaColor.Green, HexaColor.Blue };

        [Header("Nodes Layout")]
        public List<NodeSetup> Nodes = new List<NodeSetup>();

        [Header("Reward")] 
        public int RewardCoins;
    }

    [Serializable]
    public struct NodeSetup
    {
        public int Col;
        public int Row;
        
        public bool IsIceGrid;
        public bool IsLocked;
        
        public List<HexaColor> StackColors;
    }
}
