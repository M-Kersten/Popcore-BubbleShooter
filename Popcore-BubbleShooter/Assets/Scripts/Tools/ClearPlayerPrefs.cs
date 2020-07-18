using UnityEditor;
using UnityEngine;

/// <summary>
/// Collection of tools for unity related behaviours
/// </summary>
namespace UnityTools
{
    /// <summary>
    /// Class helping with testing playerprefs in unity editor
    /// </summary>
    public class ClearPlayerPrefs
    {
        /// <summary>
        /// Clears playerprefs by going to tools/clear playerprefs
        /// </summary>
#if UNITY_EDITOR
        [MenuItem("Tools/Clear playerprefs")]
        public static void ClearPrefs()
        {
            Debug.Log("playerprefs cleared");
            PlayerPrefs.DeleteAll();
        }
#endif
    }
}