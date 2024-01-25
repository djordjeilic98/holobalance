using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AnimalFeedingParameters
{
    public int numAnimals = 3;
}

public class AnimalFeeding : AbstractScenarioController {
    [Header("Parameters")]
    public AnimalFeedingParameters parameters;

    [Space(10)]
    [Header("Settings")]

    public GameObject[] carrots;
    int currentCarrot;

    public GameObject[] animals;
    public Transform[] waypoints;
    private int[] positions;

    //results for sending
    List<float> TotalTime = new List<float>();
    List<float> ReactionTime = new List<float>();
    List<bool> CorrectAnswer = new List<bool>();

    private float answerStartTime;

    private int answersCorrect;

    // Use this for initialization
    void Start () {
        //StartScenario();
    }

    public override void StartScenario(int progression)
    {
        gameObject.SetActive(true);

        base.StartScenario(progression);

        EnableInput(false);

        parameters = ProgressionLoader.Instance.gameParameters.animal_feeding[progression];

        positions = new int[parameters.numAnimals];

        for (int i = 0; i < carrots.Length; i++)
        {
            carrots[i].SetActive(i < parameters.numAnimals);
        }

        //randomize positions
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = i;
        }
        currentCarrot = 0;
        answersCorrect = 0;

        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].SetActive(false);
            animals[i].GetComponent<NavigationController>().timeFed = 0;
            animals[i].GetComponent<GazeButton>().enabled = false;
        }

        //set animals on position
        for (int i = 0; i < positions.Length; i++)
        {
            animals[positions[i]].SetActive(true);
            animals[positions[i]].transform.position = waypoints[i].position;
            animals[positions[i]].transform.rotation = waypoints[i].rotation;
            animals[positions[i]].GetComponent<NavigationController>().startPosition = waypoints[i].position;
        }

        TotalTime.Clear();
        ReactionTime.Clear();
        CorrectAnswer.Clear();
    }

    public override void RunScenario()
    {
        base.RunScenario();
        StartGame();
    }

    public void StartGame()
    {
        EnableInput(true);

        answerStartTime = Time.time;
    }

    public override void EndScenario()
    {
        base.EndScenario();

        gameObject.SetActive(false);

        CancelInvoke();
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);

        EnableInput(false);
    }

    public override string GetEndText(bool win)
    {
        return base.GetEndText(win) + "\n" + LocalizationManager.instance.GetLocalizedValue("score") + ": " + answersCorrect + "/" + parameters.numAnimals +
            "\n" + LocalizationManager.instance.GetLocalizedValue("response_time") + ": " + (duration / parameters.numAnimals).ToString("0.00") + " s";
    }

    public void EnableInput(bool enable)
    {
        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].GetComponent<GazeButton>().enabled = enable;
        }
    }

    public void AnimalSelected(NavigationController animal)
    {
        EnableInput(false);
        TotalTime.Add(duration);
        ReactionTime.Add(Time.time - answerStartTime);
        CorrectAnswer.Add(animal.timeFed == 1);
        answerStartTime = Time.time;

        if (animal.timeFed == 1)
        {
            answersCorrect++;
        }
    }

    public void Shuffle(GameObject feedingObject)
    {
        //randomize positions
        for (int i = 0; i < positions.Length; i++)
        {
            int rnd = Random.Range(0, positions.Length);
            int tempGO = positions[rnd];
            positions[rnd] = positions[i];
            positions[i] = tempGO;
        }

        //set animals on position
        for (int i = 0; i < positions.Length; i++)
        {
            animals[positions[i]].GetComponent<NavigationController>().startPosition = waypoints[i].position;
            if (feedingObject != animals[positions[i]])
            {
                animals[positions[i]].GetComponent<NavigationController>().ChangeState(NavigationController.STATES.CHANGE_PLACE);
            }
        }

        Invoke("CarrotEat", 2.0f);
    }

    public void CarrotEat()
    {
        carrots[currentCarrot++].SetActive(false);

        if (currentCarrot >= parameters.numAnimals)
        {
            int animalsFed = 0;
            for (int i = 0; i < animals.Length; i++)
            {
                animalsFed += animals[i].GetComponent<NavigationController>().timeFed > 0 ? 1 : 0;
            }
            FinishScenario((animalsFed / (float)parameters.numAnimals) > 0.69f);
        }
    }

    public override string GetResultsJSON()
    {
        string currentTimestamp = ResultsSender.GetTimestamp();

        ResultsBuilder results = new ResultsBuilder(gameId, currentTimestamp, duration);
        results.AddSummary("total_rounds", "Total rounds", parameters.numAnimals);
        results.AddSummary("total_rounds_correct", "Total rounds correct", answersCorrect);
        results.AddSummary("avg_reaction_time", "Average reaction time", (duration / parameters.numAnimals));
        results.AddSummary("level", "Progression level", currentProgression + 1);

        results.AddBarChart("reaction_time", "Reaction time(ms)", "Reaction time - right answer(ms)", "Reaction time - wrong answer(ms)",
            ReactionTime, CorrectAnswer);
        results.AddBarChart("offset", "Offset", "Time(ms)", TotalTime);

        results.AddSummary("total_score", "Total Score", (answersCorrect / (float)parameters.numAnimals) * 100);
        results.AddSummary("exercise_duration", "Duration", duration);

        return results.getJSON();
    }
}

