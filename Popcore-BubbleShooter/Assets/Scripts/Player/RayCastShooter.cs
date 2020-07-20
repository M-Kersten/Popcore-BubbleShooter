using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RayCastShooter : SingletonUtility.Singleton<RayCastShooter>
{
	[SerializeField]
	private GameObject canvas;
	[SerializeField]
	private LineRenderer aimLine;

	[SerializeField]
	private Cell playerCell;

	[SerializeField]
	private Transform player;

	public bool BallInPosition;

	public Action<Cell> Fired;
	public Action<List<Vector3>> FiredPositions;

	public int CurrentBallScore;

	public bool ReadyToFire;

	private bool mouseDown = false;
	private List<Vector3> hitPositions = new List<Vector3>();

	private Cell selectedCell
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

	private Cell currentlySelected;
    private bool animating;

    private void Start()
    {
		CurrentBallScore = (int)Math.Pow(2, UnityEngine.Random.Range(1, 6));
		playerCell.Score = CurrentBallScore;
		ReadyToFire = true;
	}

    void Update()
	{
		if (!ReadyToFire)
			return;
#if !UNITY_EDITOR
		if (Input.touches.Length > 0)
		{
			Touch touch = Input.touches[0];

			if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
				HandleTouchUp(touch.position);
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Began)
				HandleTouchMove(touch.position);            

			return;
		}
#else
		if (Input.GetMouseButtonDown(0))
		{
			mouseDown = true;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			mouseDown = false;
			HandleTouchUp(Input.mousePosition);
		}
		if (mouseDown)
			HandleTouchMove(Input.mousePosition);
#endif
	}

	void HandleTouchUp(Vector2 touch)
	{
		if (hitPositions == null || hitPositions.Count < 2)
			return;

		AnimatePlayer(false);
		
		ReadyToFire = false;
		UnityTools.VibrationManager.VibrateSelect();
		BallInPosition = false;
		hitPositions[hitPositions.Count - 1] = selectedCell.transform.position;

		AudioManager.Instance.PlayClip(1);

		FiredPositions?.Invoke(hitPositions);
		playerCell.gameObject.SetActive(false);
		aimLine.positionCount = 0;

		CoroutineManager.Instance.WaitUntil(() => BallInPosition == true, () =>
		{
			selectedCell.Score = CurrentBallScore;
			selectedCell.State = CellState.Filled;
			Fired?.Invoke(selectedCell);

			UnityTools.VibrationManager.VibrateSelect();

			selectedCell = null;
			

			CurrentBallScore = (int)Math.Pow(2, UnityEngine.Random.Range(1, 6));
			playerCell.Score = CurrentBallScore;

			playerCell.gameObject.SetActive(true);
			ReadyToFire = true;
		});
	}

	void HandleTouchMove(Vector2 touch)
	{
		if (hitPositions == null)
			return;

		hitPositions.Clear();

		AnimatePlayer(true);

		Vector2 mousePoint = Camera.main.ScreenToWorldPoint(touch);
		Vector2 direction = new Vector2(mousePoint.x - player.position.x, mousePoint.y - player.position.y);
		RaycastHit2D hit = Physics2D.Raycast(player.position, direction);

		if (hit.collider != null)
		{
			hitPositions.Add(new Vector3(player.position.x, player.position.y, 0));

			if (hit.collider.CompareTag("SideWall"))
				RaycastWall(hit, direction);
			else
			{
				if (hit.collider.gameObject.GetComponent<Cell>() != null)
					selectedCell = hit.collider.gameObject.GetComponent<Cell>();

				hitPositions.Add(hit.point);
				DrawPaths();
			}
		}
	}

	void RaycastWall(RaycastHit2D previousHit, Vector2 directionIn) 
	{
		hitPositions.Add(previousHit.point);

		var normal = Mathf.Atan2(previousHit.normal.y, previousHit.normal.x);
		var newDirection = normal + (normal - Mathf.Atan2(directionIn.y, directionIn.x));
		var reflection = new Vector2(-Mathf.Cos(newDirection), -Mathf.Sin(newDirection));
		var newCastPoint = previousHit.point + (2 * reflection);

		var hit2 = Physics2D.Raycast(newCastPoint, reflection);
		if (hit2.collider != null)
		{
			if (hit2.collider.CompareTag("SideWall"))
				RaycastWall(hit2, reflection);
			else
			{
				if (hit2.collider.gameObject.GetComponent<Cell>() != null)
					selectedCell = hit2.collider.gameObject.GetComponent<Cell>();

				hitPositions.Add(hit2.point); 
				DrawPaths();
			}
		}
		else
			DrawPaths();	
	}

    void DrawPaths()
    {
		aimLine.positionCount = Math.Min(hitPositions.Count, 4);
        aimLine.SetPositions(hitPositions.ToArray());
    }

	private void AnimatePlayer(bool setAnimate)
    {
        if (!animating && setAnimate)
        {
			LeanTween.moveLocalX(playerCell.visualiserTransform.gameObject, transform.position.x + 5, .1f).setLoopPingPong();
			animating = true;
		}
        if (!setAnimate == animating)
        {
			animating = false;
			LeanTween.cancel(playerCell.visualiserTransform.gameObject);
			playerCell.visualiserTransform.gameObject.transform.localPosition = Vector3.zero;
		}
		
	}

}
