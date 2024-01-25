using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazeButton : HoloKit.HoloKitGazeTarget {

    public UnityEvent OnAction;
    public bool OnlyDemo;

    [Tooltip("If Gaze Time is bigger than 0 then it will override the default gaze timer")]
    public float gazeTime;

	// Use this for initialization
	void Start () {
        if (OnlyDemo && !ConnectionManager.Instance().Standalone)
        {
            gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
