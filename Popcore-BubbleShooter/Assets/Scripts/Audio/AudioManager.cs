using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonUtility;

public class AudioManager : Singleton<AudioManager>
{
    #region Variables
    #region Editor
    [Header("SFX settings")]
    [SerializeField]
    [Range(0, 1)]
    private float globalEffectsVolume;
    /// <summary>
    /// The audioChannels used to fade from track to track.
    /// </summary>
    [SerializeField]
    private AudioSource effectSource;
    /// <summary>
    /// All of the sound effects the application uses.
    /// </summary>
    [Tooltip("All of the sound effects the application uses.")]
    [SerializeField]
    private List<SoundClip> soundClips;

    [Header("Music settings")]
    [SerializeField]
    [Range(0, 1)]
    private float globalMusicVolume;
    [SerializeField]
    private bool playMusicOnStart;
    /// <summary>
    /// The time to fade from one background track to another.
    /// </summary>
    [Tooltip("The time to fade from one background track to another.")]
    [SerializeField]
    private float musicFadeDuration;
    /// <summary>
    /// The AudioSource for SFX
    /// </summary>
    [SerializeField]
    private AudioSource[] musicChannels;
    /// <summary>
    /// All of the sound effects the application uses.
    /// </summary>
    [Tooltip("All of the sound effects the application uses.")]
    [SerializeField]
    private List<SoundClip> musicClips;    
    #endregion
    #endregion

    private void Start()
    {
        if (playMusicOnStart)
            PlayBackgroundTrack(0);        
    }


    /// <summary>
    /// Plays an audio clip
    /// </summary>
    /// <param name="clipIndex">The clip index of the clip you wish to play.</param>
    public void PlayClip(int clipIndex)
    {
        effectSource.pitch = soundClips[clipIndex].Pitch;
        effectSource.PlayOneShot(soundClips[clipIndex].sound, soundClips[clipIndex].Volume * globalEffectsVolume);
    }

    public void PlayClip(string clipName)
    {
        SoundClip playSound = soundClips.Find(x => x.sound.name == clipName);
        effectSource.PlayOneShot(playSound.sound, playSound.Volume * globalEffectsVolume);
    }

    public void PlayBackgroundTrack(int clipIndex)
    {
        if (musicChannels[musicChannels[0].clip != null ? 0 : 1].isPlaying && musicChannels[musicChannels[0].clip != null ? 0 : 1].clip == musicClips[clipIndex].sound)
            return;

        // pick the empty audiochannel and load the chosen track
        int chosenChannel = musicChannels[0].clip == null ? 0 : 1;
        musicChannels[chosenChannel].clip = musicClips[clipIndex].sound;
        musicChannels[chosenChannel].volume = 0;
        musicChannels[chosenChannel].Play();
        StartCoroutine(FadeTracks(chosenChannel));
    }

    private IEnumerator FadeTracks(int channelIndex)
    {
        // Set a timer
        float timer = 0;
        // Check which channel will be faded out
        int otherChannel = channelIndex == 1 ? 0 : 1;
        // Fade in the new channel and fade out the old one simultaniously
        while (timer <= musicFadeDuration)
        {
            timer += Time.deltaTime;
            musicChannels[channelIndex].volume = timer / musicFadeDuration * (musicClips[channelIndex].Volume * globalMusicVolume);
            musicChannels[otherChannel].volume = (1 - timer / musicFadeDuration) * (musicClips[otherChannel].Volume * globalMusicVolume);
            yield return null;
        }
        if (timer >= musicFadeDuration)
        {
            // remove the clip from the other channel            
            musicChannels[otherChannel].clip = null;
        }
        yield return null;
    }
}
