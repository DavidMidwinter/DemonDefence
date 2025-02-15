using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demonDeath : particleEffect
{
    [SerializeField] private ParticleSystem mainExplosion;
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private string deathSound;

    private AudioSource deathSoundSource;

    public override void initialiseEffect()
    {
        Sound effect = AudioManager.Instance.getPointSound(deathSound);
        if (effect != null)
        {
            deathSoundSource = gameObject.AddComponent<AudioSource>();
            AudioManager.Instance.setUpAudioSource(deathSoundSource, effect);
        }
    }

    public override void fireEffect()
    {
        mainExplosion.Play();
        smoke.Play();
        deathSoundSource.Play();
    }
}
