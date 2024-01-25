using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

//https://stackoverflow.com/questions/36526332/simple-socket-server-in-unity/36526634#36526634
//http://hanslen.me/2017/01/06/Leap-Motion-brings-gesture-control-to-iPhone/

[System.Serializable]
public struct ARParameters
{
    public float screenWidth;
    public float screenHeight;
    public float screenBottom;
    public Vector3 cameraOffset;
    public float eyeDistance;
    public Vector3 mrOffset;
    public float fieldOfView;
    public float distortion;
    public float viewWidth;
    public float viewHeight;
    public float lensLength;
    public float toScreenDist;
    public float toEyeDist;
    public float viewBottom;
}

public class ParameterController : MonoBehaviour
{
    public string ServerAddress = "192.168.137.50"; //HP SABA
    //private string ServerAddress = "192.168.178.23"; // home
    //private string ServerAddress = "172.21.63.22"; // eduroam
    //private string ServerAddress = "172.20.10.3"; // hotspot
    private int ServerPort = 8888;

    [Space(10)]
    [Header("AR Camera Parameters")]
    [Range(0.0f, 100.0f)] public float screenWidth = 0.135f;
    [Range(0.0f, 100.0f)] public float screenHeight = 0.062f;
    [Range(0.0f, 100.0f)] public float screenBottom = 0.01f; // cover to bottom hole
    [Range(-100.0f, 100.0f)] public float cameraOffsetX = 0.060f;
    [Range(-100.0f, 100.0f)] public float cameraOffsetY = 0.04f;
    [Range(-100.0f, 100.0f)] public float cameraOffsetZ = -0.009f;
    [Range(-10.0f, 10.0f)] public float eyeDistance = 0.065f;
    [Range(0.0f, 100.0f)] public float lensLength = 0.090f;
    [Range(0.0f, 100.0f)] public float fieldOfView = 0f;
    [Range(-10.0f, 10.0f)] public float distortion = 0f;
    [Range(-100.0f, 100.0f)] public float viewBottom = 0.00f;
    [Range(-100.0f, 100.0f)] public float toEyeDist = 0.06f; // this must not be changed (produces distortion)
    [Range(0.0f, 100.0f)] public float viewWidth = 0.0696f;
    [Range(0.0f, 100.0f)] public float viewHeight = 0.0648f;
    [Range(-100.0f, 100.0f)] public float mrOffsetX = 0.0f; 
    [Range(-100.0f, 100.0f)] public float mrOffsetY = -0.041f; // 0.5f
    [Range(-100.0f, 100.0f)] public float mrOffsetZ = -0.092f; // if z increases, size increases (but moves closer)
    [Range(-100.0f, 100.0f)] public float toScreenDist = 0.06f;
    //  z.offset    to.screen.distance
    //  -0.092      0.06                       ---> correct position, small, not moving
    //  0.6         0.3                        ---> correct position, small, not moving
    [Range(-100.0f, 100.0f)] public float MatrixParam1 = 0.04f;
    [Range(-100.0f, 100.0f)] public float MatrixParam2 = 0.037f;
    [Range(-100.0f, 100.0f)] public float MatrixParam3 = 0.005f;
    [Range(-100.0f, 100.0f)] public float MatrixParam4 = 0.1f;

    //[Space(10)]
    //[Header("Avatar Transform")]
    [Range(-2.0f, 2.0f)] float XOffset = 0.0f;
    [Range(-2.0f, 2.0f)] float YOffset = 0.0f;
    [Range(-2.0f, 2.0f)] float ZOffset = 0.0f;
    [Range(0.0f, 2.0f)] float XScaleFactor = 1.0f;
    [Range(0.0f, 2.0f)] float YScaleFactor = 1.0f;
    [Range(0.0f, 2.0f)] float ZScaleFactor = 1.0f;

    [Space(10)]

    TcpListener serverSocket = null;
    TcpClient clientSocket = null;
    Thread oThread;

    void Start()
    {
        try
        {
            serverSocket = new TcpListener(IPAddress.Parse(ServerAddress), ServerPort);
            serverSocket.Start();
            Debug.Log("Server started! " + serverSocket.Server.LocalEndPoint.ToString());
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }

        oThread = new Thread(new ThreadStart(AcceptClient));
        oThread.Start();
    }

    private void AcceptClient()
    {
        while(true)
        {
            try
            {
                clientSocket = serverSocket.AcceptTcpClient();
                if (clientSocket != null) Debug.Log("Client connected!");
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
                oThread.Abort();
                oThread.Join();
            }

            System.Threading.Thread.Sleep(1);
        }
    }

    public bool IsDeviceConnected()
    {
        return clientSocket != null;
    }

    public void SendParameters()
    {
        try
        {
            if (IsDeviceConnected())
            {
                NetworkStream serverStream = clientSocket.GetStream();
                string tokens = 
                (
                    screenWidth
                    + "," + screenHeight
                    + "," + screenBottom
                    + "," + cameraOffsetX
                    + "," + cameraOffsetY
                    + "," + cameraOffsetZ
                    + "," + eyeDistance
                    + "," + lensLength
                    + "," + fieldOfView
                    + "," + distortion
                    + "," + viewBottom
                    + "," + toEyeDist
                    + "," + viewWidth
                    + "," + viewHeight
                    + "," + mrOffsetX
                    + "," + mrOffsetY
                    + "," + mrOffsetZ
                    + "," + toScreenDist
                    + "," + XOffset
                    + "," + YOffset
                    + "," + ZOffset
                    + "," + XScaleFactor
                    + "," + YScaleFactor
                    + "," + ZScaleFactor
                    + "," + MatrixParam1
                    + "," + MatrixParam2
                    + "," + MatrixParam3
                    + "," + MatrixParam4
                );
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(tokens);

                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                Debug.Log("Message sent! " + tokens);
            }
            else
            {
                Debug.Log("Device not connected yet!");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void OnGUI()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            SendParameters();
        }
    }

    void stopServer()
    {
        serverSocket.Stop();

        if (oThread != null)
        {
            oThread.Interrupt();
        }
    }

    void OnDisable()
    {
        stopServer();
    }
}
