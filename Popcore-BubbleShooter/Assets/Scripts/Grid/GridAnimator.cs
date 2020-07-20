using UnityEngine;

public class GridAnimator : MonoBehaviour
{
    [SerializeField]
    private GameObject grid;

    private void Start()
    {
        GridManager.MovedCellDown += MoveCellDown;
        GridManager.CellsFalling += ShakeGrid;
    }

    private void OnDestroy()
    {
        GridManager.MovedCellDown -= MoveCellDown;
        GridManager.CellsFalling -= ShakeGrid;
    }

    public void ShakeGrid()
    {
        LeanTween.moveX(grid, grid.transform.position.x + .045f, .1f).setLoopCount(4).setLoopPingPong();
    }

    private void MoveCellDown(int x, int y)
    {
        RectTransform cellTransform = (RectTransform)GridManager.Instance.CurrentGrid[x, y].transform;
        LeanTween.value(cellTransform.gameObject, cellTransform.anchoredPosition.y, cellTransform.anchoredPosition.y - GridManager.Instance.CellTransform.sizeDelta.y, .3f)
            .setEase(LeanTweenType.easeInOutBack)
            .setOnUpdate((value) => cellTransform.anchoredPosition = new Vector2(cellTransform.anchoredPosition.x, value))
            .setDelay((x + y) * .06f);
    }
}
