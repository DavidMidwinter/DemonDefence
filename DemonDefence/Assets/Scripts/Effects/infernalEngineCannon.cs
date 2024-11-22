using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infernalEngineCannon : weaponEffect
{
    [SerializeField]
    private ParticleSystem
        fire,
        smoke,
        backBlast1,
        backBlast2,
        backBlast3,
        backBlast4,
        backBlast5,
        backBlast6;

    [SerializeField] private string[] soundNames;

    [SerializeField] private AudioSource shot;
    public override void fireEffect()
    {
        backBlast1.Play();
        backBlast2.Play();
        backBlast3.Play();
        backBlast4.Play();
        backBlast5.Play();
        backBlast6.Play();
        fire.Play();
        smoke.Play();
        shot.Play();
    }

    public override void initialiseEffect()
    {
        foreach (string name in soundNames)
        {
            Sound effect = AudioManager.Instance.getPointSound(name);
            if (effect != null)
            {
                shot = gameObject.AddComponent<AudioSource>();
                AudioManager.Instance.setUpAudioSource(shot, effect);
            }
            else
            {
                Debug.LogWarning($"Sound {name} could not be found.");
            }
        }
    }
}
