using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class demonSwipe : weaponEffect
{
    [SerializeField] private string[] soundNames;
    private List<AudioSource> swipeSounds = new List<AudioSource>();
    public override void fireEffect()
    {
        /// Fire all the particle systems in this effect
        /// 
        Debug.LogWarning($"{this} firing effect");
        if (swipeSounds.Count > 0)
            swipeSounds.OrderBy(s => Random.value).First().Play();
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

                swipeSounds.Add(sound);
            }
            else
            {
                Debug.LogWarning($"Sound {name} could not be found.");
            }
        }
    }
}
