using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Settings : MonoBehaviour {

    public string ServerIPAdress = "";
    public int ServerPort = 9001;

    private static Settings _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static Settings Instance()
    {
        if (!Exists())
        {
            throw new Exception("Settings could not find the Settings object. Please ensure you have added the Settings Prefab to your scene.");
        }
        return _instance;
    }

    private void Awake()
    {
        if(ServerIPAdress.Length == 0)
        {
            throw new Exception("Server IP address does not exist! Add address at Setting Prefab!");
        }

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
