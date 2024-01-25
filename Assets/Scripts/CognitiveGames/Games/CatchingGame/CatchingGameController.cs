using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CathingGameParameters
{
    public float timeBetweenSpawn = 4;
    public int objectsToSpawn = 10;
    public float fallingSpeed = 1;
    public float maxDistanceBetweenApples = 60;
}

public class CatchingGameController : AbstractScenarioController {

    [Header("Parameters")]
    public CathingGameParameters parameters;

    [Space(10)]
    [Header("Settings")]
    public GameObject fallingPrefab;
    public GameObject[] fallingObjects;
    public GameObject fallingCenter;
    public float fallingRadius;
    public GameObject basket;

    public Camera trackingCamera;

    protected Vector3 basketStartPosition;
    protected Plane basketPlane;

    protected int objectsTotal;
    protected int objectsPicked;

    public List<GameObject> spawnedObjects = new List<GameObject>();

    public GameObject[] applesInBasket;

    private int lastFallingApple = -1;

    private Vector3 previousSpawnPosition;

    //results for sending
    List<float> DistanceToPrevious = new List<float>();
    List<bool> CorrectAnswer = new List<bool>();

    // Use this for initialization
    void Awake () {
        basketStartPosition = basket.transform.position;
        InitPlane();
    }

    public void Start()
    {
        trackingCamera = Camera.main;
    }

    private void InitPlane()
    {
        Debug.Log("InitPlanes: " + fallingCenter.transform.forward + ", " + fallingCenter.transform.position);
        basketPlane = new Plane(fallingCenter.transform.forward, fallingCenter.transform.position);
    }

    void DrawPlane(Vector3 position, Vector3 normal)
    {

        Vector3 v3;

        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;

        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }
    private void OnDrawGizmos()
    {
        DrawPlane(fallingCenter.transform.position, basketPlane.normal);
    }

    public override void StartScenario(int progression)
    {
        base.StartScenario(progression);

        parameters = ProgressionLoader.Instance.gameParameters.catching_game[progression];

        lastFallingApple = -1;
        //basket.GetComponent<GazeButton>().enabled = true;

        objectsTotal = 0;
        objectsPicked = 0;

        gameObject.SetActive(true);

        //basket.transform.position = basketStartPosition;

        foreach (GameObject apple in applesInBasket)
        {
            apple.SetActive(false);
        }

        DistanceToPrevious.Clear();
        CorrectAnswer.Clear();

        previousSpawnPosition = fallingObjects[14].transform.position;
    }

    public override void RunScenario()
    {
        base.RunScenario();
        StartGame();
    }

    public void StartGame()
    {
        //Debug.Log("Idle: " + Animator.StringToHash("Idle"));
        //Debug.Log("GreenRedApple: " + Animator.StringToHash("GreenRedApple"));
        //Debug.Log("AppleGrow: " + Animator.StringToHash("AppleGrow"));

        //basket.GetComponent<GazeButton>().enabled = false;
        SpawnObject();
    }

    public void FinishGame()
    {
        FinishScenario(objectsPicked > (objectsTotal / 2));
    }

    public override string GetEndText(bool win)
    {
        return base.GetEndText(win) + "\n" + LocalizationManager.instance.GetLocalizedValue("score") + ": " + objectsPicked + "/" + objectsTotal; ;
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);

