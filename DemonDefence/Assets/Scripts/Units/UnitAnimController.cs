using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimController : MonoBehaviour
{
    private Animator[] animators;

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    public void walk()
    {
        foreach (Animator animator in animators)
        {
            animator.SetTrigger("TrWalk");
        }
    }

    public void idle()
    {
        foreach (Animator animator in animators)
        {
            animator.SetTrigger("TrIdle");
        }
    }
}
