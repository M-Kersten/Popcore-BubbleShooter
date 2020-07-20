using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimator : MonoBehaviour
{

    private void Start()
    {
        GridManager.Instance.MovedCellDown += MoveCellDown;
    }

    private void OnDestroy()
    {
        GridManager.Instance.MovedCellDown -= MoveCellDown;
    }

    private void MoveCellDown(int x, int y)
    {
        RectTransform cellTransform = (RectTransform)GridManager.Instance.ActiveGrid[x, y].transform;
        LeanTween.value(cellTransform.gameObject, cellTransform.anchoredPosition.y, cellTransform.anchoredPosition.y - GridManager.Instance.CellTransform.sizeDelta.y, .3f)
            .setEase(LeanTweenType.easeInOutBack)
            .setOnUpdate((value) => cellTransform.anchoredPosition = new Vector2(cellTransform.anchoredPosition.x, value))
            .setDelay((x + y) * .06f);
    }
}
