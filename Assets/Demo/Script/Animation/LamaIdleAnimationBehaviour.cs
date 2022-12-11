using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LamaIdleAnimationBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private int _numberOfIdleAnimations; 
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int idleAnimation = Random.Range(0, _numberOfIdleAnimations);
        animator.SetFloat("IdleBlendTree", idleAnimation);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}
}
