using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageScreenController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem confetti;

    [SerializeField]
    private TextMeshProUGUI messageText;

    [SerializeField]
    private string[] congratulationTexts;

    [SerializeField]
    private string[] gameOverTexts;

    public void ShowScreen(UserMessage userMessage)
    {        
        if (userMessage == UserMessage.LevelClear)
        {
            messageText.text = congratulationTexts[Random.Range(0, congratulationTexts.Length)];
            confetti.Play();
        }
        else
        {
            messageText.text = gameOverTexts[Random.Range(0, gameOverTexts.Length)];
        }
        
        LeanTween.moveLocal(messageText.gameObject, Vector3.zero, 1)
            .setEase(LeanTweenType.easeOutQuart)
            .setOnComplete(()=>
            {
                if (confetti.isPlaying)                
                    confetti.Stop();

                LeanTween.moveLocal(messageText.gameObject, Vector3.left * 1800, 1)
                         .setDelay(1)
                         .setEase(LeanTweenType.easeInQuart);                
            });
    }
}
