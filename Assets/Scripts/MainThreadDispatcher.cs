using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method.
/// It is used by the separate thread from EdgeClient prefab which forms connection with EdgeServer
/// All messages received by the server via EdgeClient`s separate thread are passed
/// to the unity`s main thread via this class and MainThreadDispatcher::Enqueue(String message) method
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static MainThreadDispatcher Instance()
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
            return;
        }
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
	/// Locks the queue and adds Action to the queue which when invoked send message to the existing SceneController derived class at scene
	/// </summary>
	/// <param name="message">String received from server that needs to be passed toward SceneController prefab
    public void Enqueue(String message)
    {
        this.Enqueue(() => SceneController.Instance.OnRequest(message));
    }

    public void Log(String message)
    {
        Debug.Log(message);
    }

    /// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    public void Enqueue(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => {
                StartCoroutine(action);
            });
        }
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public void Enqueue(Action action)
    {
        Enqueue(ActionWrapper(action));
    }

    IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }   
}