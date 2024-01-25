using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStatusStart : StateMachineBehaviour {

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        ((SceneControllerVirtualCoach)(SceneController.Instance)).currentAnimationPlaying = true;
        Debug.Log("Animation Start");
    }
}
