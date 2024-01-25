using System;
using System.Collections;
using System.Collections.Generic;
//using UnityARInterface;
using UnityEngine;

public class TestingServer : MonoBehaviour
{

    private static TestingServer _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static TestingServer Instance()
    {
        if (!Exists())
        {
            throw new Exception("TestingServer does not exist. Please ensure you have added the TestingServer component to an object in your scene.");
        }
        return _instance;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public IEnumerator PostHttpRequest(string body, System.Action<bool> callback = null)
    {
        Debug.Log("TESTING REQUEST:" + body);
        yield return true;
        if (callback != null)
        {
            callback.Invoke(true);
        }

        ScenePayload payload = ScenePayload.FromJSON(body);

        //Load first excercise
        if (CheckPayload(payload, "Load", "Scene", "StartScene"))
        {
            SendPayload("Load", "Scene", "SIEX1 P1");
        }

        //Start first excercise when scene is loaded
        if (CheckPayload(payload, "Load", "Scene", "SIEX1 P1"))
        {
            /*ARInterface arInterface = ARInterface.GetInterface();
            arInterface.StopService();
            ARInterface.SetInterface(null);*/

            //SendPayload("Start", "Game", "G6 P0");
            SendPayload("ShowAvatar", "Exercise", "Exercise");
        }

        if (CheckPayload(payload, "ShowAvatar", "Exercise", "Exercise"))
        {
            SendPayload("ShowAvatar", "Greetings", "NiceWork");
        }

        if (CheckPayload(payload, "ShowAvatar", "Greetings", "NiceWork"))
        {
            SendPayload("ShowAvatar", "Interruptions", "PleaseStop");
        }

        if (CheckPayload(payload, "ShowAvatar", "Interruptions", "PleaseStop"))
        {
            SendPayload("Start", "Game", "G6 P0");
            //SendPayload("Load", "Scene", "CognitiveGames");
        }

        if (CheckPayload(payload, "Load", "Scene", "CognitiveGames"))
        {
            //SendPayload("Start", "Game", "G6 P0");
        }
    }

    void SendPayload(string action, string type, string value)
    {
        ScenePayload payloadToSend = new ScenePayload
        {
            action = action,
            type = type,
            value = value,
            //data = httpServer.GetEndpoint()
        };
        MainThreadDispatcher.Instance().Enqueue(payloadToSend.ToJSON());
    }

    bool CheckPayload(ScenePayload payload, string action, string type, string value)
    {
        return payload.action == action && payload.type == type && payload.value == value;
    }
}
