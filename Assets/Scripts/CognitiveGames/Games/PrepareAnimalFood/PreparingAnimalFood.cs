using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PreparingFoodParameters
{
    public int numDifferentFood = 2;
    public int totalRounds = 10;
}

public class PreparingAnimalFood : AbstractScenarioController {
    [Header("Parameters")]
    public PreparingFoodParameters parameters;

    [Space(10)]
    [Header("Settings")]

    public GameObject animal;
    public GameObject[] cards;
    public GameObject[] foods;

    public Transform centerBoxes;
    public Transform floorBoxes;

    private int currentCard;
    public GameObject currentFood;

    private int numFood;
    private int goodFood;

    private int[] foodIndex;

    //results for sending
    List<float> TotalTime = new List<float>();
    List<float> ReactionTime = new List<float>();
    List<bool> CorrectAnswer = new List<bool>();

    private float answerStartTime;

    // Use this for initialization
    void Start () {
        //Invoke("SpawnAnimal", 1);
        //Invoke("SpawnAnimal", 3);
        //Invoke("SpawnAnimal", 5);
        //Invoke("SpawnAnimal", 7);
        //StartScenario();
    }

    public override void StartScenario(int progression)
    {
        base.StartScenario(progression);

        numFood = 0;
        parameters = ProgressionLoader.Instance.gameParameters.preparing_food[progression];

        foreach (GameObject food in foods)
        {
            //food.SetActive(false);
        }
        gameObject.SetActive(true);

        for (int i = 0; i < foods.Length; i++)
        {
            foods[i].SetActive(false);
            foods[i].GetComponent<GazeButton>().enabled = false;
        }

        foodIndex = new int[foods.Length];
        for (int i = 0; i < foodIndex.Length; i++)
        {
            foodIndex[i] = i;
        }
        for (int i = 0; i < foodIndex.Length; i++)
        {
            int rnd = Random.Range(0, foodIndex.Length);
            int tempGO = foodIndex[rnd];
            foodIndex[rnd] = foodIndex[i];
            foodIndex[i] = tempGO;
        }

        //Vector3 boxSize = new Vector3(0.3f, 0, 0);
        //Vector3 boxSpace = new Vector3(0.1f, 0, 0);
        float boxSize = 0.3f;
        float boxSpace = 0.1f;
        /*if (parameters.numDifferentFood == 5)
        {
            boxSpace = new Vector3(0.01f, 0, 0);
        }*/

        //first 3 on table
        int numOnTable = (parameters.numDifferentFood > 3 ? 3 : parameters.numDifferentFood);
        

        Vector3 startPosition = centerBoxes.transform.position - centerBoxes.transform.forward * ((boxSize * numOnTable) + (boxSpace * (numOnTable - 1))) / 2 + centerBoxes.transform.forward * boxSize / 2;
        for (int i = 0; i < numOnTable; i++)
        {
            foods[foodIndex[i]].SetActive(true);
            foods[foodIndex[i]].transform.position = startPosition;
            foods[foodIndex[i]].transform.rotation = centerBoxes.transform.rotation;
            startPosition += centerBoxes.transform.forward * (boxSize + boxSpace);
        }

        int numOnFloor = (parameters.numDifferentFood > 3 ? parameters.numDifferentFood - 3 : 0);
        startPosition = floorBoxes.transform.position - floorBoxes.transform.forward * ((boxSize * numOnFloor) + (boxSpace * (numOnFloor - 1))) / 2 + centerBoxes.transform.forward * boxSize / 2;
        for (int i = 0; i < numOnFloor; i++)
        {
            foods[foodIndex[i + 3]].SetActive(true);
            foods[foodIndex[i + 3]].transform.position = startPosition;
            foods[foodIndex[i + 3]].transform.rotation = floorBoxes.transform.rotation;
            startPosition += floorBoxes.transform.forward * (boxSize + boxSpace);
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

    void EnableInput(bool enable)
    {
        for (int i = 0; i < parameters.numDifferentFood; i++)
        {
            foods[foodIndex[i]].GetComponent<GazeButton>().enabled = enable;
        }
    }

    public override void EndScenario()
    {
        base.EndScenario();

        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        EnableInput(true);

        NextCard();
    }

    //public void SelectFood(int foodIndex)
    //{
    //    foods[foodIndex].SetActive(true);
    //    foods[foodIndex].GetComponent<Animator>().Play("Flying");

    //    //Invoke("Eat", 0.5f);
        

    //    if (currentCard == foodIndex)
    //    {
    //        goodFood++;
    //    }
    //}

    public void SelectFood(GameObject food)
    {
        EnableInput(false);
        food.GetComponent<MovingController>().StartMoving();

        ReactionTime.Add(Time.time - answerStartTime);
        TotalTime.Add(duration);
        CorrectAnswer.Add(currentFood == food);

        if (currentFood == food)
        {
            goodFood++;
        }
    }

    public void Eat()
    {
        cards[foodIndex[currentCard]].SetActive(false);
        animal.GetComponent<Animator>().SetTrigger("eat");
        //Invoke("NextCard", 3.2f);
    }

    public void NextCard()
    {
        numFood++;
        if (numFood <= parameters.totalRounds)
        {
            cards[foodIndex[currentCard]].SetActive(false);

            currentCard = Random.Range(0, parameters.numDifferentFood);

            cards[foodIndex[currentCard]].SetActive(true);
            currentFood = foods[foodIndex[currentCard]];

            EnableInput(true);
        } else
        {
            cards[foodIndex[currentCard]].SetActive(false);

            FinishScenario((goodFood / (float)parameters.totalRounds) > 0.69f);
        }
        answerStartTime = Time.time;
    }

    public override string GetEndText(bool win)
    {
        return base.GetEndText(win) + "\n" + LocalizationManager.instance.GetLocalizedValue("score") + ": " + goodFood + "/" + parameters.totalRounds +
             "\n" + LocalizationManager.instance.GetLocalizedValue("response_time") + ": " + (duration / parameters.totalRounds).ToString("0.00") + " s";
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);
    }

    public override string GetResultsJSON()
    {
        string currentTimestamp = ResultsSender.GetTimestamp();

        ResultsBuilder results = new ResultsBuilder(gameId, currentTimestamp, duration);
        results.AddSummary("total_rounds", "Total rounds", parameters.totalRounds);
        results.AddSummary("total_rounds_correct", "Total rounds correct", goodFood);
        results.AddSummary("avg_reaction_time", "Average reaction time", (duration / parameters.totalRounds));
        results.AddSummary("level", "Progression level", currentProgression + 1);

        results.AddBarChart("reaction_time", "Reaction time(ms)", "Reaction time - right answer(ms)", "Reaction time - wrong answer(ms)",
            ReactionTime, CorrectAnswer);
        results.AddBarChart("offset", "Offset", "Time(ms)", TotalTime);

        results.AddSummary("total_score", "Total Score", (goodFood / (float)parameters.totalRounds) * 100);

        return results.getJSON();
    }
}
