using System;
using UnityEngine;

/// <summary>
/// A cell that is part of the grid
/// </summary>
public class GridCell : Cell
{
    public Vector2Int Index;
    public bool Seperated = false;

    public CellState State
    {
        get { return state; }
        set
        {
            state = value;
            StateChanged();
        }
    }

    [SerializeField]
    private CellState state;

    private void Awake()
    {
        Score = (int)Math.Pow(2, UnityEngine.Random.Range(1, 6));
        visualiser.UpdateVisuals(Score);
    }

    /// <summary>
    /// Moves the cell towards a neighbour with a higher exponent and merges with it
    /// </summary>
    /// <param name="upgradeCell"></param>
    public void MergeWithUpgrade(GridCell upgradeCell, Action callback)
    {
        GridManager.Instance.CurrentGrid[Index.x, Index.y] = null;
        visualiser.AnimateIn();
        visualiser.MoveToCell(upgradeCell, () =>
        {            
            DestroyCell();
            callback.Invoke();
        });
    }

    /// <summary>
    /// Destroy and animate out this cell
    /// </summary>
    public void DestroyCell(bool fallDown = false, int index = 0)
    {
        if (fallDown)
            visualiser.AnimateFallDown(index, callback: () => Destroy(gameObject));
        else
            visualiser.AnimateOut(State, () => Destroy(gameObject));
    }

    /// <summary>
    /// Function to call on state change to visually reflect the new state
    /// </summary>
    private void StateChanged()
    {
        visualiser.UpdateState(state);
    }
}
