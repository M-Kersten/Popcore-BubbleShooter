using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animates the cells being added to the grid
/// </summary>
public class PlayerCellAnimator : MonoBehaviour
{
    [SerializeField]
    private PlayerCell cellPrefab;

    private void Start()
    {
        PlayerCellShooter.FiredPositions += AnimateShot;
    }

    private void OnDestroy()
    {
        PlayerCellShooter.FiredPositions -= AnimateShot;
    }

    /// <summary>
    /// Animate the player cell moving towards it assigned spot in the grid
    /// </summary>
    public void AnimateShot(List<Vector3> positions)
    {
        PlayerCell cell = Instantiate(cellPrefab.gameObject, transform).GetComponent<PlayerCell>();
        cell.Init(PlayerCellShooter.Instance.CurrentBallScore);
        for (int i = 1; i < positions.Count; i++)
        {
            LeanTweenType tween = LeanTweenType.notUsed;
            if (i == positions.Count - 1)
            {
                tween = LeanTweenType.easeOutSine;
                if (i == 1)
                    tween = LeanTweenType.easeInOutSine;
            }
            else if (i == 1)
                tween = LeanTweenType.easeInSine;
            MoveBall(positions, cell.gameObject, i, tween);
        }
    }

    /// <summary>
    /// Animate the playercell along the bounce paths towards a given position
    /// </summary>
    private void MoveBall(List<Vector3> positions, GameObject cell, int i, LeanTweenType tween)
    {
        LeanTween.scale(cell, Vector3.one * 1.2f, .2f).setEase(LeanTweenType.easeInOutSine).setDelay((i - 1) * .5f).setLoopPingPong().setLoopOnce();
        LeanTween.move(cell, positions[i], .5f)
                        .setDelay((i - 1) * .5f)
                        .setEase(tween)
                        .setOnComplete(() =>
                        {
                            if (i == positions.Count - 1)
                            {
                                Destroy(cell.gameObject);
                                PlayerCellShooter.Instance.BallInPosition = true;
                            }
                        });
    }
}
