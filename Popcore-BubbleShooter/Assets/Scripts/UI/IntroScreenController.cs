using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroScreenController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup introCanvas;

    [SerializeField]
    ParticleSystem particles;

    [SerializeField]
    private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        AudioManager.Instance.PlayClip(2);
        particles.Play();
        LeanTween.alphaCanvas(introCanvas, 0, .5f)
            .setOnComplete(() =>
            {
                introCanvas.gameObject.SetActive(false);
                particles.Stop();
                RayCastShooter.Instance.ReadyToFire = true;
            })
            .setDelay(2);
    }

}
