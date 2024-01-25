using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MarbleGameParameters
{
    public float spawnInterval = 8.0f;
    public int objectsToSpawn = 10;
}

public class MarbleGame : AbstractScenarioController {

    [Header("Parameters")]
    public MarbleGameParameters parameters;

    [Space(10)]
    [Header("Settings")]
    public Material[] materials;
    public GameObject prefab;
    public GameObject[] spawnPoints;

    public int goodBalls = 0;
    public int ballFinished = 0;
    List<GameObject> spawnedBalls = new List<GameObject>();

    public GameObject[] progressionLevel;

	// Use this for initialization
	void Start () {
        
	}

    public override void StartScenario(int progression)
    {
        base.StartScenario(progression);

        parameters = ProgressionLoader.Instance.gameParameters.marble_game[progression];

        goodBalls = 0;
        ballFinished = 0;

        gameObject.SetActive(true);

        for (int i = 0; i < progressionLevel.Length; i++)
        {
            progressionLevel[i].SetActive(false);
        }
        progressionLevel[progression].SetActive(true);

        for(int i = 0; i < spawnedBalls.Count; i++)
        {
            Destroy(spawnedBalls[i]);
        }
        spawnedBalls.Clear();
    }

    public override void RunScenario()
    {
        base.RunScenario();
        StartGame();
    }

    public void StartGame()
    {
        InvokeRepeating("SpawnBall", 2.0f, parameters.spawnInterval);
    }

    public override void EndScenario()
    {
        base.EndScenario();

        CancelInvoke("SpawnBall");

        gameObject.SetActive(false);
    }

    public void SpawnBall()
    {
        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject ball = Instantiate(prefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
        int materialIndex = Random.Range(0, materials.Length);
        ball.GetComponent<MeshRenderer>().material = materials[materialIndex];
        ball.GetComponent<BallController>().materialIndex = materialIndex;
        spawnedBalls.Add(ball);

        if (spawnedBalls.Count >= parameters.objectsToSpawn)
        {
            CancelInvoke("SpawnBall");
        }
        ball.GetComponent<Animator>().enabled = false;
    }

    public override string GetEndText(bool win)
    {
        return base.GetEndText(win) + "\n" + LocalizationManager.instance.GetLocalizedValue("score") + ": " + goodBalls + "/" + parameters.objectsToSpawn;
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);
    }

    public void BallFinished(bool success)
    {
        Debug.Log("Ball finished: " + success);
        if (success)
        {
            goodBalls++;
        }
        ballFinished++;
        if (ballFinished >= parameters.objectsToSpawn)
        {
            FinishScenario((goodBalls / (float)parameters.objectsToSpawn) > 0.69f);
        }
    }

    public override string GetResultsJSON()
    {
        string currentTimestamp = ResultsSender.GetTimestamp();

        ResultsBuilder results = new ResultsBuilder(gameId, currentTimestamp, duration);
        results.AddSummary("total_marbles", "Total rounds", parameters.objectsToSpawn);
        results.AddSummary("total_marbles_correct", "Total rounds correct", goodBalls);
        //results.AddSummary("avg_reaction_time", "Average reaction time", (duration / parameters.numAnimals));
        results.AddSummary("level", "Progression level", currentProgression + 1);

        //results.AddBarChart("reaction_time", "Reaction time(ms)", "Reaction time - right answer(ms)", "Reaction time - wrong answer(ms)",
        //    ReactionTime, CorrectAnswer);

        results.AddSummary("total_score", "Total Score", (goodBalls / (float)parameters.objectsToSpawn) * 100);
        results.AddSummary("exercise_duration", "Duration", duration);

        return results.getJSON();
    }
}
