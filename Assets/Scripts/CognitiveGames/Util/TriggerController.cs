using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerController : MonoBehaviour {

    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    public string TriggerTag;
    public string TriggerName;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger enter: " + other.name);
        if (other.gameObject.tag.Equals(TriggerTag))
        {
            OnEnter.Invoke();
        }
        else
        {

            if (other.gameObject.name.Equals(TriggerName))
            {
                OnEnter.Invoke();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("Trigger exit: " + other.name);
        if (other.gameObject.tag.Equals(TriggerTag))
        {
            OnExit.Invoke();
        }
        else
        {

            if (other.gameObject.name.Equals(TriggerName))
            {
                OnExit.Invoke();
            }
        }
    }
}
