using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStatusEnd : StateMachineBehaviour {

	// OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0) &&
            ((SceneControllerVirtualCoach)(SceneController.Instance)).currentAnimationPlaying == true)
        {
            ((SceneControllerVirtualCoach)(SceneController.Instance)).currentAnimationPlaying = false;
            Debug.Log("End");
        }
    }

	// OnStateExit is called before OnStateExit is called on any state inside this state machine
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //((SceneControllerVirtualCoach)(SceneController.Instance)).currentAnimationPlaying = false;
    }
}
