using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerCellShooter : SingletonUtility.Singleton<PlayerCellShooter>
{
	public static Action<GridCell> Fired;
	public static Action<List<Vector3>> FiredPositions;

	
	public bool BallInPosition;	
	public int CurrentBallScore;
	
	[SerializeField]
	private GameObject canvas;
	[SerializeField]
	private LineRenderer aimLine;
	[SerializeField]
	private PlayerCell playerCell;
	[SerializeField]
	private Transform player;

	private GridCell currentlySelected;
	
	private bool animating;
	private List<Vector3> hitPositions = new List<Vector3>();

	private GridCell selectedCell
    {
        get { return currentlySelected; }
        set
        {
			if (value == null)
			{
				currentlySelected = value;
				return;
			}
			
			if (value.State == CellState.Filled)
				return;

            if (currentlySelected != null)
				currentlySelected.State = CellState.Empty;

			currentlySelected = value;
			currentlySelected.State = CellState.Preview;
		}
    }	

    private void Start()
    {
		BubbleInput.InputMoved += HandleTouchMove;
		BubbleInput.InputReleased += HandleTouchUp;

		CurrentBallScore = (int)Math.Pow(2, UnityEngine.Random.Range(1, 6));
		playerCell.Score = CurrentBallScore;
	}

    private void OnDestroy()
    {
		BubbleInput.InputMoved -= HandleTouchMove;
		BubbleInput.InputReleased -= HandleTouchUp;
	}

    private void HandleTouchUp(Vector2 touch)
	{
		if (hitPositions.Count < 2 || selectedCell == null)
			return;

		aimLine.positionCount = 0;

		BubbleInput.Instance.InputEnabled = false;
		AnimatePlayer(false);
		
		Bubbleshooter.Feedback.VibrationManager.VibrateSelect();
		BallInPosition = false;

		hitPositions[hitPositions.Count - 1] = selectedCell.transform.position;

		AudioManager.Instance.PlayClip(1);

		FiredPositions?.Invoke(hitPositions);
		playerCell.gameObject.SetActive(false);		

		CoroutineManager.Instance.WaitUntil(() => BallInPosition == true, () =>
		{
			selectedCell.Score = CurrentBallScore;
			selectedCell.State = CellState.Filled;
			Fired?.Invoke(selectedCell);

			selectedCell = null;	
			CurrentBallScore = (int)Math.Pow(2, UnityEngine.Random.Range(1, 6));
			playerCell.Score = CurrentBallScore;
			playerCell.gameObject.SetActive(true);
			
		});
	}

	private void HandleTouchMove(Vector2 touch)
	{		
		hitPositions.Clear();
			
		Vector2 mousePoint = Camera.main.ScreenToWorldPoint(touch);
		Vector2 direction = new Vector2(mousePoint.x - player.position.x, mousePoint.y - player.position.y);
		RaycastHit2D hit = Physics2D.Raycast(player.position, direction);

		if (hit.collider != null)
		{
			hitPositions.Add(new Vector3(player.position.x, player.position.y, 0));

			if (hit.collider.CompareTag("Wall"))
				RaycastWall(hit, direction);
			else
			{
				if (hit.collider.gameObject.GetComponent<GridCell>() != null)
					selectedCell = hit.collider.gameObject.GetComponent<GridCell>();

				hitPositions.Add(hit.point);
				UpdateLinerenderer();
			}
		}
		AnimatePlayer(true);
	}

	private void RaycastWall(RaycastHit2D previousHit, Vector2 directionIn) 
	{
		hitPositions.Add(previousHit.point);

		var normal = Mathf.Atan2(previousHit.normal.y, previousHit.normal.x);
		var newDirection = normal + (normal - Mathf.Atan2(directionIn.y, directionIn.x));
		var reflection = new Vector2(-Mathf.Cos(newDirection), -Mathf.Sin(newDirection));
		var newCastPoint = previousHit.point + (2 * reflection);

		var hit2 = Physics2D.Raycast(newCastPoint, reflection);
		if (hit2.collider != null)
		{
			if (hit2.collider.CompareTag("Wall"))
				RaycastWall(hit2, reflection);
			else
			{
				if (hit2.collider.gameObject.GetComponent<GridCell>() != null)
					selectedCell = hit2.collider.gameObject.GetComponent<GridCell>();

				hitPositions.Add(hit2.point); 
				UpdateLinerenderer();
			}
		}
		else
			UpdateLinerenderer();	
	}

    private void UpdateLinerenderer()
    {
		aimLine.positionCount = Math.Min(hitPositions.Count, 4);
        aimLine.SetPositions(hitPositions.ToArray());
    }

	private void AnimatePlayer(bool setAnimate)
    {
        if (!animating && setAnimate)
        {
			LeanTween.moveLocalX(playerCell.VisualiserTransform.gameObject, transform.position.x + 5, .1f).setLoopPingPong();
			animating = true;
		}
        if (!setAnimate == animating)
        {
			animating = false;
			LeanTween.cancel(playerCell.VisualiserTransform.gameObject);
			playerCell.VisualiserTransform.gameObject.transform.localPosition = Vector3.zero;
		}
		
	}

}
