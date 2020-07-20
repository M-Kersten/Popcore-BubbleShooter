using UnityEngine;

/// <summary>
/// Base class for cell data
/// </summary>
[SelectionBase]
public class Cell : MonoBehaviour
{    
    public int Score
    {
        get { return score; } 
        set
        {
            score = value;
            visualiser.UpdateVisuals(score);
        } 
    }

    [SerializeField]
    protected CellVisualiser visualiser;

    private int score;    
}