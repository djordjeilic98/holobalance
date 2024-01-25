using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleEnd : MonoBehaviour {

    public int materialIndex;

    public MarbleGame marbleGame;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ball")
        {
            marbleGame.BallFinished(other.gameObject.GetComponent<BallController>().materialIndex == materialIndex);
            Destroy(other.gameObject);
        }
    }
}
