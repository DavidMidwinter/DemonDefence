using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] pointSounds;
    public Sound[] ambientSounds;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in ambientSounds)
        {
            s.initialise(gameObject);
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(ambientSounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning($"Sound {name} could not be found");
            return;
        }
        s.source.Play();
    }

    public Sound getPointSound(string name)
    {
        return Array.Find(pointSounds, sound => sound.name == name);
    }
}