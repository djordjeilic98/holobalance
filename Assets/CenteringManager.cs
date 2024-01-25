using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenteringManager : MonoBehaviour
{
    public static CenteringManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
