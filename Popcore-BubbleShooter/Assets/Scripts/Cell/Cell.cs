using System;
using UnityEngine;

[SelectionBase]
public class Cell : MonoBehaviour
{
    public CellState State
    {
        get { return state; }
        set 
        { 
            state = value;
            StateChanged();
        }
    }
        
    public bool PlayerBall;
    
    public int Score
    {
        get { return score; } 
        set
        {
            score = value;
            visualiser.UpdateVisuals(score);
        } 
    }

    public Vector2Int Index;

    public bool Seperated = false;

    [SerializeField]
    private CellVisualiser visualiser;
    private int score;
    [SerializeField]
    private CellState state;

    public Transform visualiserTransform { get { return visualiser.transform; } }

    private void Awake()
    {
        if (PlayerBall)
        {
            Score = RayCastShooter.Instance.CurrentBallScore;
            return;
        }

        Score = (int)Math.Pow(2, UnityEngine.Random.Range(1, 6));
        visualiser.UpdateVisuals(Score);
    }

    public void Init(int score)
    {
        Score = score;
    }

    private void OnEnable()
    {
        if (PlayerBall)        
            visualiser.AnimateIn();
    }

    public void DestroyCell(bool fallDown = false, int index = 0)
    {
        if (fallDown)
            visualiser.AnimateFallDown(index, callback: () => Destroy(gameObject));
        else
            visualiser.AnimateOut(State, () => Destroy(gameObject));       
    }

    private void StateChanged()
    {
        visualiser.UpdateState(state);
    }
}