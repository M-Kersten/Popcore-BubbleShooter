using UnityEngine;

/// <summary>
/// Collection of constant values that should be accessible in the whole project
/// </summary>
namespace UnityTools
{
    /// <summary>
    /// Collection of constant values that should be accessible in the whole project, collected in 1 location to make looking up values quicker
    /// </summary>
    public class Constants
    {
        #region PlayerprefKeys
        /// <summary>
        /// level the player has reached
        /// </summary>
        public const string PLAYERLEVEL = "Level";
        #endregion
        #region SceneManagement
        /// <summary>
        /// Name of the gamescene, used to load the correct scene
        /// </summary>
        public const string MAINSCENE = "MainScene";
        /// <summary>
        /// Name of the title/intro scene
        /// </summary>
        public const string TITLESCENE = "TitleScene";
        #endregion
        #region Saving
        /// <summary>
        /// Since application.datapath is a runtime variable, this string is static readonly
        /// </summary>
        public static readonly string SAVEFOLDER = Application.persistentDataPath + "/ProgressData/";
        #endregion
    }
}