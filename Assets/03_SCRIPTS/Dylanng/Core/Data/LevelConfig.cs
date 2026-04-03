using System.Collections.Generic;
using UnityEngine;
using Dylanng.Core.Base;

namespace Dylanng.Data
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Dylanng/Data/Level Config")]
    public class LevelConfig : ScriptableData
    {
        public List<LevelData> Levels;

        [Header("Loop Settings (Hyper Casual)")]
        [Tooltip("Nếu bật, khi người chơi vượt qua level cuối, game sẽ lặp lại các level cũ.")]
        public bool LoopLevels = true;
        [Tooltip("Index bắt đầu lặp. Ví dụ: bỏ qua các level hướng dẫn (0, 1) và bắt đầu lặp từ level 2.")]
        public int LoopStartIndex = 0;
    }
}