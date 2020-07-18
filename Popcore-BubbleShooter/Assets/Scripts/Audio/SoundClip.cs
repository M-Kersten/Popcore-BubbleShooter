using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundClip
{
    public AudioClip sound;
    /// <summary>
    /// The volume of the clip.
    /// </summary>
    [Range(0f, 1f)] public float Volume;
    [Range(-3f, 3f)] public float Pitch;
}
