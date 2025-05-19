using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class demonSwipe : particleEffect
{
    [SerializeField] private string[] soundNames;
    private List<AudioSource> swipeSounds = new List<AudioSource>();
    public override void fireEffect()
    {
        /// Fire all the particle systems in this effect
        /// 
        Debug.Log($"{this}[demonSwipe]: firing effect");
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
                AudioManager.Instance.setUpAudioSource(sound, effect);

                swipeSounds.Add(sound);
            }
            else
            {
                Debug.LogWarning($"{this}[demonSwipe]: Sound {name} could not be found.");
            }
        }
    }
}
