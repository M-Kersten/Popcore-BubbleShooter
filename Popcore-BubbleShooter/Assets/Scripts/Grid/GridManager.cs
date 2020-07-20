using Bubbleshooter.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Singleton instance
    public static GridManager Instance { get; private set; }
    #endregion

    /// <summary>
    /// All the cells in the currently active grid
    /// </summary>
    public GridCell[,] CurrentGrid;

    public static Action<GridCell, float> CellCreated;
    public static Action<int, int> MovedCellDown;
    public static Action CellsFalling;

    public RectTransform CellTransform => cellPrefab.transform as RectTransform;
    private RectTransform GridRect => gridInstance.transform as RectTransform;

    [SerializeField]
    private GridCell cellPrefab;
    [SerializeField]
    private Transform gridRoot;
    [SerializeField]
    private GameObject gridPrefab;
    [SerializeField]
    private LevelSettings settings;  
    [SerializeField]
    private MessageScreenController messageScreen;

    private int activeRows;
    private GameObject gridInstance;
    private bool spawningEvenRow;

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
        PlayerCellShooter.Fired += CellAdded;
    }

    private void OnDestroy()
    {
        PlayerCellShooter.Fired -= CellAdded;
    }

    /// <summary>
    /// Function to call when a cell has been added to the grid
    /// </summary>
    /// <param name="cell"></param>
    private void CellAdded(GridCell cell)
    {        
        MatchCells(cell, () => {
        if (CheckForGameOver())
            ResetGrid();
        MoveGridDown();
        CoroutineManager.Instance.Wait((settings.TotalCollums + settings.TotalRows) *.065f, () => BubbleInput.Instance.InputEnabled = true);
        });
    }

    /// <summary>
    /// Spawn in a new grid with specified settings
    /// </summary>
    private void SpawnGrid()
    {
        gridInstance = Instantiate(gridPrefab, gridRoot);
        CellTransform.sizeDelta = new Vector2((GridRect.rect.size.x * settings.HorizontalCellspacing) / settings.TotalCollums, GridRect.rect.size.y / settings.TotalRows);

        CurrentGrid = new GridCell[settings.TotalCollums, settings.TotalRows + 1];

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

    /// <summary>
    /// Resets the grid to 1 line of cells
    /// </summary>
    private void ResetGrid()
    {
        List<GridCell> allCells = GetAllCells(CellState.Filled);
        for (int i = 0; i < allCells.Count; i++)
        {
            GridCell cell = allCells[i];
            CurrentGrid[cell.Index.x, cell.Index.y] = null;
            cell.DestroyCell(true, i);
        }

        CellsFalling();
        Bubbleshooter.Feedback.VibrationManager.VibrateError();
    }

    /// <summary>
    /// Whether the grid is filled too much, leading to a start over
    /// </summary>
    /// <returns></returns>
    private bool CheckForGameOver()
    {
        bool gameOver = false;
        List<GridCell> allCells = GetAllCells(CellState.Filled);
        foreach (GridCell cell in allCells)
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
       
    private void AddCellRow()
    {
        spawningEvenRow = !spawningEvenRow;
        for (int x = 0; x < settings.TotalCollums; x++)
            CreateNewCell(x, settings.TotalRows);
        
        activeRows++;
    }

    private void MoveGridDown()
    {        
        RemoveEmptyCells();
        AddCellRow();

        // set new indices
        for (int x = 0; x < settings.TotalCollums; x++)
        {
            for (int y = 0; y < settings.TotalRows; y++)
            {                
                if (CurrentGrid[x, y + 1] != null)
                {           
                    CurrentGrid[x, y] = CurrentGrid[x, y + 1];
                    CurrentGrid[x, y + 1] = null;

                    CurrentGrid[x, y].Index = new Vector2Int(x, y);
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


        GridCell newCell = Instantiate(cellPrefab, Vector3.zero, cellPrefab.transform.rotation, gridInstance.transform);
        newCell.State = state;

        ((RectTransform)newCell.transform).anchoredPosition = new Vector2(CellTransform.rect.width  * horizontalAdjustment - GridRect.rect.size.x / 2, 
                                                                          CellTransform.rect.height * (y + .5f) - GridRect.rect.size.y / 2);
        CurrentGrid[x, y] = newCell;
        newCell.Index = new Vector2Int(x, y);
               
        return newCell;
    }

    private void DeleteSeperatedCells()
    {
        // by default all cells set to be seperated
        foreach (GridCell cell in GetAllCells(CellState.Filled))        
            cell.Seperated = true;

        // the top cells are the anchor points
        for (int x = 0; x < settings.TotalCollums; x++)
        {
            if (CurrentGrid[x, settings.TotalRows - 1] != null && CurrentGrid[x, settings.TotalRows - 1].State == CellState.Filled)
            {
                CurrentGrid[x, settings.TotalRows - 1].Seperated = false;
            }
        }

        // go through each cell from top to bottom and set them to be connected if a neighbour is already connected
        for (int x = 0; x < settings.TotalCollums; x++)
        {
            for (int y = settings.TotalRows - 1; y >= 0; y--)
            {
                if (CurrentGrid[x, y] == null)
                    continue;

                if (CurrentGrid[x, y].State == CellState.Filled && CurrentGrid[x, y].Seperated == false)
                {
                    foreach (var cell in GetNeighbours(CurrentGrid[x, y], false, false).ToList())
                    {
                        cell.Seperated = false;
                    }
                }
            }
        }
        List<GridCell> seperatedCells = GetAllCells(CellState.Filled).Where(x => x.Seperated).ToList();
        if (seperatedCells.Count < 1)
            return;

        CellsFalling();

        for (int i = 0; i < seperatedCells.Count; i++)
        {
            GridCell cell = seperatedCells[i];            
                CurrentGrid[cell.Index.x, cell.Index.y] = null;
                cell.DestroyCell(true, i);            
        }
    }

    public void MatchCells(GridCell newCell, Action callback = null)
    {
        AudioManager.Instance.PlayClip(0);
        Bubbleshooter.Feedback.VibrationManager.VibrateSelect();

        HashSet<GridCell> matchedCells = GetNeighbours(newCell);
        HashSet<GridCell> newNeighbours = matchedCells;

        // while new unique neighbours have been found, keep looking for more
        while (newNeighbours.Count > 0)
        {
            newNeighbours = new HashSet<GridCell>();
            foreach (var cell in matchedCells)
                newNeighbours.UnionWith(GetNeighbours(cell));
            
            newNeighbours.ExceptWith(matchedCells);
            matchedCells.UnionWith(newNeighbours);
        }

        GridCell upgradeCell = null;
        List<GridCell> matches = matchedCells.ToList();

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
                    List<GridCell> explodeCellList = GetNeighbours(matches[i], false, false).ToList();
                    foreach (GridCell cell in explodeCellList)
                    {
                        CurrentGrid[cell.Index.x, cell.Index.y] = null;
                        cell.DestroyCell();
                    }
                }
            }
        }
        for (int i = 0; i < matches.Count; i++)
        {
            if (upgradeCell == null)
            {
                matches[i].Score *= 2;
                upgradeCell = matches[i];
            }
            else            
                CurrentGrid[matches[i].Index.x, matches[i].Index.y].MergeWithUpgrade(upgradeCell);
        }
        
        CoroutineManager.Instance.Wait(.5f, () => 
        {
            DeleteSeperatedCells();

            if (upgradeCell != null)   
                MatchCells(upgradeCell, callback);
            else            
                callback?.Invoke();
           
            int remainingCells = GetAllCells(CellState.Filled).Count;
            if (remainingCells < 1)      
                messageScreen.ShowScreen(UserMessage.LevelClear);
            else if (remainingCells < settings.TotalCollums - 1)
                messageScreen.ShowScreen(UserMessage.GoodJob);
        });
    }

    public HashSet<GridCell> GetNeighbours(GridCell cell, bool onlyValid = true, bool checkForUpgrade = false)
    {
        HashSet<GridCell> neighbours = new HashSet<GridCell>();
        for (int x = cell.Index.x - 1; x <= cell.Index.x + 1; x++)
        {
            for (int y = cell.Index.y - 1; y <= cell.Index.y + 1; y++)
            {
                int offset = (activeRows - cell.Index.y) % 2 == 0 ? 1 : -1;
                if ((x == cell.Index.x + offset && y == cell.Index.y - 1) || (x == cell.Index.x + offset && y == cell.Index.y + 1))
                    continue;
                // avoid cycling through cells outside of grid scope or the current cell
                if (x >= 0 && y >= 0 && x < settings.TotalCollums && y < settings.TotalRows && CurrentGrid[x, y] != null && CurrentGrid[x,y].State == CellState.Filled && cell.Index != CurrentGrid[x,y].Index)
                {
                    // if only valid/linkable cells should be returned, skip the current cell if invalid
                    if (onlyValid && CurrentGrid[x, y].Score != cell.Score)
                        continue;
                    else if (checkForUpgrade && CurrentGrid[x, y].Score != cell.Score * 2)
                        continue;

                    neighbours.Add(CurrentGrid[x, y]);
                }
            }
        }
        return neighbours;
    }

    private List<GridCell> GetAllCells(CellState state = CellState.Filled)
    {
        List<GridCell> returnCells = new List<GridCell>();
        for (int x = 0; x < settings.TotalCollums; x++)        
            for (int y = 0; y < settings.TotalRows; y++)            
                if (CurrentGrid[x,y] != null && CurrentGrid[x,y].State == state)                
                    returnCells.Add(CurrentGrid[x, y]);
        return returnCells;
    }

    private void RemoveEmptyCells()
    {
        // remove current empty cells
        List<GridCell> emptyCells = GetAllCells(CellState.Empty);
        emptyCells.AddRange(GetAllCells(CellState.Preview));
        foreach (GridCell cell in emptyCells)
        {
            CurrentGrid[cell.Index.x, cell.Index.y] = null;
            cell.DestroyCell();
        }
    }

    private void SpawnEmptyCells()
    {     
        // spawn new ones
        List<GridCell> filledCells = GetAllCells(CellState.Filled);

        foreach (GridCell cell in filledCells)
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
                        if (CurrentGrid[x, y] == null)
                            CreateNewCell(x, y, CellState.Empty);                    
                }
            }
        }
    }

}
