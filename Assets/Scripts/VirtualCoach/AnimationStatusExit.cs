using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStatusExit : StateMachineBehaviour {

	// OnStateExit is called before OnStateExit is called on any state inside this state machine
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        ((SceneControllerVirtualCoach)(SceneController.Instance)).currentAnimationPlaying = false;
        Debug.Log("Animation Exit");
    }
}
