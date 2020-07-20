using UnityEngine;
using TMPro;

/// <summary>
/// Controls all aspects of the messages to the user
/// </summary>
public class MessageScreenController : MonoBehaviour
{
    /// <summary>
    /// Confetti particles to show when player did something positive
    /// </summary>
    [SerializeField]
    private ParticleSystem confetti;

    [SerializeField]
    private TextMeshProUGUI messageText;

    [SerializeField]
    private MessagePreset messages;

    private bool cooldown = false;

    public void ShowScreen(UserMessage userMessage)
    {
        if (cooldown)
            return;
        cooldown = true;

        if (userMessage == UserMessage.GoodJob || userMessage == UserMessage.LevelClear)
        {
            confetti.Play();
            AudioManager.Instance.PlayClip(4);
            messageText.text = userMessage == UserMessage.GoodJob ? messages.CongratulationsTexts[Random.Range(0, messages.CongratulationsTexts.Length)] : messages.PerfectText;
        }
        else if (userMessage == UserMessage.GameOver)
        {
            messageText.text = messages.GameoverTexts[Random.Range(0, messages.GameoverTexts.Length)];
            AudioManager.Instance.PlayClip(3);
        }
        AnimateInScreen();
    }

    private void AnimateInScreen()
    {
        LeanTween.moveLocal(messageText.gameObject, Vector3.zero, 1)
                    .setEase(LeanTweenType.easeOutQuart)
                    .setOnComplete(() =>
                    {
                        if (confetti.isPlaying)
                            confetti.Stop();

                        LeanTween.moveLocal(messageText.gameObject, Vector3.right * 1800, 1)
                                 .setDelay(1)
                                 .setEase(LeanTweenType.easeInQuart)
                                 .setOnComplete(() =>
                                 {
                                     messageText.gameObject.transform.localPosition += Vector3.left * 1800 * 2;
                                     cooldown = false;
                                 });
                    });
    }
}