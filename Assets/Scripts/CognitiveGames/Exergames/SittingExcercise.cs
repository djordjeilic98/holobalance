using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SittingExcercise : AbstractScenarioController {
    public GameObject commonObjects;
    public Transform startPosition;
    public GameObject objectToMove;
    public GameObject centralObject;
    public GameObject framingObject;
    public GameObject directionObject;
    public float speed = 1;

    public GameObject pointsArray;
    private Transform[] points;
    private int currentPointIndex;

    public float gameDuration;

    public bool moving;

    Vector3 offset;

    public Camera trackingCamera;

    protected Plane trackingPlane;

    //0 - horizontal, 1 - vertical
    public int axis;

    private void Awake()
    {
        trackingPlane = new Plane(startPosition.forward, startPosition.position);
    }

    // Use this for initialization
    void Start () {
        //StartMoving();
        trackingCamera = Camera.main;
        ShowInstructionAnimation();
    }

    public Animator infoFrame;
    void ShowInstructionAnimation()
    {
        //infoFrame.gameObject.SetActive(true);
        string trigger = (gameId == "holobalance_exergame_s2_sitting_1") ? "StartHorizontal" : "StartVertical";
        infoFrame.SetTrigger(trigger);
        SetFrameInfoFocus(false);
    }

    void LockInstructionAnimation()
    {
        //infoFrame.gameObject.SetActive(false);
        infoFrame.SetTrigger("Stop");
        SetFrameInfoFocus(true);
    }

    public override void OnInstructions()
    {
        base.OnInstructions();
        if ((gameId == "holobalance_exergame_s2_sitting_1"))
        {
            Invoke("LockInstructionAnimation", 1.5f);
            Invoke("ShowInstructionAnimation", 8.5f);
        }
        else
        {
            LockInstructionAnimation();
            Invoke("ShowInstructionAnimation", 7.1f);
        }
    }

    public void SetFrameInfoFocus(bool inFocus)
    {
        Image[] lines = infoFrame.GetComponentsInChildren<Image>();
        foreach (Image line in lines)
        {
            line.color = inFocus ? GetComponent<TrackingObjectDetector>().focusColor : GetComponent<TrackingObjectDetector>().normalColor;
        }
    }

    Vector3 CalculateOffset()
    {
        Ray ray = new Ray(trackingCamera.transform.position, trackingCamera.transform.forward);
        float enter = 0.0f;
        if (trackingPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return startPosition.position - hitPoint;
        }
        return Vector3.zero;
    }


    float headTurnTime;
	// Update is called once per frame
	public override void Update () {
        base.Update();
		if (moving)
        {
            headTurnTime += Time.deltaTime;

            float distance = (points[currentPointIndex].position - objectToMove.transform.position).magnitude;
            if (distance < 0.1f)
            {
                //Debug.Log("Head turn time: " + headTurnTime);
                headTurnTime = 0;
                currentPointIndex++;
                if (currentPointIndex >= points.Length)
                {
                    currentPointIndex = 0;
                }
            }

            
            Vector3 direction = (points[currentPointIndex].position - objectToMove.transform.position).normalized;
            objectToMove.transform.position = objectToMove.transform.position + direction * speed * Time.deltaTime;
            //objectToMove.transform.LookAt(points[currentPointIndex].position);

            offset = CalculateOffset();
            //Debug.Log(offset);
            framingObject.transform.localPosition = offset;

            directionObject.transform.position = framingObject.transform.position + (centralObject.transform.position - framingObject.transform.position).normalized * 0.5f;
            directionObject.transform.LookAt(new Vector3(centralObject.transform.position.x, centralObject.transform.position.y, framingObject.transform.position.z));


        }
    }

    // 0 - left, 1 - right, 2 - up, 3 - down
    public int GetDirection()
    {
        if (axis == 0)
        {
            if (points[currentPointIndex].position.x < objectToMove.transform.position.x)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        } else
        {
            if (points[currentPointIndex].position.y > objectToMove.transform.position.y)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
    }

    public override void StartScenario(int progression)
    {
        base.StartScenario(progression);

        gameObject.SetActive(true);
        commonObjects.SetActive(true);

        switch (progression)
        {
            case 0:
                speed = 0.96f;
                break;
            case 1:
                speed = 1.36f;
                break;
            case 2:
                speed = 2.08f;
                break;
            case 3:
                speed = 3.2f;
                break;
        }

        
        //centralObject.GetComponent<GazeButton>().enabled = ConnectionManager.Instance().Standalone;

        //objectToMove.GetComponent<GazeButton>().enabled = true;
        objectToMove.transform.position = startPosition.position;
        framingObject.transform.localPosition = Vector3.zero;
        //objectToMove.transform.rotation = Quaternion.Euler(0, 90, 0);
        //objectToMove.transform.GetChild(0).GetComponent<Animator>().SetInteger("state", 0);
        moving = false;
        currentPointIndex = 0;
        List<Transform> children = new List<Transform>();
        pointsArray.transform.GetComponentsInChildren<Transform>(true, children);
        children.RemoveAt(0);
        points = children.ToArray();
        Debug.Log("Points:" + points.Length);

        if (points.Length >= 2) {
            Vector3 difference = points[0].position - points[1].position;
            axis = difference.x < difference.y ? 1 : 0;
            //Debug.Log(difference + ", " + axis);
        }

        GetComponent<TrackingObjectDetector>().UpdateGraphics(false);

        //RunScenario();
    }

    public override void RunScenario()
    {
        base.RunScenario();

        StartMoving();
    }

    public void TakeOff()
    {
        //objectToMove.transform.GetChild(0).GetComponent<Animator>().SetInteger("state", 1);

        //objectToMove.GetComponent<GazeButton>().enabled = false;

        Invoke("StartMoving", 0.5f);
        centralObject.GetComponent<GazeButton>().enabled = false;
    }

    public void StartMoving()
    {
        moving = true;
        GetComponent<TrackingObjectDetector>().StartTracking();

        if (ConnectionManager.Instance().Standalone)
        {
            Invoke("FinishDemo", gameDuration);
        }
    }

    public override void EndScenario()
    {
        base.EndScenario();

        GetComponent<TrackingObjectDetector>().StopTracking();
        CancelInvoke("FinishDemo");

        gameObject.SetActive(false);
        commonObjects.SetActive(false);
    }

    public void FinishDemo()
    {
        FinishScenario(true);
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);

        moving = false;
        GetComponent<TrackingObjectDetector>().StopTracking();
    }
}
