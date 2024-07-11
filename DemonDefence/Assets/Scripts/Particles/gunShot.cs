using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class gunShot : weaponEffect
{
    /// Gun smoke effect - for handheld firearms
    [SerializeField] private ParticleSystem smoke, fire;
    [SerializeField] private string[] soundNames;
    private List<Sound> gunshots = new List<Sound>();
    public override void initialiseEffect()
    {
        foreach(string name in soundNames)
        {
            Sound effect = AudioManager.Instance.getPointSound(name);
            if(effect != null)
            {
                effect.initialise(gameObject);
                gunshots.Add(effect);
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
            Debug.LogWarning($"{this} firing effect");
            smoke.Play();
            fire.Play();
            if (gunshots.Count > 0)
                gunshots.OrderBy(s => Random.value).First().source.Play();
        }
    }
}
