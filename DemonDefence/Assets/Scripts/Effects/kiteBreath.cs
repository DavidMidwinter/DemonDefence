using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class kiteBreath : weaponEffect
{

    [SerializeField] private ParticleSystem fire, backblast1, backblast2;

    [SerializeField] private string[] soundNames;

    [SerializeField] private AudioSource breath;
    public override void initialiseEffect()
    {
        foreach (string name in soundNames)
        {
            Sound effect = AudioManager.Instance.getPointSound(name);
            if (effect != null)
            {
                breath = gameObject.AddComponent<AudioSource>();
                AudioManager.Instance.setUpAudioSource(breath, effect);
            }
            else
            {
                Debug.LogWarning($"Sound {name} could not be found.");
            }
        }
    }
    public override void fireEffect()
    {
        /// Fire all the particle systems in this effect
        /// 
        Debug.Log($"{this} firing effect");
        fire.Play();
        backblast1.Play();
        backblast2.Play();
        breath.Play();
    }
}
