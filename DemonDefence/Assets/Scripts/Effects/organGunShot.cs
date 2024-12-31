using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class organGunShot : weaponEffect
{
    /// Gun smoke effect - for handheld firearms
    [SerializeField] private ParticleSystem touchHoleBlast;

    [SerializeField] public List<organBarrel> barrels = new List<organBarrel>();
    [SerializeField] private string[] soundNames;

    [SerializeField] private List<AudioSource> gunshots = new List<AudioSource>();
    public override void initialiseEffect()
    {
        foreach (string name in soundNames)
        {
            Sound effect = AudioManager.Instance.getPointSound(name);
            if (effect != null)
            {
                AudioSource sound = gameObject.AddComponent<AudioSource>();
                AudioManager.Instance.setUpAudioSource(sound, effect);

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
            touchHoleBlast.Play();

            StartCoroutine(volley());
        }
    }

    private IEnumerator volley()
    {
        int delay;
        foreach (organBarrel barrel in barrels)
        {

            barrel.Play();
            if (gunshots.Count > 0)
                gunshots.OrderBy(s => UnityEngine.Random.value).First().Play();
            delay = UnityEngine.Random.Range(3, 5);
            for (int i = 0; i < delay; i++)
            {
                yield return null;
            }
        }
    }
}

[Serializable]
public class organBarrel
{
    [SerializeField] public ParticleSystem smoke;
    [SerializeField] public ParticleSystem fire;
    public void Play()
    {
        smoke.Play();
        fire.Play();
    }

}