using UnityEngine;
using Dylanng.Core.Base;

namespace Dylanng.Data
{
    [CreateAssetMenu(fileName = "LevelData_", menuName = "Dylanng/Data/Level Data")]
    public class LevelData : ScriptableData
    {
        [Header("Level Info")]
        public int LevelID;
        public GameObject LevelPrefab;

        [Header("Settings")]
        public float TimeLimit = 0f;
        public int TargetScore = 100;
        //Thêm các thông số khác
    }
}