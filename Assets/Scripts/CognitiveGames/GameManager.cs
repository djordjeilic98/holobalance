using HoloKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject holokit;
    public RadialMenuItem MainMenu;

    public GameObject InfoCanvas;
    public Text InfoText;
    public Image InfoImage;
    public GameObject InfoPlayButton;

    public GameObject EndCanvas;
    public Text EndText;
    public GameObject RetryButton;
    public GameObject NextButton;

    public GameObject BackCanvas;

    public AbstractScenarioController[] Games;

    public static GameManager Instance;

    public GameObject content;

    public int currentGameIndex;
    public int currentProgession;
    private int numRetries = 0;

    public GameObject lastMenu;
    public GameObject lastCentralButton;

    public enum GamesList
    {
        NONE,
        SITTING_1,
        SITTING_2,
        STANDING_1,
        STANDING_2,
        STANDING_3,
        WALKING_1,
        WALKING_2,
        WALKING_3,
        WALKING_4,
        MEMORY_GAME,
        CATCHING_GAME,
        REMEMBER_PREVIOUS_GAME,
        BRIDGE_CROSSING,
        ANIMAL_FEEDING,
        PREPARE_ANIMAL_FOOD,
        MAIN_MENU
    }

    public GamesList StartingGame;
    public int StartingGameProgression;

    public AbstractScenarioController currentGame;

    private HoloKitGazeManager[] gazeManagers;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        Invoke("Init", 0);
        gazeManagers = Resources.FindObjectsOfTypeAll<HoloKitGazeManager>();
        SetGaze(true);
    }

    public void SetGaze(bool enable)
    {
        //Debug.Log("SetGaze: " + enable);
        foreach (HoloKitGazeManager gazeManager in gazeManagers)
        {
            gazeManager.gameObject.SetActive(enable);
        }
    }

    private void OnEnable()
    {
        if (ConnectionManager.Instance().Standalone)
        {
            holokit.SetActive(true);
        }
    }

    void Init()
    {
        if (StartingGame == GamesList.NONE || !ConnectionManager.Instance().Standalone)
        {
            MainMenu.Hide();
        }
        else if (StartingGame != GamesList.MAIN_MENU)
        {
            StartGame((int)StartingGame - 1, StartingGameProgression);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetGameIndex(AbstractScenarioController gameController)
    {
        for (int i = 0; i < Games.Length; i++)
        {
            if (Games[i] == gameController)
            {
                return i;
            }
        }
        return -1;
    }

    public void StartGame(int gameIndex, int progressionLevel)
    {
        MainMenu.Hide();

        if (currentGame != null)
        {
            currentGame.EndScenario();
        }
        if (currentGameIndex != gameIndex || currentProgession != progressionLevel)
        {
            numRetries = 0;
        }
        currentGameIndex = gameIndex;
        currentProgession = progressionLevel;

        EndCanvas.SetActive(false);
        currentGame = Games[gameIndex].GetComponent<AbstractScenarioController>();
        currentGame.StartScenario(progressionLevel);

        bool gazeEnabled = ConnectionManager.Instance().Standalone || GetGameIndex(currentGame) >= 9;
        SetGaze(gazeEnabled);
    }

    public void RunGame()
    {
        currentGame.GetComponent<AudioFeedback>().Stop();
        currentGame.RunScenario();
    }

    public void FinishGame(bool win)
    {
        
        currentGame.FinishScenario(win);
    }

    public void ShowBack(bool show)
    {
        BackCanvas.SetActive(show);
        if (!ConnectionManager.Instance().Standalone)
        {
            BackCanvas.SetActive(false);
        }
    }

    public void ShowInfoPanel()
    {
        InfoCanvas.SetActive(true);

        InfoText.gameObject.SetActive(false);
        InfoImage.gameObject.SetActive(false);

        if (currentGame.GetComponent<InfoController>())
        {
            currentGame.GetComponent<InfoController>().ShowInfo();
        }
        else
        {
            currentGame.GetComponent<AudioFeedback>().Info();
            InfoText.gameObject.SetActive(true);
            InfoText.text = LocalizationManager.instance.GetLocalizedValue(currentGame.GetComponent<AudioFeedback>().infoKey);

            if (currentGame.GetComponent<AudioFeedback>().infoImage)
            {
                InfoImage.gameObject.SetActive(true);
                InfoImage.sprite = currentGame.GetComponent<AudioFeedback>().infoImage;
                InfoText.gameObject.SetActive(false);
            }
            else
            {
                InfoImage.gameObject.SetActive(false);
            }
        }

        InfoPlayButton.SetActive(ConnectionManager.Instance().Standalone || GetGameIndex(currentGame) >= 9);
    }

    public void HideInfo()
    {
        InfoCanvas.SetActive(false);
        if (currentGame.GetComponent<InfoController>())
        {
            currentGame.GetComponent<InfoController>().HideInfo();
        }
    }

    public void ShowEndPanel(bool win)
    {
        //currentGame.GetComponent<AudioFeedback>().Win;
        

        if (ConnectionManager.Instance().Standalone || GetGameIndex(currentGame) >= 9)
        {
            RetryButton.SetActive(true);
            NextButton.SetActive(true);

            EndCanvas.SetActive(true);
            RetryButton.SetActive(numRetries < 2);

            EndText.text = currentGame.GetEndText(win);
            if (win)
            {
                currentGame.GetComponent<AudioFeedback>().Win();
            }
            else
            {
                currentGame.GetComponent<AudioFeedback>().Lose();
            }
        }

        //FIX: Show panel in front of you
        //winPanel.SetActive(true);
        //endCanvas.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 1;
        //endCanvas.transform.LookAt(playerCamera.transform.position);
    }

    public void EndGame()
    {
        currentGame.EndScenario();
    }

    public void NextGame()
    {
        HideInfo();
        BackCanvas.SetActive(false);
        EndCanvas.SetActive(false);
        if (ConnectionManager.Instance().Standalone)
        {
            lastMenu.GetComponent<RadialMenuController>().OpenMenu(lastCentralButton);
        }
        EndGame();
        currentGame.SendResults();
    }

    public void RetryGame()
    {
        numRetries++;
        StartGame(currentGameIndex, currentProgession);
    }

    public void SendResults(ScenePayload payload)
    {
        StartCoroutine(ConnectionManager.Instance().PostHttpRequest(payload.ToJSON()));
        //StartCoroutine(ResultsSender.PostData(payload.data)); //for testing the RRD backend
    }

    public void Recenter()
    {
        //Debug.Log((Camera.main.transform.position - content.transform.position).magnitude);
        Vector3 forwardHorizontal = (new Vector3(Camera.main.transform.forward.x, Camera.main.transform.position.y, Camera.main.transform.forward.z)).normalized;
        Vector3 frontPosition = Camera.main.transform.position + forwardHorizontal * 1.11f;
        Vector3 lookAtPosition = new Vector3(Camera.main.transform.position.x, content.transform.position.y, Camera.main.transform.position.z);
        content.transform.position = new Vector3(frontPosition.x, content.transform.position.y, frontPosition.z);
        content.transform.LookAt(lookAtPosition);
        content.transform.Rotate(new Vector3(0, 180, 0));

        foreach (var game in Games)
        {
            game.Recenter();
        }
    }

    /*private void OnDrawGizmos()
    {
        Vector3 forwardHorizontal = (new Vector3(Camera.main.transform.forward.x, Camera.main.transform.position.y, Camera.main.transform.forward.z)).normalized;
        Vector3 frontPosition = Camera.main.transform.position + forwardHorizontal * 1.11f;
        Vector3 lookAtPosition = new Vector3(Camera.main.transform.position.x, content.transform.position.y, Camera.main.transform.position.z);
        Vector3 newContentPosition = new Vector3(frontPosition.x, content.transform.position.y, frontPosition.z);
        Debug.DrawLine(Camera.main.transform.position, frontPosition, Color.cyan);
    }*/
}
