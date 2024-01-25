using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class RememberPreviousParameters
{
    public int objectsToSpawn = 10;
    public int numDifferentAnimals = 2;
    public int numRememberingBack = 1;
}

public class RememberPreviousGame : AbstractScenarioController {

    [Header("Parameters")]
    public RememberPreviousParameters parameters;

    [Space(10)]
    [Header("Settings")]

    //UI
    public GameObject gameCanvas;

    public Animator hatAnimator;
    public Animator wandAnimator;

    public GameObject[] objects;
    private GameObject[] selectedAnimals;

    protected int objectsTotal;
    protected int answersCorrect;
    float totalResponseTime;

    private float answerStartTime;
    //private int previous;
    private int current;
    private float currentTime;

    //results for sending
    List<float> TotalTime = new List<float>();
    List<float> ReactionTime = new List<float>();
    List<bool> CorrectAnswer = new List<bool>();

    private int[] backAnimals;

    private int numToShow;

    public override void StartScenario(int progression)
    {
        string infoKey = "";
        switch (progression)
        {
            case 0:
            case 1:
                infoKey = "remember_previous_1_info";
                break;
            case 2:
            case 3:
                infoKey = "remember_previous_2_info";
                break;
            case 4:
                infoKey = "remember_previous_3_info";
                break;
        }
        GetComponent<AudioFeedback>().infoKey = infoKey;

        base.StartScenario(progression);

        parameters = ProgressionLoader.Instance.gameParameters.remember_previous[progression];

        backAnimals = new int[parameters.numRememberingBack];

        selectedAnimals = new GameObject[objects.Length];
        for (int i = 0; i < selectedAnimals.Length; i++)
        {
            selectedAnimals[i] = objects[i];
        }
        for (int i = 0; i < selectedAnimals.Length; i++)
        {
            int rnd = Random.Range(0, selectedAnimals.Length);
            GameObject tempGO = selectedAnimals[rnd];
            selectedAnimals[rnd] = selectedAnimals[i];
            selectedAnimals[i] = tempGO;
        }

        gameCanvas.SetActive(false);

        gameObject.SetActive(true);

        objectsTotal = 0;
        totalResponseTime = 0;
        answersCorrect = 0;

        HideAnimal();

        current = Random.Range(0, parameters.numDifferentAnimals);
        objects[current].SetActive(true);

        TotalTime.Clear();
        ReactionTime.Clear();
        CorrectAnswer.Clear();

        


        //testing
        //objectsTotal = 3;
        //answersCorrect = 2;
        //totalResponseTime = 5;
        //duration = 10;
        //ReactionTime.Add(1.7f);
        //ReactionTime.Add(1.8f);
        //ReactionTime.Add(1.2f);
        //CorrectAnswer.Add(true);
        //CorrectAnswer.Add(true);
        //CorrectAnswer.Add(false);

        //SendResults();
    }

    void StoreAnimal(int animalId)
    {
        for (int i = 1; i < backAnimals.Length; i++)
        {
            backAnimals[i - 1] = backAnimals[i];
        }
        backAnimals[backAnimals.Length - 1] = animalId;
    }

    public override void RunScenario()
    {
        base.RunScenario();
        StartGame();
    }

    public void HideAnimal()
    {
        //StoreAnimal(current);
        //previous = current;
        foreach (GameObject animal in objects)
        {
            animal.SetActive(false);
        }
    }

    public void ShowAnimal()
    {
        StoreAnimal(current);

        current = Random.Range(0, parameters.numDifferentAnimals);
        objects[current].SetActive(true);

        Invoke("ShowButtons", 0.5f);
    }

    public void ShowButtons()
    {
        if (numToShow == 0)
        {
            answerStartTime = Time.time;
            gameCanvas.SetActive(true);
        }
    }

    public override void EndScenario()
    {
        base.EndScenario();

        //CancelInvoke("SpawnObject");

        CancelInvoke();

        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        //gameCanvas.SetActive(true);

        numToShow = parameters.numRememberingBack;

        Invoke("ShowNextAnimal", 2);
        //ShowNextAnimal();
    }

    public void ShowNextAnimal()
    {
        hatAnimator.SetTrigger("down");
        wandAnimator.SetTrigger("abrakadabra");

        gameCanvas.SetActive(false);
        Invoke("HideAnimal", 0.8f);
        Invoke("ShowAnimal", 1);

        numToShow--;
        if (numToShow > 0)
        {
            Invoke("ShowNextAnimal", 3);
        }
    }

    void FinishGame()
    {
        gameCanvas.SetActive(false);

        //SendResults();
    }

    public override string GetEndText(bool win)
    {
        return base.GetEndText(win) + "\n" + LocalizationManager.instance.GetLocalizedValue("score") + ": " + answersCorrect + "/" + objectsTotal +
            "\n" + LocalizationManager.instance.GetLocalizedValue("response_time") + ": " + (totalResponseTime / objectsTotal).ToString("0.00") + " s";
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);
        FinishGame();
    }

    public void Answer(bool same)
    {
        bool correct = false;

        //check answer
        if ((same && current == backAnimals[0]) || (!same && current != backAnimals[0]))
        {
            correct = true;
            answersCorrect++;
            
        }

        totalResponseTime += Time.time - answerStartTime;

        ReactionTime.Add(Time.time - answerStartTime);
        TotalTime.Add(duration);
        CorrectAnswer.Add(correct);

        objectsTotal++;
        if (objectsTotal >= parameters.objectsToSpawn)
        {
            FinishScenario(true);
        } else
        {
            hatAnimator.SetTrigger("down");
            wandAnimator.SetTrigger("abrakadabra");

            gameCanvas.SetActive(false);
            Invoke("HideAnimal", 0.8f);
            Invoke("ShowAnimal", 1);
        }
    }

    public override string GetResultsJSON()
    {
        string currentTimestamp = ResultsSender.GetTimestamp();

        ResultsBuilder results = new ResultsBuilder(gameId, currentTimestamp, duration);
        results.AddSummary("total_rounds", "Total rounds", objectsTotal);
        results.AddSummary("total_rounds_correct", "Total rounds correct", answersCorrect);
        results.AddSummary("avg_reaction_time", "Average reaction time", (totalResponseTime / objectsTotal));
        results.AddSummary("level", "Progression level", currentProgression + 1);
        results.AddSummary("exercise_duration", "Duration", duration);

        results.AddBarChart("reaction_time", "Reaction time(ms)", "Reaction time - right answer(ms)", "Reaction time - wrong answer(ms)",
            ReactionTime, CorrectAnswer);
        results.AddBarChart("offset", "Offset", "Time(ms)", TotalTime);

        results.AddSummary("total_score", "Total Score", (answersCorrect / (float)objectsTotal) * 100);

        return results.getJSON();
    }
}
