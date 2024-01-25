using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class EdgeClient : MonoBehaviour {
    #region private members
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion
    // Use this for initialization    

    void Start()
    {
        Debug.Log("EdgeClient Start()");
        ConnectToTcpServer();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToServer("Client send message to server");
        }
    }
    /// <summary>
    /// Setup socket connection. 	
    /// </summary>
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(Settings.Instance().ServerIPAdress, Settings.Instance().ServerPort);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incoming stream into byte array. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                        MainThreadDispatcher.Instance().Enqueue(serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary>
    /// <param name="message">String to be sent to server
    public virtual void SendMessageToServer(String message)
    {
        if (socketConnection == null || message.Length == 0)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message: " + message);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private static EdgeClient _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static EdgeClient Instance()
    {
        if (!Exists())
        {
            throw new Exception("EdgeClient could not find the EdgeClient object. Please ensure you have added the EdgeClient Prefab to your scene.");
        }
        return _instance;
    }

    private void OnDestroy()
    {         
        clientReceiveThread.Abort();
        _instance = null;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
}
