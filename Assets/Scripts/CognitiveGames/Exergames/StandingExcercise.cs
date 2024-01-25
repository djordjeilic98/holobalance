using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandingExcercise : AbstractScenarioController {
    public GameObject animal;
    public GameObject ball;

    public GameObject ballStart;
    public GameObject ballAnimated;

    public GameObject moveTrigger;
    public GameObject cameraObject;

    public float standingTime;

    private bool gameStarted;

    // Use this for initialization
    void Start () {
        //animal.GetComponent<Animator>().Play("TakeBall");
        //ball.GetComponent<Animator>().SetInteger("state", 1);
        cameraObject = Camera.main.gameObject;
    }

    public override void Update()
    {
        base.Update();
    }

    public void OnMoved()
    {
        if (gameStarted)
        {
            //Debug.Log("Exit");
            FinishScenario(false);
            CancelInvoke("Win");
            DropBall();
        }
    }

    public void DropBall()
    {
        animal.GetComponent<Animator>().SetInteger("state", 2);
        ball.GetComponent<Animator>().SetInteger("state", 2);
    }

    public override void StartScenario(int progression)
    {
        base.StartScenario(progression);

        gameObject.SetActive(true);

        animal.GetComponent<Animator>().SetInteger("state", 0);
        ball.GetComponent<Animator>().SetInteger("state", 0);

        gameStarted = false;

        moveTrigger.SetActive(false);

        ballAnimated.transform.position = ballStart.transform.position;
    }

    public override void RunScenario()
    {
        base.RunScenario();
        StartGame();
    }

    public void TakeBall()
    {
        ball.GetComponent<Animator>().SetInteger("state", 1);
    }

    public override void EndScenario()
    {
        base.EndScenario();
        gameObject.SetActive(false);
        CancelInvoke("Win");
    }

    public void StartGame()
    {
        gameStarted = true;

        if (ConnectionManager.Instance().Standalone)
        {
            Invoke("Win", standingTime);//TODO: implement this for standalone
        }

        animal.GetComponent<Animator>().SetInteger("state", 1);
        //ball.GetComponent<Animator>().SetInteger("state", 1);
        Invoke("TakeBall", 1.0f);

        moveTrigger.SetActive(ConnectionManager.Instance().Standalone);
        moveTrigger.transform.position = new Vector3(cameraObject.transform.position.x, moveTrigger.transform.position.y, cameraObject.transform.position.z);
    }

    public void Win()
    {
        FinishScenario(true);
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);

        moveTrigger.SetActive(false);

        gameStarted = false;
        if (win)
        {
        } else
        {

            animal.GetComponent<Animator>().SetInteger("state", 2);
            ball.GetComponent<Animator>().SetInteger("state", 2);
        }
    }
}
