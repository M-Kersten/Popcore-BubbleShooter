using UnityEngine;

namespace Bubbleshooter.UI
{
    /// <summary>
    /// Script for scaling a gameobject in a smooth repeating fashion
    /// </summary>
    public class UIBubble : MonoBehaviour
    {
        /// <summary>
        /// Scale to animate towards and from
        /// </summary>
        private float scale = .9f;

        private void OnEnable()
        {
            LeanTween.scale(gameObject, Vector3.one * scale, Random.Range(2, 2.4f))
                .setEase(LeanTweenType.easeInOutSine)
                .setDelay(Random.Range(0, 1))
                .setLoopPingPong();
        }

        private void OnDisable() => LeanTween.cancel(gameObject);
    }
}