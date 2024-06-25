using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public BaseUnit unit;
    public Animator animator;
    private void Awake()
    {
        unit.playAnimation += playAnimation;
        animator = gameObject.GetComponent<Animator>();
    }

    private void playAnimation(animations anim)
    {
        Debug.Log($"{transform.parent.name}: {anim.ToString()}");
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