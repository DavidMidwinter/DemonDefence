using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunSmoke : pointEffect
{
    [SerializeField] private ParticleSystem smoke, fire;
    public override void fireEffect()
    {
        {
            smoke.Play();
            fire.Play();
        }
    }
}
