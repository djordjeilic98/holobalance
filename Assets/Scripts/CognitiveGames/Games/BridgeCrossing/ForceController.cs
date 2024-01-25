using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceController : MonoBehaviour {

    public Vector3 forceDirection;
    public Vector3 relativeforceDirection;

    public bool IsLocal;

    // Use this for initialization
    void Start () {
        relativeforceDirection = IsLocal ? (transform.rotation * forceDirection).normalized : forceDirection;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ball")
        {
            //Debug.Log("AddForce: " + relativeforceDirection);
            other.gameObject.GetComponent<BallController>().forceVector = other.gameObject.GetComponent<BallController>().force * relativeforceDirection;
            //if (IsLocal)
            //{
            //    other.gameObject.GetComponent<BallController>().forceVector = new Vector3(other.gameObject.GetComponent<BallController>().forceVector.x, other.gameObject.GetComponent<BallController>().forceVector.y, other.gameObject.GetComponent<BallController>().forceVector.z);
            //}
        }
    }

    private void OnDrawGizmos()
    {
        relativeforceDirection = IsLocal ? (transform.rotation * forceDirection) : forceDirection;
        Vector3 endPoint = transform.position + relativeforceDirection * 0.1f + Vector3.up * 0.1f;
        Debug.DrawLine(transform.position + Vector3.up * 0.1f, endPoint, Color.red);
        Debug.DrawLine(endPoint, endPoint + Quaternion.AngleAxis(-135, Vector3.up) * relativeforceDirection * 0.05f, Color.red);
        Debug.DrawLine(endPoint, endPoint + Quaternion.AngleAxis(135, Vector3.up) * relativeforceDirection * 0.05f, Color.red);
    }
}
