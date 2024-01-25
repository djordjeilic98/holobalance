using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    static public bool isEnabled = false;
    public string sourceName;
    public string targetName;

    protected Transform target;
    protected Transform source;
    protected Transform initTrasform;

    private void Start()
    {
        source = GameObject.Find(sourceName).transform;
        target = GameObject.Find(targetName).transform;

        initTrasform = source.transform;
    }

    void LateUpdate()
    {
        if(isEnabled)
        {
            source.transform.LookAt(target);
        }
        else
        {
            source.transform.rotation = initTrasform.rotation;
        }
    }
}
