using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public BaseUnit unit;
    public Animator animator;
    public float animspeed;
    private void Awake()
    {
        unit.playAnimation += playAnimation;
        animator = gameObject.GetComponent<Animator>();
        animspeed = unit.movement_speed / 20;
        animator.SetFloat("WalkSpeed", animspeed);
        
        playAnimation(animations.Idle);
    }

    private void playAnimation(animations anim)
    {
        Debug.Log($"{transform.parent.name}: {anim}");
        animator.SetTrigger(anim.ToString());
    }
}

public enum animations
{
    Idle,
    Walk,
    Attack,
    Order
}