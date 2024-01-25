using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEyesEnd : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAt.isEnabled = false;
        Debug.Log("Eyes Exit");
    }
}
