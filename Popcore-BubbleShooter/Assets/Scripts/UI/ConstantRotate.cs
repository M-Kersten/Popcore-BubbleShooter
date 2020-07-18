using UnityEngine;

/// <summary>
/// Script to constantly rotate a gameobject
/// </summary>
public class ConstantRotate : MonoBehaviour
{
    #region Variables
    #region Editor
    /// <summary>
    /// Whether to start rotating on enable
    /// </summary>
    [SerializeField]
    private bool startOnEnable;
    /// <summary>
    /// The orientation to rotate in
    /// </summary>
    [SerializeField]
    private Vector3 rotateVector;
    #endregion
    #region Private
    /// <summary>
    /// Whether the gameobject is rotating currently
    /// </summary>
    private bool rotating;
    #endregion
    #endregion

    #region Methods
    #region Unity
    void Start () 
    {
        if (startOnEnable)
            rotating = true;
	}

    private void FixedUpdate()
    {
        if (rotating)
            transform.Rotate(rotateVector);
    }
    #endregion

    #region Public
    /// <summary>
    /// Start or stop rotating the gameobject
    /// </summary>
    /// <param name="rotate"></param>
    public void Rotate(bool rotate) => rotating = rotate;
    #endregion
    #endregion
}
