using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    [SerializeField]
    private Cell cellPrefab;

    private void Start()
    {
        RayCastShooter.Instance.FiredPositions += AnimateShot;
    }

    private void OnDestroy()
    {
        RayCastShooter.Instance.FiredPositions -= AnimateShot;
    }

    public void AnimateShot(List<Vector3> positions)
    {
        Cell cell = Instantiate(cellPrefab.gameObject, transform).GetComponent<Cell>();
        cell.Init(RayCastShooter.Instance.CurrentBallScore);
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

    private void MoveBall(List<Vector3> positions, GameObject cell, int i, LeanTweenType tween)
    {
        LeanTween.move(cell, positions[i], .5f)
                        .setDelay((i - 1) * .5f)
                        .setEase(tween)
                        .setOnComplete(() =>
                        {
                            if (i == positions.Count - 1)
                            {
                                Destroy(cell.gameObject);
                                RayCastShooter.Instance.BallInPosition = true;
                            }
                        });
    }
}
