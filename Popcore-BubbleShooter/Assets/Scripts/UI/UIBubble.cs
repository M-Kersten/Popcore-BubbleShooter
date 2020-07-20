using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBubble : MonoBehaviour
{
    void Start()
    {
        LeanTween.scale(gameObject, Vector3.one * .9f, UnityEngine.Random.Range(2, 2.4f)).setEase(LeanTweenType.easeInOutSine).setDelay(UnityEngine.Random.Range(0, 1)).setLoopPingPong();
    }
}
