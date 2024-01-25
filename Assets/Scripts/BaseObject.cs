using UnityEngine;
using System;

public class BaseObject : MonoBehaviour
{
    private static BaseObject _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static BaseObject Instance()
    {
        if (!Exists())
        {
            throw new Exception("MainThreadDispatcher could not find the MainThreadDispatcher object. Please ensure you have added the MainThreadDispatcher Prefab to your scene.");
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

    void OnDestroy()
    {
        _instance = null;
    }
}
