using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractScenarioController : MonoBehaviour {
    public string gameId;

    public int currentProgression;

    protected bool running;

    protected float duration;

    public GameObject assets;

    private int gameIndex;

	// Update is called once per frame
	public virtual void Update () {
        if (running)
        {
            duration += Time.deltaTime;
        }
    }

    public virtual void StartScenario(int progression)
    {
        gameIndex = GameManager.Instance.GetGameIndex(this);

        currentProgression = progression;       

        GameManager.Instance.currentGame = this;
        GameManager.Instance.currentGameIndex = gameIndex;
        GameManager.Instance.currentProgession = progression;
        GameManager.Instance.ShowInfoPanel();
        GameManager.Instance.ShowBack(true);

        assets.SetActive(false);

        EventManager.StartListening("Instructions", OnInstructions);
    }

    public virtual void OnInstructions()
    {
        Debug.Log("OnInstructions");
    }

    public virtual void RunScenario()
    {
        running = true;
        duration = 0;

        assets.SetActive(true);

        GameManager.Instance.HideInfo();
    }

    public virtual void FinishScenario(bool win)
    {
        running = false;

        GameManager.Instance.ShowEndPanel(win);

        GameManager.Instance.ShowBack(false);

        assets.SetActive(false);

        CancelInvoke();
    }

    public virtual string GetResultsJSON()
    {
        return "";
    }

    public virtual void EndScenario()
    {
        running = false;
        GameManager.Instance.HideInfo();
        EventManager.StopListening("Instructions", OnInstructions);
    }

    public void SendResults()
    {
        if (!ConnectionManager.Instance().Standalone)
        {
            Debug.Log("Sending game results to EDGE...");
            ScenePayload payloadToSend = new ScenePayload
            {
                action = "CognitiveGamesStart",
                type = gameId,
                value = "" + (currentProgression + 1),
                status = "Done",
                data = GetResultsJSON(),
            };
            GameManager.Instance.SendResults(payloadToSend);
        }
    }

    public virtual string GetEndText(bool win)
    {
        return LocalizationManager.instance.GetLocalizedValue(win ? SceneControllerCTG.WIN : SceneControllerCTG.LOSE);
    }

    public virtual void Recenter()
    {

    }
}
