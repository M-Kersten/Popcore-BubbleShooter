using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField]
    private Cell cellPrefab;

    private GameObject gridInstance;

    [SerializeField]
    private Transform gridRoot;

    [SerializeField]
    private GameObject gridPrefab;

    [SerializeField]
    private LevelSettings settings;

    /// <summary>
    /// All the cells in the currently active grid
    /// </summary>
    public Cell[,] ActiveGrid;

    public RectTransform CellTransform => cellPrefab.transform as RectTransform;
    private RectTransform GridRect => gridInstance.transform as RectTransform;


    private void Start()
    {
        SpawnGrid();
    }

    private void SpawnGrid()
    {
        gridInstance = Instantiate(gridPrefab, gridRoot);
        CellTransform.sizeDelta = new Vector2((GridRect.rect.size.x * 1.05f) / settings.TotalCollums, GridRect.rect.size.y / settings.TotalRows);

        ActiveGrid = new Cell[settings.TotalCollums, settings.TotalRows];

        for (int x = 0; x < settings.TotalCollums; x++)
        {
            for (int y = settings.TotalRows - 1; y >= settings.StartingRows; y--)
            {
                CreateNewCell(x, y, (y + x) * .1f);
            }
        }
    }

    private void CreateNewCell(int x, int y, float delay = 0)
    {
        Cell newTile = Instantiate(cellPrefab, Vector3.zero, cellPrefab.transform.rotation, gridInstance.transform);
        ((RectTransform)newTile.transform).anchoredPosition = new Vector2(CellTransform.rect.width * (y % 2 == 0 ? x : x + .66f) - GridRect.rect.size.x / 2, CellTransform.rect.height * (y + .5f) - GridRect.rect.size.y / 2);
    }
}
