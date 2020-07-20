using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "messages", menuName = "Settings/messages")]
public class MessagePreset : ScriptableObject
{
    public string PerfectText;

    public string[] CongratulationsTexts;

    public string[] GameoverTexts;
}
