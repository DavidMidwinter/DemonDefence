using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class spearStrike : weaponEffect
{
    [SerializeField] private string[] soundNames;
    private List<AudioSource> strikeSounds = new List<AudioSource>();
    public override void fireEffect()
    {
        /// Fire all the particle systems in this effect
        /// 
        Debug.LogWarning($"{this} firing effect");
        if (strikeSounds.Count > 0)
            strikeSounds.OrderBy(s => Random.value).First().Play();
    }

    public override void initialiseEffect()
    {
        foreach (string name in soundNames)
        {
            Sound effect = AudioManager.Instance.getPointSound(name);
            if (effect != null)
            {
                AudioSource sound = gameObject.AddComponent<AudioSource>();
                sound.clip = effect.clip;
                sound.volume = effect.volume;
                sound.pitch = effect.pitch;
                sound.loop = effect.loop;

                strikeSounds.Add(sound);
            }
            else
            {
                Debug.LogWarning($"Sound {name} could not be found.");
            }
        }
    }
}
