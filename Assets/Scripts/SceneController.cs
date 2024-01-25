using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.SceneManagement;
using Weelco.SpeechControl;
using UnityEngine.Android;

[System.Serializable]
public class ScenePayload
{
    public string action;
    public string type;
    public string value;
    public string status;
    public string data;
    public bool speechRecognition = false;
    public string [] allowedRecognitionWords = null;
    public string[] allowedRecognitionWordsEnglish = null;

    public static ScenePayload FromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ScenePayload>(jsonString);
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}

public abstract class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    public static HttpServer httpServer { get; private set; }
    public static bool isDemo = false;

    protected SpeechControl speechControl = null;

    protected bool isSceneLoaded = false;
    protected bool isWindowsOS;
    
    protected bool speechRecognitionActive = false;
    protected string speechRecognitionResult = null;
    // Use this for initialization
    public virtual void Start()
    {
        Instance = this;
        if(!isDemo)
            httpServer = HttpServer.Instance();

        isWindowsOS = Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor;

        speechControl = FindObjectsOfType<SpeechControl>()[0];

        if (speechControl != null)
            speechControl.OnTranslateComplete += onTranslateComplete;
        else
            Debug.LogError("Couldn't find SpeechControl Component");

#if PLATFORM_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone) == false)
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
    }

    public virtual void Update()
    {
        if(isSceneLoaded)
        {
            isSceneLoaded = false;
            StartCoroutine(PostSceneLoaded());
        }
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // when coroutine being called from this method it is executed
        // immediately on awake and before scene loaded
        //StartCoroutine(PostSceneLoaded());
        isSceneLoaded = true;
    }

    public IEnumerator PostSceneLoaded()
    {
        ScenePayload payload = new ScenePayload
        {
            action = "Load",
            type = "Scene",
            value = SceneManager.GetActiveScene().name,
            status = "Done"
        };

        yield return httpServer.PostHttpRequest(httpServer.GetServerEndpoint(), payload.ToJSON());
    }

    /// <summary>
	/// Method that is invoked by MainThreadDispatcher used for receiving messages from server
    /// It needs to be overridden by the SceneController derived class at the current scene
	/// </summary>
	/// <param name="message">String received from server that needs to be passed toward SceneController prefab
    public virtual void OnRequest(string message)
    {
        ScenePayload payload = ScenePayload.FromJSON(message);
        switch (payload.action)
        {
            case "LoadScene":
                LoadScene(payload);
                break;
            case "Exit":
                Application.Quit();
                break;
            default:
                StartCoroutine(NotSupportedEvent(payload));
                break;
        }
    }

    protected virtual void LoadScene(ScenePayload payload)
    {
        switch (payload.value)
        {
            case "CognitiveGames":
            case "VirtualCoach":
                ConnectionManager.Instance().Standalone = false;
                GetTransitionMessage(payload);
                StartCoroutine(WaitForSceneLoad(payload.value, 5));
                break;
            default:
                StartCoroutine(NotSupportedEvent(payload));
                break;
        }
    }

    protected IEnumerator NotSupportedEvent(ScenePayload payload)
    {
        payload.status = "Not supported";
        yield return httpServer.PostHttpRequest(
            httpServer.GetServerEndpoint(),
            payload.ToJSON());
    }

    protected IEnumerator WaitForSceneLoad(string sceneName, int delay = 1)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    protected virtual void GetTransitionMessage(ScenePayload payload) { }
    protected IEnumerator RecognizeSpeech(ScenePayload payload)
    {
        speechRecognitionResult = null;
        speechRecognitionActive = true;
        speechControl.StartRecord();

        int limit = 20,
            count = 0;

        while(speechRecognitionActive && count <= limit)
        {
            yield return new WaitForSeconds(0.5f);
            count++;
        }

        payload.status = "Failed";
        if(speechRecognitionResult != null)
        {
            for(int i = 0; i < payload.allowedRecognitionWords.Length; i++)
            {
                string word = payload.allowedRecognitionWords[i];
                if (speechRecognitionResult == word)
                {
                    payload.status = "Done";
                    payload.data = payload.allowedRecognitionWordsEnglish[i];
                    break;
                }
            }
            payload.allowedRecognitionWords = null;
        }
    }

    private void onTranslateComplete(string response)
    {
        speechRecognitionActive = false;
        ResponseData result = new ResponseData(response);
        if(result.Data.Count > 0)
            speechRecognitionResult = result.Data[0].transcript.ToLower();
    }

    protected string FilterMessage(string message)
    {
        // remove null byte chars and invisible
        message = ReplaceNonPrintableCharacters(message, ' ');

        // remove everything visible not connected
        int index = message.IndexOf(" ");
        if(index > -1)
        {
            message = message.Remove(message.IndexOf(" "));
        }

        return message;
    }

    protected string ReplaceNonPrintableCharacters(string s, char replaceWith)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            byte b = (byte)c;
            if (b < 32)
                result.Append(replaceWith);
            else
                result.Append(c);
        }
        return result.ToString();
    }

}
