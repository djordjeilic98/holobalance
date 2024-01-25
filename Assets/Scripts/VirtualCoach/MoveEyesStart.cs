using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEyesStart : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAt.isEnabled = true;
        Debug.Log("Eyes Enter");
    }
}
