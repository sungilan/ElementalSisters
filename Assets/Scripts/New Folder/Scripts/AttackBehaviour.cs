using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is to check when attack animation is finished playing
/// </summary>
public class AttackBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    /// OnStateExit는 전환이 끝나고 상태 시스템이 이 상태 평가를 마치면 호출됩니다.
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("attack anim finished");

        animator.gameObject.transform.parent.GetComponent<ChampionAnimation>().OnAttackAnimationFinished();
    }

    // OnStateMove는 Animator.OnAnimatorMove() 직후에 호출됩니다.
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
