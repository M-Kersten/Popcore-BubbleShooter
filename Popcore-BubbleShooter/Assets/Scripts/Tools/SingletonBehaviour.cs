using UnityEngine;

/// <summary>
/// Contains logic for easy implementation of singleton design pattern
/// </summary>
namespace SingletonUtility
{
    /// <summary>
    /// Turns any MonoBehaviour into a Singleton. Note that i imported this from one of my previous projects to make my life a bit easier :)
    /// If this is not acceptable due to the "no external tools" guideline, i've implement the singleton pattern manually in some other scripts too (levelselectpopup.cs for instance)
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// Check whether an instance exists in scene
        /// </summary>
        public static bool Exists => instance != null;

        /// <summary>
        /// Singleton-Reference
        /// Auto-Creates GameObject if it does not exist
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                GameObject obj = new GameObject
                {
                    // Giving the generated singleton a preset suffix makes it easier to discern in scene view
                    name = typeof(T).Name + "-Instance"
                };
                instance = obj.AddComponent<T>();
                return instance;
            }
            protected set => instance = value;
        }
        #endregion

        #region Private
        /// <summary>
        /// Internal Singleton-Reference
        /// Protected get so it can be overridden through inheritance
        /// </summary>
        protected static T instance { get; private set; }
        #endregion

        #region Instance
        /// <summary>
        /// Whether this Singleton has a Root-Object. If true, root-Object will be added to DontDestroyOnLoad instead
        /// </summary>
        [SerializeField]
        [Tooltip("Set true if gameobject is in root of the scene")]
        private bool isRootObject;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Singleton-Setup in awake
        /// </summary>
        protected virtual void Awake()
        {
            // destroy duplicate singleton instances
            if (instance != null && instance != this)
            {
                Debug.Log($"DUPLICATE SINGLETON: instance of {gameObject.name} already exists, destroying it now");
                Destroy(gameObject);
                return;
            }
            if (isRootObject)
                DontDestroyOnLoad(gameObject);
            else
                DontDestroyOnLoad(transform.root.gameObject);
            instance = this as T;
        }

        /// <summary>
        /// Singleton-Destruction
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (Exists && instance == this)
                instance = null;
        }
        #endregion
    }
}