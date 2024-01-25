using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackingObjectDetector : MonoBehaviour {

    public GameObject objectToTrack;
    public GameObject referentObject;

    public GameObject frameObject;

    public GameObject arrow;
    public Transform[] arrowPoints;

    private bool tracking;

    private float totalTime;
    private float goodTime;

    public float trackingTollerance;

    public Color normalColor;
    public Color focusColor;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
        if (tracking)
        {
            //Vector3 screenPosition = Camera.main.WorldToScreenPoint(objectToTrack.transform.position);
            //float cX = Mathf.Abs((Screen.width / 2 - screenPosition.x) / Screen.width); //percentage missed
            //float cY = Mathf.Abs((Screen.height / 2 - screenPosition.y) / Screen.height);

            //bool inFocus = (screenPosition.y > 0 && cX < 0.3 && cY < 0.3);

            //totalTime += Time.deltaTime;
            //goodTime += inFocus ? Time.deltaTime : 0;
            Vector3 diffenceVector = new Vector3(referentObject.transform.position.x, referentObject.transform.position.y, 0) - new Vector3(objectToTrack.transform.position.x, objectToTrack.transform.position.y, 0);
            float distance = diffenceVector.magnitude;
            //Debug.Log("Distance: " + distance);
            totalTime += Time.deltaTime;
            UpdateGraphics(distance < trackingTollerance);
            if (distance < trackingTollerance)
            {
                goodTime += Time.deltaTime;
            }

            int direction = GetComponent<SittingExcercise>().GetDirection();
            arrow.transform.position = arrowPoints[direction].position;
            if (direction == 0)
            {
                arrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
            } else if (direction == 1)
            {
                arrow.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else if (direction == 2)
            {
                arrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (direction == 3)
            {
                arrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }

            //arrow.transform.localScale = new Vector3(arrow.transform.localScale.x, distance, arrow.transform.localScale.z);

            int offsetDirection = 0;
            if (GetComponent<SittingExcercise>().axis == 0)
            {
                if (objectToTrack.transform.position.x < referentObject.transform.position.x)
                {
                    offsetDirection = 0;
                }
                else
                {
                    offsetDirection = 1;
                }
            } else
            {
                if (objectToTrack.transform.position.y > referentObject.transform.position.y)
                {
                    offsetDirection = 2;
                }
                else
                {
                    offsetDirection = 3;
                }
            }
            if (offsetDirection != direction)
            {
                arrow.SetActive(false);
            } else
            {
                arrow.SetActive(true);
            }
            //arrow.transform.position = objectToTrack.transform.position + diffenceVector;

            //Debug.Log("Offset: " + Camera.main.WorldToViewportPoint(objectToTrack.transform.position));

            //Debug.Log("Offset: " + cX + ", " + cY + ", " + screenPosition.z);
        }
    }

    public void UpdateGraphics(bool inFocus)
    {
        Image[] lines = frameObject.GetComponentsInChildren<Image>();
        foreach(Image line in lines)
        {
            line.color = inFocus ? focusColor : normalColor;
        }
    }

    public void StartTracking()
    {
        tracking = true;
        totalTime = 0;
        goodTime = 0;
    }

    public void StopTracking()
    {
        tracking = false;
    }

    public int Score()
    {
        return (int)((goodTime / totalTime) * 100);
    }
}
