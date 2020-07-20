
using UnityEngine;
/// <summary>
/// A cell that's being controlled by the player
/// </summary>
public class PlayerCell : Cell
{
    public Transform VisualiserTransform { get { return visualiser.transform; } }

    private void Start()
    {
        Score = PlayerCellShooter.Instance.CurrentBallScore;
    }
    public void Init(int score)
    {
        Score = score;
    }

    private void OnEnable()
    {
        visualiser.AnimateIn();
    }
}
