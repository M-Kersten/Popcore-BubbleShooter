using UnityEngine;

/// <summary>
/// Class for handling all cell animations in the grid
/// </summary>
public class GridAnimator : MonoBehaviour
{
    /// <summary>
    /// Defines the root object of the grid used in animations
    /// </summary>
    [SerializeField]
    private GameObject grid;

    [SerializeField]
    private float maxRotationDegrees;
    [SerializeField]
    private float minRotationDegrees;

    private bool movedLeft;

    private void Start()
    {
        GridManager.MovedCellDown += MoveCellDown;
        GridManager.CellsDestroyed += ShakeGrid;
        GridManager.TurnOver += GridRotate;
    }

    private void OnDestroy()
    {
        GridManager.MovedCellDown -= MoveCellDown;
        GridManager.CellsDestroyed -= ShakeGrid;
        GridManager.TurnOver -= GridRotate;
    }

    public void GridRotate(bool reset)
    {
        float newDegrees = Random.Range(minRotationDegrees, maxRotationDegrees);
        if (movedLeft)
            newDegrees *= -1;
        if (reset)
            newDegrees = 0;
        LeanTween.rotateLocal(grid, Vector3.forward * newDegrees, .5f).setEase(LeanTweenType.easeInOutBack);
        LeanTween.scale(grid, Vector3.one * .94f, .25f).setEase(LeanTweenType.easeInOutQuart).setOnComplete(() => LeanTween.scale(grid, Vector3.one, .25f).setEase(LeanTweenType.easeInOutQuart));
        movedLeft = !movedLeft;
    }

    /// <summary>
    /// Shake the grid, used to simulate the grid shaking of loose cells
    /// </summary>
    private void ShakeGrid()
    {
        LeanTween.moveX(grid, grid.transform.position.x + .045f, .1f).setLoopCount(4).setLoopPingPong();
    }

    /// <summary>
    /// Move cell in the given index downwards by 1
    /// </summary>
    private void MoveCellDown(int x, int y)
    {
        RectTransform cellTransform = (RectTransform)GridManager.Instance.CurrentGrid[x, y].transform;
        LeanTween.value(cellTransform.gameObject, cellTransform.anchoredPosition.y, cellTransform.anchoredPosition.y - GridManager.Instance.CellTransform.sizeDelta.y, .3f)
            .setEase(LeanTweenType.easeInOutBack)
            .setOnUpdate((value) => cellTransform.anchoredPosition = new Vector2(cellTransform.anchoredPosition.x, value))
            .setDelay((x + y) * .06f);
    }
}
