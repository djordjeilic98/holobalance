using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMove : MonoBehaviour {

    public Transform[] positions;
    public int currentPosition = 0;

    private bool CanMove = true;

    public GameObject[] deflectors;

	// Use this for initialization
	void Start () {
        transform.position = positions[currentPosition].position;
        transform.rotation = positions[currentPosition].rotation;

        for (int i = 0; i < deflectors.Length; i++)
        {
            if (deflectors[i])
            {
                deflectors[i].SetActive(false);
            }
        }
        if (currentPosition <= deflectors.Length - 1)
        {
            if (deflectors[currentPosition])
            {
                deflectors[currentPosition].SetActive(true);
            }
        }
    }

	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move()
    {
        Debug.Log("Move: " + name + ", " + CanMove);
        if (CanMove)
        {
            currentPosition++;
            if (currentPosition >= positions.Length)
            {
                currentPosition = 0;
            }

            transform.position = positions[currentPosition].position;
            transform.rotation = positions[currentPosition].rotation;

            for (int i = 0; i < deflectors.Length; i++)
            {
                if (deflectors[i])
                {
                    deflectors[i].SetActive(false);
                }
            }
            if (currentPosition <= deflectors.Length - 1)
            {
                if (deflectors[currentPosition])
                {
                    deflectors[currentPosition].SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ball")
        {
            //Debug.Log("Move stop");
            //CanMove = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ball")
        {
            //Debug.Log("Move start");
            CanMove = true;
        }
    }
}
