using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixerGroup mainMixerGroup;

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
            setUpAudioSource(globalSounds[s.name], s);
        }

        PlayerSettings.updateSetting += onSettingsUpdate;
        setVolume();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(ambientSounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning($"{this}[AudioManager]: Sound {name} could not be found");
            return;
        }
        globalSounds[s.name].Play();
    }

    public Sound getPointSound(string name)
    {
        return Array.Find(pointSounds, sound => sound.name == name);
    }

    public void setUpAudioSource(AudioSource source, Sound sound)
    {

        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.outputAudioMixerGroup = mainMixerGroup;
    }

    void onSettingsUpdate(string key)
    {
        switch (key){
            case "volume":
                setVolume();
                break;
            default:
                break;
        }
    }
    void setVolume()
    {
        float volume = PlayerSettings.getPref("volume") - 80;
        mainMixerGroup.audioMixer.SetFloat("Volume", volume);
        Debug.Log($"{this}[AudioManager]: Volume: {volume}");
    }
}