using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kiteBreath : weaponEffect
{

    [SerializeField] private ParticleSystem fire, backblast1, backblast2;
    public override void fireEffect()
    {
        fire.Play();
        backblast1.Play();
        backblast2.Play();
    }
}
