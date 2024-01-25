using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendingExcercise : AbstractScenarioController {
    public int NumDone = 0;
    public int repetitionNeeded = 5;
    public GameObject commonObjects;
    public string actionAnimation;

    public GameObject animal;

    public GameObject trigger;

    public float gameDuration;

    // Use this for initialization
    void Start () {
		
	}

    public override void StartScenario(int progression)
    {
        base.StartScenario(progression);

        trigger.SetActive(false);

        commonObjects.SetActive(true);

        gameObject.SetActive(true);

        //Invoke("FinishScenario", 15);

        NumDone = 0;
    }

    public override void OnInstructions()
    {
        base.OnInstructions();
        if (actionAnimation == "sit")
        {
            float startTime = 1f;
            float interval = 3.7f;
            Invoke("ShowInfoAnimation", startTime);
            Invoke("ShowInfoAnimation", startTime + interval);
            Invoke("ShowInfoAnimation", startTime + interval * 2);
        }
        else if (actionAnimation == "jump")
        {
            float startTime = 1f;
            float interval = 2.25f;
            Invoke("ShowInfoAnimation", startTime);
            Invoke("ShowInfoAnimation", startTime + interval);
            Invoke("ShowInfoAnimation", startTime + interval * 2);
        }
    }

    public Animator infoDog;
    public void ShowInfoAnimation()
    {
        infoDog.SetTrigger(actionAnimation);
    }

    public override void RunScenario()
    {
        base.RunScenario();

        if (ConnectionManager.Instance().Standalone)
        {
            trigger.SetActive(true);
            Invoke("FinishScenario", gameDuration);
        }
    }

    public void BendingDone()
    {
        Debug.Log("BendingDone");
        NumDone++;
        Invoke("AnimalDo", 1.0f);
        if (NumDone >= repetitionNeeded)
        {
            //FinishScenario(true);//TODO: implement this for standalone
        }
    }

    public void AnimalDoDelayed()
    {
        Invoke("AnimalDo", 1);
    }

    public void AnimalDo()
    {
        animal.GetComponent<Animator>().SetTrigger(actionAnimation);
    }

    public override void EndScenario()
    {
        base.EndScenario();

        gameObject.SetActive(false);
        commonObjects.SetActive(false);

        CancelInvoke();
    }

    public void FinishScenario()
    {
        FinishScenario(true);
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);

        trigger.SetActive(false);
    }
}
