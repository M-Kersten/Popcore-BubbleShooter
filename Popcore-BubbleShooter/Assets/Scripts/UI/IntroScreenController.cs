using UnityEngine;
using UnityEngine.UI;

namespace Bubbleshooter.UI
{
    /// <summary>
    /// Controller script for the starting screen
    /// </summary>
    public class IntroScreenController : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup introCanvas;

        [SerializeField]
        ParticleSystem particles;

        [SerializeField]
        private Button startButton;

        private void Start() => startButton.onClick.AddListener(StartGame);

        /// <summary>
        /// Fades from the start screen to the game
        /// </summary>
        private void StartGame()
        {
            AudioManager.Instance.PlayClip(2);
            particles.Play();
            LeanTween.alphaCanvas(introCanvas, 0, .5f)
                .setOnComplete(() =>
                {
                    introCanvas.gameObject.SetActive(false);
                    particles.Stop();
                    BubbleInput.Instance.InputEnabled = true;
                })
                .setDelay(2);
        }

    }
}