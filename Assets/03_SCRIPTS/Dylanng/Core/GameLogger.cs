using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Dylanng.Core
{
    public static class GameLogger
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message) => Debug.Log($"[LOG] {message}");

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message) => Debug.LogWarning($"[WARNING] {message}");

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message) => Debug.LogError($"[ERROR] {message}");
    }
}
