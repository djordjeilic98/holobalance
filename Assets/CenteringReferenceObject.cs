using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenteringReferenceObject : MonoBehaviour
{
    public static CenteringReferenceObject Instance;

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

    // Start is called before the first frame update
    void Start()
    {
        if (CenteringManager.Instance)
        {
            CenteringManager.Instance.transform.position = transform.position;
            CenteringManager.Instance.transform.rotation = transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
