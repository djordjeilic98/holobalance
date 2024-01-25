using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovingController : MonoBehaviour {

    public GameObject objectToMove;
    public Transform[] positions;
    public float speed;

    public float hideAfter;

    public UnityEvent OnFinish;
    public UnityEvent OnHide;

    private int currentPointIndex;
    private bool moving;

    // Use this for initialization
    void Start () {
		
	}

    public void StartMoving()
    {
        objectToMove.SetActive(true);
        currentPointIndex = 0;
        objectToMove.transform.position = positions[currentPointIndex].position;
        moving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            float distance = (positions[currentPointIndex].position - objectToMove.transform.position).magnitude;

            Vector3 direction = (positions[currentPointIndex].position - objectToMove.transform.position).normalized;
            objectToMove.transform.position = objectToMove.transform.position + direction * speed * Time.deltaTime;

            if (distance < 0.1f)
            {
                currentPointIndex++;
                if (currentPointIndex >= positions.Length)
                {
                    objectToMove.transform.position = positions[currentPointIndex - 1].position;
                    moving = false;
                    currentPointIndex = 0;
                    OnFinish.Invoke();
                    Invoke("Hide", hideAfter);
                }
            }

            
            //objectToMove.transform.LookAt(poi
        }
    }

    public void Hide()
    {
        objectToMove.SetActive(false);
        OnHide.Invoke();
    }
 }
