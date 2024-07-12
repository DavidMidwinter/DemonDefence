using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    /// <summary>
    /// Handles animations for a given unit
    /// </summary>
    public BaseUnit unit;
    public Animator animator;
    public float animspeed;
    public weaponEffect weaponEffect;
    private List<(AudioSource sound, float defaultPitch)> footsteps = new List<(AudioSource, float)>();
    [SerializeField] private string[] footstepNames;
    private void Awake()
    {
        Debug.Log($"Set up {this}");
        unit.playAnimation += playAnimation;
        animator = gameObject.GetComponent<Animator>();
        animspeed = unit.movement_speed / 20;
        animator.SetFloat("WalkSpeed", animspeed);

        if (weaponEffect)
            weaponEffect.initialiseEffect();

        foreach (string name in footstepNames)
        {
            Sound effect = AudioManager.Instance.getPointSound(name);
            if (effect != null)
            {
                AudioSource sound = gameObject.AddComponent<AudioSource>();
                sound.clip = effect.clip;
                sound.volume = effect.volume;
                sound.loop = effect.loop;

                footsteps.Add((sound, effect.pitch));
            }
            else
            {
                Debug.LogWarning($"Sound {name} could not be found.");
            }
        }
        playAnimation(animations.Idle);
    }

    protected virtual void playAnimation(animations anim)
    {
        /// Play an animation
        /// Args:
        ///     animations anim: The animation to fire, defined in the enum animations
        Debug.Log($"{transform.parent.name}: {anim}");
        animator.SetTrigger(anim.ToString());
    }

    public void attackEffect()
    {
        /// If this weapon has a weapon particle effect, then fire that effect.
        if (weaponEffect)
        {
            weaponEffect.fireEffect();
        }
    }

    public void footstep()
    {
        if (footsteps.Count > 0)
        {
            (AudioSource sound, float defaultPitch) footstep = footsteps.OrderBy(s => Random.value).First();
            footstep.sound.pitch = footstep.defaultPitch * Random.Range(0.5f, 1.5f);
            footstep.sound.Play();
        }
    }
}

public enum animations
{
    Idle,
    Walk,
    Attack,
    Order
}