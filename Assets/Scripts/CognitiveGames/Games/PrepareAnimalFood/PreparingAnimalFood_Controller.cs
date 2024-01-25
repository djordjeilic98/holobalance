using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PreparingAnimalFood_Controller : MonoBehaviour {

    private Vector3 startPosition;
    public Transform target;
    public GameObject food;

    public float eatingDistance;

    float timeInState;

    public enum STATES
    {
        WALKING,
        WAITING_FOR_FOOD,
        EATING,
        WALKING_BACK
    }

    public STATES state;

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
        ChangeState(STATES.WALKING);
        //GetComponent<NavMeshAgent>().

    }

    // Update is called once per frame
    void Update()
    {
        timeInState += Time.deltaTime;
        if (state == STATES.WALKING)
        {
            //Debug.Log("Distance: " + (transform.position - target.position).magnitude);
            if ((transform.position - target.position).magnitude < eatingDistance)
            {
                ChangeState(STATES.WAITING_FOR_FOOD);
            }
        }
        if (state == STATES.WALKING_BACK)
        {
            Debug.Log("Distance back: " + (transform.position - startPosition).magnitude);
            if ((transform.position - startPosition).magnitude < 0.5)
            {
                gameObject.SetActive(false);
            }
        }
        if (state == STATES.EATING && timeInState > 2)
        {
            food.SetActive(false);
            ChangeState(STATES.WALKING_BACK);
        }
    }

    public void ChangeState(STATES newState)
    {
        timeInState = 0;
        switch (newState)
        {
            case STATES.WALKING:
                GetComponent<Animator>().SetInteger("state", 1);
                GetComponent<NavMeshAgent>().SetDestination(target.position);
                break;
            case STATES.WAITING_FOR_FOOD:
                GetComponent<NavMeshAgent>().SetDestination(transform.position);
                GetComponent<Animator>().SetInteger("state", 0);
                break;
            case STATES.EATING:
                GetComponent<NavMeshAgent>().SetDestination(transform.position);
                GetComponent<Animator>().SetInteger("state", 2);
                break;
            case STATES.WALKING_BACK:
                GetComponent<Animator>().SetInteger("state", 1);
                GetComponent<NavMeshAgent>().SetDestination(startPosition);
                break;
        }
        state = newState;
    }

    public void ChangeState(int newState)
    {
        ChangeState((STATES)newState);
    }
}
