using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneController : SceneController 
{
    public Text textNotifications;

    public Button buttonServerIP;
    public InputField inputFieldServerIP;

    public Button buttonDE, buttonGR, buttonEN, buttonPT, buttonIT;
    public Button buttonFemale, buttonMale;
    public Button buttonDemo;

    private bool connected = false;
    private Coroutine connectToEdge;
    private string initMessage = "Connecting...";

    private ScenePayload payload;
    protected string language, avatar;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        textNotifications.text = initMessage;
        inputFieldServerIP.text = httpServer.EdgeServerAddress.ToString();
        buttonServerIP.onClick.AddListener(OnChangeServerIP);
        buttonDE.onClick.AddListener(OnButtonDE);
        buttonGR.onClick.AddListener(OnButtonGR);
        buttonEN.onClick.AddListener(OnButtonEN);
        buttonIT.onClick.AddListener(OnButtonIT);
        buttonPT.onClick.AddListener(OnButtonPT);
        buttonFemale.onClick.AddListener(OnButtonFemale);
        buttonMale.onClick.AddListener(OnButtonMale);
        buttonDemo.onClick.AddListener(OnButtonVCDemo);

        payload = new ScenePayload
        {
            action = "Load",
            type = "Scene",
            value = SceneManager.GetActiveScene().name,
            status = "Done",
            data = HttpServer.Instance().GetEndpoint()
        };

        language = PlayerPrefs.GetString("Language");
        avatar = PlayerPrefs.GetString("Avatar");

        switch(language)
        {
            case "EN":
                OnButtonEN();
                break;
            case "DE":
                OnButtonDE();
                break;
            case "GR":
                OnButtonGR();
                break;
            case "PT":
                OnButtonPT();
                break;
            case "IT":
                OnButtonIT();
                break;
            default:
                OnButtonEN();
                break;
        }

        switch (avatar)
        {
            case "Female":
                OnButtonFemale();
                break;
            case "Male":
                OnButtonMale();
                break;
            default:
                OnButtonFemale();
                break;
        }

        connectToEdge = StartCoroutine(ConnectToEdge(payload));
        
        // disable gaze manager, gazing point and set camera to AR mode if holobox
        if(isWindowsOS)
        {
            GameObject.Find("HoloKitGazeManager").SetActive(false);
            GameObject.Find("HoloKitCamera").GetComponent<Holobalance.HolokitCamera>().SetToModeAR();
        }
    }

    private void OnButtonVCDemo()
    {
        isDemo = true;
        SceneManager.LoadScene("VirtualCoach");
    }

    public void OnButtonCGDemo()
    {
        ConnectionManager.Instance().Standalone = true;
        SceneManager.LoadScene("CognitiveGames");
    }

    private void OnChangeServerIP()
    {
        httpServer.EdgeServerAddress = Convert.ToInt32(inputFieldServerIP.text);
        StopCoroutine(connectToEdge);
        connectToEdge = StartCoroutine(ConnectToEdge(payload));
    }

    private void OnButtonIT()
    {
        buttonIT.interactable = false;
        buttonPT.interactable = true;
        buttonDE.interactable = true;
        buttonGR.interactable = true;
        buttonEN.interactable = true;

        PlayerPrefs.SetString("Language", "IT");
    }

    private void OnButtonPT()
    {
        buttonPT.interactable = false;
        buttonIT.interactable = true;
        buttonDE.interactable = true;
        buttonGR.interactable = true;
        buttonEN.interactable = true;

        PlayerPrefs.SetString("Language", "PT");
    }

    private void OnButtonDE()
    {
        buttonDE.interactable = false;
        buttonPT.interactable = true;
        buttonIT.interactable = true;
        buttonGR.interactable = true;
        buttonEN.interactable = true;
        
        PlayerPrefs.SetString("Language", "DE");
    }
    private void OnButtonGR()
    {
        buttonGR.interactable = false;
        buttonPT.interactable = true;
        buttonIT.interactable = true;
        buttonDE.interactable = true;
        buttonEN.interactable = true;
        
        PlayerPrefs.SetString("Language", "GR");
    }
    private void OnButtonEN()
    {
        buttonEN.interactable = false;
        buttonPT.interactable = true;
        buttonIT.interactable = true;
        buttonGR.interactable = true;
        buttonDE.interactable = true;

        PlayerPrefs.SetString("Language", "EN");
    }

    private void OnButtonFemale()
    {
        buttonFemale.interactable = false;
        buttonMale.interactable = true;

        PlayerPrefs.SetString("Avatar", "Female");
    }

    private void OnButtonMale()
    {
        buttonMale.interactable = false;
        buttonFemale.interactable = true;

        PlayerPrefs.SetString("Avatar", "Male");
    }

    // override this method in order to prevent sending notice
    // to edge server about scene loaded regarding this scene
    // because it will send message to server anyway when connected (ConnectToEdge)
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

    protected IEnumerator ConnectToEdge(ScenePayload payload)
    {
        int maxAttempts = 10;
        int attemptCount = 0;
        while(attemptCount < maxAttempts && !connected)
        {
            textNotifications.text = initMessage + String.Format("(Attempt {0})", attemptCount);
            yield return ConnectionManager.Instance().PostHttpRequest(payload.ToJSON(), (bool result) => { connected = result; });
            attemptCount++;
        }

        if(!connected)
            textNotifications.text = String.Format("Unable to connect to {0}!", HttpServer.Instance().GetServerEndpoint());
        else
        {
            textNotifications.text = "Connected!";
            PlayerPrefs.SetInt("EdgeServerAddress", httpServer.EdgeServerAddress);
        }

        yield return new WaitForSeconds(1.0f);
    }

    protected override void GetTransitionMessage(ScenePayload payload)
    {
        switch (payload.value)
        {
            case "VirtualCoach":
                textNotifications.text = "Loading Virtual Coach";
                break;
            case "CognitiveGames":
                textNotifications.text = "Loading Cognitive Games";
                break;
            default:
                textNotifications.text = "HoloBalance";
                break;
        }
    }

    protected void GetTransitionMessage1(ScenePayload payload)
    {
        switch(payload.value)
        {
            case "VirtualCoach":
                textNotifications.text = "Loading Virtual Coach\nExercise 1 | Sit-Yaw\nProgression 1 ";
                break;
            case "VC holobalance_sitting_3 P0":
                textNotifications.text = "Loading Virtual Coach\nExercise 3 | Sit-Bend\nProgression 0";
                break;
            case "VC holobalance_standing_2 P1":
                textNotifications.text = "Loading Virtual Coach\nExercise 6 | Stand-Reach\nProgression 1";
                break;
            case "VC holobalance_standing_4 P0":
                textNotifications.text = "Loading Virtual Coach\nExercise 7 | Stand-Turn\nProgression 0";
                break;
            case "VC holobalance_walking_1 P0":
                textNotifications.text = "Loading Virtual Coach\nExercise 8 | Walk-Horizon\nProgression 1";
                break;
            case "VC holobalance_walking_2 P0":
                textNotifications.text = "Loading Virtual Coach\nExercise 9 | Walk-Pitch\nProgression 0";
                break;
            case "CognitiveGames":
                textNotifications.text = "Loading Cognitive Games";
                break;
            default:
                textNotifications.text = "HoloBalance";
                break;
        }
    }
}
