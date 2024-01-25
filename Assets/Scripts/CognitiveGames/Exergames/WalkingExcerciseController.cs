using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingExcerciseController : AbstractScenarioController
{
    public GameObject commonObjects;

    public Transform startPoint;
    public Transform[] finishPoint;
    public Transform leavePoint;
    public GameObject objectToMove;
    public GameObject playerCamera;

    public Vector3 currentTarget;
    private int targetIndex;

    public float speed;
    public float lateralSpeed;

    public GameObject[] pointsArray;
    private Transform[] points;
    private int currentPointIndex;

    public float distanceFromPlayer;
    private bool firstPass;
    public GameObject flower2;
    private Vector3 desiredPosition;

    public float standaloneDuration;

    enum BirdState
    {
        HOVERING,
        FOLLOWING,
        FLYING,
        LEAVING
    }

    BirdState birdState;

    public void Start()
    {
        //testing
        //StartScenario(0);
        playerCamera = Camera.main.gameObject;
    }

    public Animator infoBird;
    public override void OnInstructions()
    {
        base.OnInstructions();
        if (gameId == "holobalance_exergame_s2_walking_1")
        {
            infoBird.SetTrigger("Straight");
        } else if (gameId == "holobalance_exergame_s2_walking_2")
        {
            infoBird.SetTrigger("Horizontal");
        }
        else if (gameId == "holobalance_exergame_s2_walking_3")
        {
            infoBird.SetTrigger("Vertical");
        }
        else if (gameId == "holobalance_exergame_s2_walking_4")
        {
            infoBird.SetTrigger("VShape");
        }

    }

    public override void StartScenario(int progression)
    {
        base.StartScenario(progression);

        birdState = BirdState.HOVERING;
        currentTarget = finishPoint[0].position;
        targetIndex = 0;

        switch (progression)
        {
            case 0:
                lateralSpeed = 0.96f;
                break;
            case 1:
                lateralSpeed = 1.36f;
                break;
            case 2:
                lateralSpeed = 2.08f;
                break;
            case 3:
                lateralSpeed = 3.2f;
                break;
        }

        firstPass = true;
        flower2.SetActive(false);
        gameObject.SetActive(true);
        commonObjects.SetActive(true);
        objectToMove.SetActive(true);
        objectToMove.GetComponent<Animator>().enabled = false;
        objectToMove.transform.position = startPoint.position;
        objectToMove.transform.rotation = startPoint.rotation;

        if (ConnectionManager.Instance().Standalone)
        {
            //objectToMove.GetComponent<GazeButton>().enabled = true;
        }

        if (pointsArray.Length > 0)
        {
            currentPointIndex = 0;
            List<Transform> children = new List<Transform>();

            pointsArray[0].transform.GetComponentsInChildren<Transform>(true, children);
            children.RemoveAt(0);
            points = children.ToArray();
            objectToMove.transform.position = points[0].transform.position;
        }
    }

    public override void RunScenario()
    {
        base.RunScenario();
        StartGame();
    }

    public override void EndScenario()
    {
        base.EndScenario();
        gameObject.SetActive(false);
        commonObjects.SetActive(false);
    }

    public void Win()
    {
        FinishScenario(true);
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);
    }

    public void StartGame()
    {
        birdState = BirdState.FOLLOWING;
        if (pointsArray != null && pointsArray.Length > 0)
        {
            birdState = BirdState.FLYING;
        }

        if (ConnectionManager.Instance().Standalone)
        {
            //objectToMove.GetComponent<GazeButton>().enabled = false;
        }

        if (ConnectionManager.Instance().Standalone)
        {
            Invoke("Win", standaloneDuration);
        }
    }

    public override void Update()
    {
        base.Update();
        switch (birdState) {
            case (BirdState.FOLLOWING):
                {
                    float distance = (currentTarget - objectToMove.transform.position).magnitude;
                    //Debug.Log("Player distance: " + distance);
                    if (distance < 0.5f)
                    {
                        //birdState = BirdState.SWITCHING_DIRECTION;
                        if (currentTarget == finishPoint[0].position)
                        {
                            currentTarget = finishPoint[1].position;
                            targetIndex = 1;
                        }
                        else
                        {
                            currentTarget = finishPoint[0].position;
                            targetIndex = 0;
                        }

                        if (firstPass)
                        {
                            firstPass = false;
                            flower2.SetActive(true);
                        }
                        //change direction
                        //moving = false;
                        //objectToMove.GetComponent<Animator>().enabled = true;
                        //Invoke("Win", 2.0f);
                    }

                    Vector3 direction = (currentTarget - playerCamera.transform.position).normalized;
                    Vector3 birdPosition = playerCamera.transform.position + direction * distanceFromPlayer;
                    desiredPosition = new Vector3(birdPosition.x, startPoint.position.y, birdPosition.z);
                    objectToMove.transform.position += (desiredPosition - objectToMove.transform.position).normalized * speed * Time.deltaTime;
                    objectToMove.transform.LookAt(currentTarget);

                    float playerDistance = (objectToMove.transform.position - playerCamera.transform.position).magnitude;
                    //Debug.Log("Player distance: " + playerDistance);

                    /*if (playerDistance < 0.5 || playerDistance > 1.3)
                    {
                        moving = false;
                        leaving = true;
                        Lose();
                    }*/

                    //offset = CalculateOffset();
                    //framingObject.transform.localPosition = offset;

                    //UpdateMove();

                    break;
                }
            case BirdState.FLYING:
                {
                    objectToMove.transform.LookAt(currentTarget);
                    UpdateMove();
                    break;
                }
            case BirdState.LEAVING:
                {
                    float distance = (leavePoint.position - objectToMove.transform.position).magnitude;
                    if (distance < 0.1f)
                    {
                        objectToMove.SetActive(false);
                    }
                    Vector3 direction = (leavePoint.position - objectToMove.transform.position).normalized;
                    objectToMove.transform.position = objectToMove.transform.position + direction * speed * Time.deltaTime * 5;
                    objectToMove.transform.LookAt(leavePoint.position);
                    break;
                }
        }
    }

    public void SwitchDirection(int pointsIndex)
    {
        if (birdState == BirdState.FLYING)
        {
            currentPointIndex = 0;
            List<Transform> children = new List<Transform>();

            pointsArray[pointsIndex].transform.GetComponentsInChildren<Transform>(true, children);
            children.RemoveAt(0);
            points = children.ToArray();
            objectToMove.transform.position = points[0].transform.position;

            if (currentTarget == finishPoint[0].position)
            {
                currentTarget = finishPoint[1].position;
                targetIndex = 1;
            }
            else
            {
                currentTarget = finishPoint[0].position;
                targetIndex = 0;
            }

            if (firstPass)
            {
                firstPass = false;
                flower2.SetActive(true);
            }
        }
    }

    public void UpdateMove()
    {
        if (points != null)
        {
            Vector3 waypointPosition = new Vector3(points[currentPointIndex].position.x, points[currentPointIndex].position.y, points[currentPointIndex].position.z);
            float distance = (waypointPosition - objectToMove.transform.position).magnitude;
            if (distance < 0.1f)
            {
                currentPointIndex++;
                if (currentPointIndex >= points.Length)
                {
                    currentPointIndex = 0;
                }
            }

            Vector3 direction = (waypointPosition - objectToMove.transform.position).normalized;
            objectToMove.transform.position = objectToMove.transform.position + direction * lateralSpeed * Time.deltaTime;
            //objectToMove.transform.LookAt(waypointPosition);
        }
    }

    public override void Recenter()
    {
        base.Recenter();
        currentTarget = finishPoint[targetIndex].position;
    }
}