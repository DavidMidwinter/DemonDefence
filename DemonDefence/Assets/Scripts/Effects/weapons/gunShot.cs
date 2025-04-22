using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class gunShot : particleEffect
{
    /// Gun smoke effect - for handheld firearms
    [SerializeField] private ParticleSystem smoke, fire;
    [SerializeField] private string[] soundNames;

    [SerializeField] private List<AudioSource> gunshots = new List<AudioSource>();
    public override void initialiseEffect()
    {
        foreach(string name in soundNames)
        {
            Sound effect = AudioManager.Instance.getPointSound(name);
            if(effect != null)
            {
                AudioSource sound = gameObject.AddComponent<AudioSource>();
                AudioManager.Instance.setUpAudioSource(sound, effect);

                gunshots.Add(sound);
            }
            else
            {
                Debug.LogWarning($"{this}[gunShot]: Sound {name} could not be found.");
            }
        }
    }
    public override void fireEffect()
    {
        {
            /// Fire all the particle systems in this effect
            /// 
            Debug.Log($"{this}[gunShot]: firing effect");
            smoke.Play();
            fire.Play();
            if (gunshots.Count > 0)
                gunshots.OrderBy(s => Random.value).First().Play();
        }
    }
}
