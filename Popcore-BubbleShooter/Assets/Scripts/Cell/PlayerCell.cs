using UnityEngine;

/// <summary>
/// A cell that's being controlled by the player
/// </summary>
public class PlayerCell : Cell
{
    [SerializeField]
    private bool animateIn = false;

    public Transform VisualiserTransform { get { return visualiser.transform; } }

    private void Start()
    {
        Score = PlayerCellShooter.Instance.CurrentBallScore;
    }

    /// <summary>
    /// Initializes the cell with a given score
    /// </summary>
    public void Init(int score)
    {
        Score = score;
    }

    private void OnEnable()
    {
        if (animateIn)
            visualiser.AnimateIn();
    }
}
