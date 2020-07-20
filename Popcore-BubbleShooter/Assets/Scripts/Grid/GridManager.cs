using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Singleton instance
    public static GridManager Instance { get; private set; }
    #endregion

    [SerializeField]
    private Cell cellPrefab;

    private GameObject gridInstance;

    [SerializeField]
    private Transform gridRoot;

    [SerializeField]
    private GameObject gridPrefab;

    [SerializeField]
    private LevelSettings settings;

    private int activeRows;

    private bool spawningEvenRow;

    [SerializeField]
    private MessageScreenController messageScreen;

    /// <summary>
    /// All the cells in the currently active grid
    /// </summary>
    public Cell[,] ActiveGrid;

    public RectTransform CellTransform => cellPrefab.transform as RectTransform;
    private RectTransform GridRect => gridInstance.transform as RectTransform;

    public Action<Cell, float> CellCreated;
    public Action<int, int> MovedCellDown;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        SpawnGrid();
        RayCastShooter.Instance.Fired += CellAdded;
    }

    private void OnDestroy()
    {
        RayCastShooter.Instance.Fired -= CellAdded;
    }

    private void CellAdded(Cell cell)
    {
        MatchCells(cell);
        if (CheckForGameOver())
            ResetBoard();
        MoveGridDown();
    }

    private void ResetBoard()
    {
        List<Cell> allCells = GetAllCells(CellState.Filled);
        for (int i = 0; i < allCells.Count; i++)
        {
            Cell cell = allCells[i];
            ActiveGrid[cell.Index.x, cell.Index.y] = null;
            cell.DestroyCell(true, i);
        }
        GridShake();
    }

    private bool CheckForGameOver()
    {
        bool gameOver = false;

        List<Cell> allCells = GetAllCells(CellState.Filled);
        foreach (Cell cell in allCells)
        {
            if (cell.Index.y == 0)
            {
                gameOver = true;
                break;
            }
        }

        if (gameOver)        
            messageScreen.ShowScreen(UserMessage.GameOver);
        
        return gameOver;
    }

    private void SpawnGrid()
    {
        gridInstance = Instantiate(gridPrefab, gridRoot);
        CellTransform.sizeDelta = new Vector2((GridRect.rect.size.x * 1.05f) / settings.TotalCollums, GridRect.rect.size.y / settings.TotalRows);

        ActiveGrid = new Cell[settings.TotalCollums, settings.TotalRows + 1];

        for (int x = 0; x < settings.TotalCollums; x++)
        {
            for (int y = settings.TotalRows - 1; y >= settings.StartingRows; y--)
            {
                CreateNewCell(x, y);
            }
        }
        activeRows = settings.StartingRows;
        SpawnEmptyCells();
    }

    private void GridShake()
    {
        LeanTween.moveX(gridRoot.gameObject, gridRoot.transform.position.x + .03f, .1f).setLoopCount(4).setLoopPingPong();
    }

    private void AddCellRow()
    {
        spawningEvenRow = !spawningEvenRow;
        for (int x = 0; x < settings.TotalCollums; x++)
            CreateNewCell(x, settings.TotalRows);
        
        activeRows++;
    }

    private void MoveGridDown()
    {
        /*
        if (activeRows == settings.TotalRows)
        {
            Debug.Log("game over!");
            return;
        }
        */
        
        RemoveEmptyCells();
        AddCellRow();

        // set new indices
        for (int x = 0; x < settings.TotalCollums; x++)
        {
            for (int y = 0; y < settings.TotalRows; y++)
            {                
                if (ActiveGrid[x, y + 1] != null)
                {           
                    ActiveGrid[x, y] = ActiveGrid[x, y + 1];
                    ActiveGrid[x, y + 1] = null;

                    ActiveGrid[x, y].Index = new Vector2Int(x, y);
                    MovedCellDown?.Invoke(x, y);
                }
            }
        }
        SpawnEmptyCells();
    }

    private Cell CreateNewCell(int x, int y, CellState state = CellState.Filled)
    {
        // adjust the cells horizontally to create a "beehive" pattern which is nicer to look at
        float horizontalAdjustment = y % 2 == 0 ? x : x + .5f;
        if (y == settings.TotalRows)        
            horizontalAdjustment = spawningEvenRow ? x : x + .5f;
        if (state == CellState.Empty)
            horizontalAdjustment = (activeRows - y) % 2 == 0 ? x : x + .5f;


        Cell newCell = Instantiate(cellPrefab, Vector3.zero, cellPrefab.transform.rotation, gridInstance.transform);
        newCell.State = state;

        ((RectTransform)newCell.transform).anchoredPosition = new Vector2(CellTransform.rect.width  * horizontalAdjustment - GridRect.rect.size.x / 2, 
                                                                          CellTransform.rect.height * (y + .5f) - GridRect.rect.size.y / 2);
        ActiveGrid[x, y] = newCell;
        newCell.Index = new Vector2Int(x, y);
               
        return newCell;
    }

    private void DeleteSeperatedCells()
    {
        foreach (Cell cell in GetAllCells(CellState.Filled))        
            cell.Seperated = true;

        for (int x = 0; x < settings.TotalCollums; x++)
        {
            if (ActiveGrid[x, settings.TotalRows - 1] != null && ActiveGrid[x, settings.TotalRows - 1].State == CellState.Filled)
                ActiveGrid[x, settings.TotalRows - 1].Seperated = false;
        }

        for (int x = 0; x < settings.TotalCollums; x++)
        {
            for (int y = settings.TotalRows - 1; y >= 0; y--)
            {
                if (ActiveGrid[x, y] == null)
                    continue;

                if (ActiveGrid[x, y].State == CellState.Filled)
                {
                    
                    List<Cell> connectedCells = GetNeighbours(ActiveGrid[x, y], false, false).ToList();
                    foreach (var cell in connectedCells)
                    {
                        if (cell.Seperated == false)
                        {
                            ActiveGrid[x, y].Seperated = false;
                        }
                    }
                }
            }
        }
        List<Cell> seperatedCells = GetAllCells(CellState.Filled).Where(x => x.Seperated).ToList();
        if (seperatedCells.Count < 1)
            return;

        GridShake();
        for (int i = 0; i < seperatedCells.Count; i++)
        {
            Cell cell = seperatedCells[i];            
                ActiveGrid[cell.Index.x, cell.Index.y] = null;
                cell.DestroyCell(true, i);            
        }
    }

    public void MatchCells(Cell newCell)
    {
        AudioManager.Instance.PlayClip(0);

        HashSet<Cell> matchedCells = GetNeighbours(newCell);
        HashSet<Cell> newNeighbours = matchedCells;

        // while new unique neighbours have been found, keep looking for more
        while (newNeighbours.Count > 0)
        {
            newNeighbours = new HashSet<Cell>();
            foreach (var cell in matchedCells)
                newNeighbours.UnionWith(GetNeighbours(cell));
            
            newNeighbours.ExceptWith(matchedCells);
            matchedCells.UnionWith(newNeighbours);
        }

        Cell upgradeCell = null;
        List<Cell> matches = matchedCells.ToList();

        for (int i = matches.Count - 1; i >= 0; i--)
        {
            // interaction logic here
            // check for a neighbour with score and change the score of the cell
            if (upgradeCell == null && GetNeighbours(matches[i], false, true).Count > 0)
            {
                matches[i].Score *= 2;
                upgradeCell = matches[i];

                // if the highest possible cellscore is reached, destroy the cell and its surrounding cells with it
                if (matches[i].Score == 2048)
                {
                    List<Cell> explodeCellList = GetNeighbours(matches[i], false, false).ToList();
                    foreach (Cell cell in explodeCellList)
                    {
                        ActiveGrid[cell.Index.x, cell.Index.y] = null;
                        cell.DestroyCell();
                    }
                }
            }
            else
            {
                ActiveGrid[matches[i].Index.x, matches[i].Index.y] = null;
                matches[i].DestroyCell();
            }
        }
        DeleteSeperatedCells();

        if (upgradeCell != null)        
            MatchCells(upgradeCell);

        if (GetAllCells(CellState.Filled).Count < 1)        
            messageScreen.ShowScreen(UserMessage.LevelClear);        
    }

    public HashSet<Cell> GetNeighbours(Cell cell, bool onlyValid = true, bool checkForUpgrade = false)
    {
        HashSet<Cell> neighbours = new HashSet<Cell>();
        for (int x = cell.Index.x - 1; x <= cell.Index.x + 1; x++)
        {
            for (int y = cell.Index.y - 1; y <= cell.Index.y + 1; y++)
            {
                int offset = (activeRows - cell.Index.y) % 2 == 0 ? 1 : -1;
                if ((x == cell.Index.x + offset && y == cell.Index.y - 1) || (x == cell.Index.x + offset && y == cell.Index.y + 1))
                    continue;
                // avoid cycling through cells outside of grid scope or the current cell
                if (x >= 0 && y >= 0 && x < settings.TotalCollums && y < settings.TotalRows && ActiveGrid[x, y] != null && ActiveGrid[x,y].State == CellState.Filled && cell.Index != ActiveGrid[x,y].Index)
                {
                    // if only valid/linkable cells should be returned, skip the current cell if invalid
                    if (onlyValid && ActiveGrid[x, y].Score != cell.Score)
                        continue;
                    else if (checkForUpgrade && ActiveGrid[x, y].Score != cell.Score * 2)
                        continue;

                    neighbours.Add(ActiveGrid[x, y]);
                }
            }
        }
        return neighbours;
    }

    private List<Cell> GetAllCells(CellState state = CellState.Filled)
    {
        List<Cell> returnCells = new List<Cell>();
        for (int x = 0; x < settings.TotalCollums; x++)        
            for (int y = 0; y < settings.TotalRows; y++)            
                if (ActiveGrid[x,y] != null && ActiveGrid[x,y].State == state)                
                    returnCells.Add(ActiveGrid[x, y]);
        return returnCells;
    }

    private void RemoveEmptyCells()
    {
        // remove current empty cells
        List<Cell> emptyCells = GetAllCells(CellState.Empty);
        emptyCells.AddRange(GetAllCells(CellState.Preview));
        foreach (Cell cell in emptyCells)
        {
            ActiveGrid[cell.Index.x, cell.Index.y] = null;
            cell.DestroyCell();
        }
    }

    private void SpawnEmptyCells()
    {     
        // spawn new ones
        List<Cell> filledCells = GetAllCells(CellState.Filled);

        foreach (Cell cell in filledCells)
        {
            for (int x = cell.Index.x - 1; x <= cell.Index.x + 1; x++)
            {
                for (int y = cell.Index.y - 1; y <= cell.Index.y + 1; y++)
                {
                    // cancel out the diagonal neighbours on one side to account for the beehive pattern, takes a bit of logic, not the prettiest but gets the job done
                    int offset = (activeRows - cell.Index.y) % 2 == 0 ? 1 : -1;                    
                    if ((x == cell.Index.x + offset && y == cell.Index.y - 1) || (x == cell.Index.x + offset && y == cell.Index.y + 1))          
                        continue;

                    if (x >= 0 && y >= 0 && x < settings.TotalCollums && y < settings.TotalRows)                    
                        if (ActiveGrid[x, y] == null)
                            CreateNewCell(x, y, CellState.Empty);                    
                }
            }
        }
    }

}
