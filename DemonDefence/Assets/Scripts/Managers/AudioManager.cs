using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] pointSounds;
    public Sound[] ambientSounds;

    public Dictionary<string, AudioSource> globalSounds = new Dictionary<string, AudioSource>();

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
            globalSounds[s.name] = gameObject.AddComponent<AudioSource>();
            globalSounds[s.name].clip = s.clip;
            globalSounds[s.name].volume = s.volume;
            globalSounds[s.name].pitch = s.pitch;
            globalSounds[s.name].loop = s.loop;
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
        globalSounds[s.name].Play();
    }

    public Sound getPointSound(string name)
    {
        return Array.Find(pointSounds, sound => sound.name == name);
    }
}