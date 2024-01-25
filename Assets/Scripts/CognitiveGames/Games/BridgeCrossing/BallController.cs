using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public Vector3 forceVector;

    public int materialIndex;

    public float force;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Rigidbody>().AddForce(forceVector * Time.deltaTime);
	}

    public void StartRolling()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnDrawGizmos()
    {
        Vector3 relativeforceDirection = GetComponent<Rigidbody>().velocity;
        Vector3 endPoint = transform.position + relativeforceDirection * 0.1f;
        Debug.DrawLine(transform.position, endPoint, Color.blue);
        Debug.DrawLine(endPoint, endPoint + Quaternion.AngleAxis(-135, Vector3.up) * relativeforceDirection * 0.05f, Color.blue);
        Debug.DrawLine(endPoint, endPoint + Quaternion.AngleAxis(135, Vector3.up) * relativeforceDirection * 0.05f, Color.blue);
    }
}
