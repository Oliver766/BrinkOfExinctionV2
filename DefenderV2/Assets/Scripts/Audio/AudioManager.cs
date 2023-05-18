using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Script by Matthew Harris
// SID 1808854

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup soundMixer;

    private Dictionary <string, Coroutine> delayedSounds = new Dictionary <string, Coroutine>();
    private List<string> currentlyPlaying = new List<string>();

    private void Awake()
    {
        // Ensure there is only one existing AudioManager at a time

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Set up each sound from the array as an AudioSource

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.isMusic ? musicMixer : soundMixer;
        }
    }

    /// <summary>
    /// Cancel a specific sound queued to play
    /// </summary>
    /// <param name="name">The name of the sound to cancel</param>
    public void CancelPlayWithDelay(string name)
    {
        if (delayedSounds.ContainsKey(name))
        {
            Coroutine coroutine = delayedSounds[name];
            StopCoroutine(coroutine);

            delayedSounds.Remove(name);
        }
    }

    /// <summary>
    /// Cancel all sounds queued to play
    /// </summary>
    public void CancelAllPlayWithDelay()
    {
        StopAllCoroutines();

        delayedSounds.Clear();
    }

    /// <summary>
    /// Play a sound after a specific delay
    /// </summary>
    /// <param name="name">The name of the sound to play</param>
    /// <param name="delay">The length of the delay</param>
    public void PlayWithDelay(string name, float delay)
    {
        // Store the coroutine in case it needs cancelling later
        Coroutine coroutine = StartCoroutine(CoPlayWithDelay(name, delay));

        delayedSounds.Add(name, coroutine);
    }

    private IEnumerator CoPlayWithDelay(string name, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (delayedSounds.ContainsKey(name))
        {
            delayedSounds.Remove(name);
        }

        Play(name);
    }

    /// <summary>
    /// Play a sound matching a string
    /// </summary>
    /// <param name="name"> The name of the sound to play </param>
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
        currentlyPlaying.Add(name);
    }

    /// <summary>
    /// Stop the sound matching a string from playing
    /// </summary>
    /// <param name="name"> The name of the sound to stop </param>
    public void Stop(string name)
    {
        if (currentlyPlaying.Contains(name))
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null || s.source == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }

            s.source.Stop();
            currentlyPlaying.Remove(name);
        }
    }

    /// <summary>
    /// Fade out a sound
    /// </summary>
    /// <param name="name">The name of the sound</param>
    /// <param name="time">The length of time to fade out over</param>
    public void FadeOut(string name, float time)
    {
        StartCoroutine(CoFadeOut(name, time));
    }

    private IEnumerator CoFadeOut(string name, float time)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s != null)
        {
            for (int i = 0; i < 10; i++)
            {
                s.source.volume = Mathf.Lerp(s.volume, 0, i / 10f);

                yield return new WaitForSeconds(time/10f);
            }

            Stop(name);
            s.source.volume = s.volume;
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }

    /// <summary>
    /// Stop all currently playing sounds
    /// </summary>
    public void StopAllSounds()
    {
        foreach (Sound sound in sounds)
        {
            Stop(sound.name);
        }
    }

    /// <summary>
    /// Stop all sound effects
    /// </summary>
    public void StopAllSoundEffects()
    {
        foreach (Sound sound in sounds)
        {
            if (!sound.isMusic)
            {
                Stop(sound.name);
            }
        }
    }

    /// <summary>
    /// Pause or unpause all currently playing sounds
    /// </summary>
    /// <param name="unPause">Whether to pause or unpause</param>
    public void PauseAllSounds(bool unPause)
    {
        foreach(Sound sound in sounds)
        {
            if (unPause)
            {
                sound.source.UnPause();
            }
            else
            {
                sound.source.Pause();
            }
        }
    }

    /// <summary>
    /// Check if the sound matching a string is currently playing
    /// </summary>
    /// <param name="name"> The name of the sound to check</param>
    /// <returns></returns>
    public bool IsSoundPlaying(string name)
    {
        return currentlyPlaying.Contains(name);
    }

    /// <summary>
    /// Get the full length of a sound
    /// </summary>
    /// <param name="name">The name of the sound</param>
    /// <returns></returns>
    public float GetSoundLength(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s != null)
        {
            return s.source.clip.length;
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return 0;
        }
    }
}
