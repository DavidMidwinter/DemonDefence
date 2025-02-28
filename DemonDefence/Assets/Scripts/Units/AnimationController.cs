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
    public particleEffect weaponEffect, deathEffect;
    private bool isActive;
    private List<(AudioSource sound, float defaultPitch)> footsteps = new List<(AudioSource, float)>();
    [SerializeField] private string[] footstepNames;
    [SerializeField] private int maxDelay;
    private void Awake()
    {
        Debug.Log($"Set up {this}");
        unit.playAnimation += playAnimation;
        PlayerSettings.updateSetting += settingUpdate;
        animator = gameObject.GetComponent<Animator>();
        updateAnimationSpeed();

        if (weaponEffect)
            weaponEffect.initialiseEffect();
        if (deathEffect)
            deathEffect.initialiseEffect();

        foreach (string name in footstepNames)
        {
            Sound effect = AudioManager.Instance.getPointSound(name);
            if (effect != null)
            {
                AudioSource sound = gameObject.AddComponent<AudioSource>();
                AudioManager.Instance.setUpAudioSource(sound, effect);

                footsteps.Add((sound, effect.pitch));
            }
            else
            {
                Debug.LogWarning($"Sound {name} could not be found.");
            }
        }
        isActive = true;
        playAnimation(animations.Idle);
    }

    private void OnDestroy()
    {
        unit.playAnimation -= playAnimation;
        PlayerSettings.updateSetting -= settingUpdate;
    }

    public void playDeathAnimation()
    {
        Debug.LogWarning($"{this} playing death manually");
        isActive = false;
        animator.SetTrigger(animations.Death.ToString());
    }
    protected virtual void playAnimation(animations anim)
    {
        /// Play an animation
        /// Args:
        ///     animations anim: The animation to fire, defined in the enum animations
        Debug.Log($"{transform.parent.name}: {anim}");
        if (!gameObject.activeInHierarchy) {
            Debug.Log($"{this} inactive in hierarchy");
            return; 
        }
        if (!isActive)
        {
            Debug.Log($"{this} isActive value is false");
            return; 
        }
        if (maxDelay > 0 && anim == animations.Attack)
        {
            Debug.Log($"{this} is playing a delayed Attack");
            StartCoroutine(delayedAttack());
            return;
        }
        animator.SetTrigger(anim.ToString());
        Debug.Log($"Played {anim}");
    }

    protected void settingUpdate(string key)
    {
        if(key == "playback-speed")
        {
            updateAnimationSpeed();
        }
    }

    protected virtual void updateAnimationSpeed()
    {
        float playback = PlayerSettings.getPlaybackSpeed();
        animspeed = (unit.movement_speed / 20) * playback;
        animator.SetFloat("WalkSpeed", animspeed);
        animator.SetFloat("AnimationSpeed", playback);

    }

    private IEnumerator delayedAttack()
    {
        int frameDelay = (int) (Random.Range(0, maxDelay) * PlayerSettings.getPlaybackSpeed());
        while (frameDelay > 0)
        {
            yield return null;
            frameDelay--;
        }

        animator.SetTrigger(animations.Attack.ToString());
    }

    public void attackEffect()
    {
        /// If this weapon has a weapon particle effect, then fire that effect.
        Debug.Log($"{this} playing attack effect");
        if (weaponEffect)
        {
            weaponEffect.fireEffect();
        }
    }

    public void deathParticleEffect()
    {
        Debug.Log($"{this} playing death effect");
        if (deathEffect)
            deathEffect.fireEffect();
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

    public void finishDeathAnimation()
    {
        StartCoroutine(hideBody());
    }

    private IEnumerator hideBody()
    {
        float pauseEndTime = Time.time + 2f;
        while (Time.time < pauseEndTime)
        {
            yield return 0;
        }
        gameObject.SetActive(false);
    }
    
}

public enum animations
{
    Idle,
    Walk,
    Attack,
    Order,
    SecondMode,
    LeaveSecondMode,
    Brace,
    Death
    
}