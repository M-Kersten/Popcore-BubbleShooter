using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using SingletonUtility;
using System;

/// <summary>
/// A simple manager for coroutines, makes time-based logic easier in other classes
/// </summary>
public class CoroutineManager : Singleton<CoroutineManager>
{
    /// <summary>
    /// Waits for a given amount of seconds and then performs a callback
    /// </summary>
    public Coroutine Wait(float time, UnityAction callback) => StartCoroutine(WaitCoroutine(time, callback));
    /// <summary>
    /// Waits for a condition to be fullfilled, then performs a callback
    /// </summary>
    public void WaitUntil(Func<bool> predictor, UnityAction callback) => StartCoroutine(WaitUntilCoroutine(predictor, callback));

    /// <summary>
    /// The coroutine to wait for on the callback
    /// </summary>
    private IEnumerator WaitCoroutine(float time, UnityAction callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
    /// <summary>
    /// The coroutine to wait until for on the callback
    /// </summary>
    private IEnumerator WaitUntilCoroutine(Func<bool> predictor, UnityAction callback)
    {
        yield return new WaitUntil(predictor);
        callback?.Invoke();
    }
}
