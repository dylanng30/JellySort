using System;
using System.Collections.Generic;

namespace JellySort.Data
{
    [Serializable]
    public class SaveData
    {
        public int CurrentLevel = 1;
        public int Coins = 100;
        public int Lives = 5;
        public long NextLifeRegenTimeTicks = 0;
        public List<BoosterData> Boosters = new List<BoosterData>();

        // User settings
        public bool IsSoundOn = true;
        public bool IsMusicOn = true;
        public bool IsHapticOn = true;
        
        public SavedLevelState ActiveLevelState = new SavedLevelState();
        
        public SaveData()
        {
            Boosters = new List<BoosterData>
            {
                new BoosterData { Type = BoosterType.Hammer, Count = 3 },
                new BoosterData { Type = BoosterType.Bomb, Count = 3 },
                new BoosterData { Type = BoosterType.RandomRockets, Count = 3 },
                new BoosterData { Type = BoosterType.ReverseStack, Count = 3 }
            };
        }
    }

    [Serializable]
    public class SavedLevelState
    {
        public bool HasActiveGame = false;
        public int LevelId;
        public int CurrentScore;
        public List<SavedNodeData> Nodes = new List<SavedNodeData>();
    }

    [Serializable]
    public struct SavedNodeData
    {
        public int Col;
        public int Row;
        public bool IsLocked;
        public List<HexaColor> StackColors;
    }

    [Serializable]
    public enum BoosterType
    {
        Hammer,
        Bomb,
        RandomRockets,
        ReverseStack
    }

    [Serializable]
    public struct BoosterData
    {
        public BoosterType Type;
        public int Count;
    }
}
