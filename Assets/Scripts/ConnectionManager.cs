using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {

    public enum ServerType
    {
        PRODUCTION,
        TESTING,
        DUMMY
    };

    public bool Standalone = false;

    public ServerType serverType = ServerType.PRODUCTION;

    private static ConnectionManager _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static ConnectionManager Instance()
    {
        if (!Exists())
        {
            throw new Exception("ConnectionManager does not exist. Please ensure you have added the ConnectionManager component to an object in your scene.");
        }
        return _instance;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public IEnumerator PostHttpRequest(string body, System.Action<bool> callback = null)
    {
        if (serverType == ServerType.PRODUCTION)
        {
            yield return HttpServer.Instance().PostHttpRequest(HttpServer.Instance().GetServerEndpoint(), body, callback);
        }
        else if (serverType == ServerType.TESTING)
        {
            yield return TestingServer.Instance().PostHttpRequest(body, callback);
        }
        else
        {
            Debug.Log("PostHttpRequest: " + body);
            yield return true;
            if (callback != null)
            {
                callback.Invoke(true);
            }
        }
    }
}