        CancelInvoke("SpawnObject");
    }

    public override void EndScenario()
    {
        base.EndScenario();

        CancelInvoke("SpawnObject");

        //delete all spawned objects
        foreach(GameObject gameObject in spawnedObjects)
        {
            Destroy(gameObject);
        }
        spawnedObjects.Clear();

        gameObject.SetActive(false);
    }

    public void PickObject(GameObject gameObject)
    {
        spawnedObjects.Remove(gameObject);
        Destroy(gameObject);

        if (objectsPicked < applesInBasket.Length)
        {
            applesInBasket[objectsPicked].SetActive(true);
        }

        objectsPicked++;
        CorrectAnswer.Add(true);
    }

    public void MissObject()
    {
        CorrectAnswer.Add(false);
    }

    void SpawnObject()
    {
        int fallingIndex = GetNextApple();
        Vector3 fallPoint = fallingObjects[fallingIndex].transform.position;
        fallingObjects[fallingIndex].GetComponent<Animator>().SetTrigger("rippen");

        //Debug.Log("Spawn: " + Time.time + ", " + fallingObjects[fallingIndex].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash);

        float Angle = Vector3.Angle(Camera.main.transform.position - fallPoint, Camera.main.transform.position - previousSpawnPosition);
        DistanceToPrevious.Add(Angle);

        previousSpawnPosition = fallPoint;

        if (lastFallingApple > -1)
        {
            //Debug.Log("DistanceToPrevious: " + (fallingObjects[fallingIndex].transform.position - fallingObjects[lastFallingApple].transform.position).magnitude);
        }

        lastFallingApple = fallingIndex;
        //spawnedObjects.Add(Instantiate(fallingPrefab, fallPoint, Quaternion.identity));
        objectsTotal++;
        if (objectsTotal >= parameters.objectsToSpawn)
        {
            Invoke("FinishGame", 9.0f - parameters.timeBetweenSpawn);
        }
        else
        {
            Invoke("SpawnObject", parameters.timeBetweenSpawn);
        }
    }

    public int GetNextApple()
    {
        int next = Random.Range(0, fallingObjects.Length);

        bool isIdle = fallingObjects[next].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle");
        //if (isIdle)
        //{
        //    Debug.Log("trying to get next apple because of idle state");
        //}

        if (isIdle && (lastFallingApple == -1 || (fallingObjects[next].transform.position - fallingObjects[lastFallingApple].transform.position).magnitude < parameters.maxDistanceBetweenApples))
        {
            return next;
        } else
        {
            //Debug.Log("GetNextApple: " + (fallingObjects[next].transform.position - fallingObjects[lastFallingApple].transform.position).magnitude);
            return GetNextApple();
        }
    }

    // Update is called once per frame
    public override void Update () {
        base.Update();
        if (running)
        {
            Ray ray = new Ray(trackingCamera.transform.position, trackingCamera.transform.forward);
            float enter = 0.0f;
            if (basketPlane.Raycast(ray, out enter))
            {
                
                Vector3 hitPoint = ray.GetPoint(enter);

                //float position = Mathf.Min(hitPoint.x, fallingCenter.transform.position.x + fallingRadius);
                //position = Mathf.Max(position, fallingCenter.transform.position.x - fallingRadius);

                //Vector3 position = new Vector3(hitPoint.x, basket.transform.position.y, hitPoint.z);
                //position = Mathf.Max(position, fallingCenter.transform.position.x - fallingRadius);
                Vector3 basketPosition = new Vector3(hitPoint.x, basket.transform.position.y, hitPoint.z);

                //Vector3 basketPosition = new Vector3(position, basket.transform.position.y, basket.transform.position.z);

                basket.transform.position = basketPosition;
            }
            else
            {
                //Debug.Log("Miss:" + enter);
            }
        }
    }

    public override string GetResultsJSON()
    {
        string currentTimestamp = ResultsSender.GetTimestamp();

        ResultsBuilder results = new ResultsBuilder(gameId, currentTimestamp, duration);
        results.AddSummary("total_rounds", "Total rounds", parameters.objectsToSpawn);
        results.AddSummary("total_rounds_correct", "Total rounds correct", objectsPicked);
        results.AddSummary("avg_reaction_time", "Average reaction time", (duration / parameters.objectsToSpawn));
        results.AddSummary("level", "Progression level", currentProgression + 1);

        results.AddBarChart("reaction_time", "Distance to previous apple", "Distance to previous apple - right answer", "Distance to previous apple - wrong answer",
            DistanceToPrevious, CorrectAnswer);

        results.AddSummary("total_score", "Total Score", (objectsPicked / (float)parameters.objectsToSpawn) * 100);
        results.AddSummary("exercise_duration", "Duration", duration);

        return results.getJSON();
    }

    public override void Recenter()
    {
        base.Recenter();
        InitPlane();
    }
}
