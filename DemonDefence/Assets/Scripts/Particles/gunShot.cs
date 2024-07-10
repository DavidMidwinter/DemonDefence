using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunSmoke : pointEffect
{
    /// Gun smoke effect - for handheld firearms
    [SerializeField] private ParticleSystem smoke, fire;
    public override void fireEffect()
    {
        {
            /// Fire all the particle systems in this effect
            smoke.Play();
            fire.Play();
        }
    }
}
