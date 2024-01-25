using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour {

    public Vector3 startPosition;
    public Vector3 endPosition;

    public Vector3 startScale;
    public Vector3 endScale;

    public Color startColor = Color.white;
    public Color endColor = Color.white;

    public float transitionSpeed = 8;

    private float transitionFactor;

    private bool isDisabled;

    public enum STATE
    {
        NONE,
        RUNNING
    }

    private STATE state;

    public STATE GetState()
    {
        return state;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (STATE.RUNNING == state)
        {
            transitionFactor += Time.deltaTime * transitionSpeed;

            if (transitionFactor > 1)
            {
                transitionFactor = 1;
                GetComponent<GazeButton>().enabled = !isDisabled;
                state = STATE.NONE;
            }

            gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, transitionFactor);
            gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, transitionFactor);

            if (GetComponent<Image>())
            {
                GetComponent<Image>().color = Color.Lerp(startColor, endColor, transitionFactor);
            }
        }
	}

    public void StartTransition(Vector3 startPosition, Vector3 endPosition, bool isDisabled = false)
    {
        this.isDisabled = isDisabled;
        GetComponent<GazeButton>().enabled = false;
        this.startPosition = startPosition;
        this.endPosition = endPosition;

        ChangeState(STATE.RUNNING);
    }

    public void ChangeState(STATE newState)
    {
        if (state != newState) {
            state = newState;
            switch (state) {
                case STATE.RUNNING:
                    transitionFactor = 0;
                    transform.position = startPosition;
                    transform.localScale = startScale;
                    break;
            }
        }
    }
}
