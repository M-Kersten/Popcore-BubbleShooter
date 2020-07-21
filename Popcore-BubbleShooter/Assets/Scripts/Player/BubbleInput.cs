using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for handling player input
/// Todo: make this an interface with platform dependent input manager deriving from it for release
/// </summary>
public class BubbleInput : SingletonUtility.Singleton<BubbleInput>
{
	private bool mouseDown = false;
	[HideInInspector]
	public bool InputEnabled;

	public static Action<Vector2> InputReleased;
	public static Action<Vector2> InputMoved;

	/// <summary>
	/// Check whether input is allowed and if it is call get input
	/// </summary>
	private void Update()
    {
		if (!InputEnabled)
			return;
		GetInput();
	}

	/// <summary>
	/// Get touch input and call the relevant events when a touch or mouseclick is detected
	/// </summary>
	private void GetInput()
	{
#if !UNITY_EDITOR
		if (Input.touches.Length > 0)
		{
			Touch touch = Input.touches[0];

			if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
				InputReleased(touch.position);
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Began)
			{	
				InputMoved(touch.position);
				if(touch.phase == TouchPhase.Began)
					Bubbleshooter.Feedback.VibrationManager.VibrateSelect();
			}
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
			InputReleased(Input.mousePosition);
		}
		if (mouseDown)
			InputMoved(Input.mousePosition);
#endif
	}
}
