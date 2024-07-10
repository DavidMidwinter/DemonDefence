using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedAttackAnimation : AnimationController
{
    /// <summary>
    /// Extension to base Animation Controller, to apply a short delay to the attack animation
    /// </summary>
    private int frameDelay;
    [SerializeField] private int maxDelay;

    protected override void playAnimation(animations anim)
    {
        if (anim == animations.Attack) StartCoroutine(delayedAttack());
        else base.playAnimation(anim);
    }
    private IEnumerator delayedAttack()
    {
        frameDelay = Random.Range(0, maxDelay);
        while(frameDelay > 0)
        {
            yield return null;
            frameDelay--;
        }

        animator.SetTrigger(animations.Attack.ToString());
    }
}
