using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class cannonShot : weaponEffect
{
    /// Gun smoke effect - for handheld firearms
    [SerializeField] private ParticleSystem smoke, fire, touchHoleBlast;
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
                sound.clip = effect.clip;
                sound.volume = effect.volume;
                sound.pitch = effect.pitch;
                sound.loop = effect.loop;

                gunshots.Add(sound);
            }
            else
            {
                Debug.LogWarning($"Sound {name} could not be found.");
            }
        }
    }
    public override void fireEffect()
    {
        {
            /// Fire all the particle systems in this effect
            /// 
            Debug.Log($"{this} firing effect");
            smoke.Play();
            fire.Play();
            touchHoleBlast.Play();
            if (gunshots.Count > 0)
                gunshots.OrderBy(s => Random.value).First().Play();
        }
    }
}
