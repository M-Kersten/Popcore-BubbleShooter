﻿using UnityEngine;

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

    /// <summary>
    /// Shake the grid, used to simulate the grid shaking of loose cells
    /// </summary>
    public void ShakeGrid()
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
