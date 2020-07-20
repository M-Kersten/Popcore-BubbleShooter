using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CellVisualiser : MonoBehaviour
{
    [SerializeField]
    CellSettings settings;

    [SerializeField]
    private TextMeshProUGUI countText;

    [SerializeField]
    private Image circle;

    [SerializeField]
    private ParticleSystem particles;

    private int score = 0;

    public void UpdateVisuals(int newScore)
    {
        score = newScore;
        countText.text = score.ToString();
        circle.color = settings.scoreColors[(int)Mathf.Log(score, 2) - 1];
        if (particles)
        {
            ParticleSystem.MainModule main = particles.main;
            main.startColor = circle.color;
        }
    }

    public void AnimateIn()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, .8f)
            .setEase(LeanTweenType.easeOutBack);
    }

    public void AnimateFallDown(int i, Action callback = null)
    {
        LeanTween.moveY(gameObject, -5, .8f)
                .setEase(LeanTweenType.easeInBack)
                .setDelay(i * .05f)
                .setOnComplete(() => callback?.Invoke());
    }

    public void AnimateOut(CellState state, Action callback = null)
    {
        float duration = .4f;
        if (state != CellState.Filled)
        {
            CoroutineManager.Instance.Wait(duration, () => callback?.Invoke());
        }
        else
        {
            particles.Play();
            LeanTween.scale(gameObject, Vector3.zero, 1)
                .setEase(LeanTweenType.easeOutQuart)
                .setOnComplete(() =>
                {
                    particles.Stop();
                    callback?.Invoke();
                });
        }
        
    }


    public void UpdateState(CellState state)
    {
        switch (state)
        {
            case CellState.Empty:
                    countText.text = string.Empty;
                    circle.color = Color.clear;
                break;
            case CellState.Preview:
                countText.text = string.Empty;
                circle.color = new Color(0, 0, 0, .2f);
                break;
            case CellState.Filled:
                UpdateVisuals(score);
                break;
            default:
                break;
        }
    }
}
