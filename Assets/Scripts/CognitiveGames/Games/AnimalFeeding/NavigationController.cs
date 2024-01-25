using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour {

    public Vector3 startPosition;
    public Transform target;
    public Transform targetOrientation;

    public float eatingDistance;

    public float stopDistance = 0.5f;

    float timeInState;

    public int timeFed;

    public AnimalFeeding animalFeeding;

    public enum STATES
    {
        IDLE,
        WALKING,
        EATING,
        WALKING_BACK,
        CHANGE_PLACE,
    }

    public STATES state;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        //GetComponent<NavMeshAgent>().

    }
	
	// Update is called once per frame
	void Update () {
        timeInState += Time.deltaTime;
        if (state == STATES.WALKING)
        {
            //Debug.Log("Distance: " + (transform.position - target.position).magnitude);
            if ((transform.position - target.position).magnitude < eatingDistance)
            {
                ChangeState(STATES.EATING);
            }
        }
        if (state == STATES.WALKING_BACK)
        {
            //Debug.Log("Distance back: " + (transform.position - startPosition).magnitude);
            if ((transform.position - startPosition).magnitude < stopDistance)
            {
                ChangeState(STATES.IDLE);
            }
        }
        if (state == STATES.CHANGE_PLACE)
        {
            //Debug.Log("Distance: " +gameObject.name + ", " + (transform.position - startPosition).magnitude);
            if ((transform.position - startPosition).magnitude < stopDistance)
            {
                ChangeState(STATES.IDLE);
            }
        }
        if (state == STATES.EATING && timeInState > 2)
        {
            ChangeState(STATES.WALKING_BACK);
        }
	}

    public void ChangeState(STATES newState)
    {
        timeInState = 0;
        switch (newState)
        {
            case STATES.IDLE:
                GetComponent<Animator>().SetInteger("state", 0);
                GetComponent<NavMeshAgent>().SetDestination(transform.position);
                break;
            case STATES.WALKING:
                timeFed++;
                animalFeeding.AnimalSelected(this);
                GetComponent<Animator>().SetInteger("state", 1);
                GetComponent<NavMeshAgent>().SetDestination(target.position);
                break;
            case STATES.EATING:
                transform.LookAt(targetOrientation);
                animalFeeding.Shuffle(gameObject);
                GetComponent<NavMeshAgent>().SetDestination(transform.position);
                GetComponent<Animator>().SetInteger("state", 2);
                break;
            case STATES.WALKING_BACK:
                animalFeeding.EnableInput(true);
                GetComponent<Animator>().SetInteger("state", 1);
                GetComponent<NavMeshAgent>().SetDestination(startPosition);
                break;
            case STATES.CHANGE_PLACE:
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
