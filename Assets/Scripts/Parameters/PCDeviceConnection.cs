using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using HoloKit;

[System.Serializable]

public class PCDeviceConnection : MonoBehaviour
{
    private int ServerPort = 8888;
    public Text text;
    public Text IPAddress;
    public HoloKitCamera HKCamera;

    TcpClient clientSocket = null;
    Thread oThread;

    public void Connect()
    {
        try
        {
            clientSocket = new TcpClient();
            clientSocket.Connect(IPAddress.text, ServerPort);
            if (clientSocket.Connected)
            {
                text.text = "Connected!";

                HKCamera.ChangeProfile();
                HKCamera.UpdateProfile();

                oThread = new Thread(new ThreadStart(getInformation));
                oThread.Start();

                IPAddress.gameObject.SetActive(false);
            }
        }
        catch (Exception ex)
        {
            text.text = ex.ToString();
        }
    }

    void getInformation()
    {
        while (true)
        {
            try
            {
                if (clientSocket != null)
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[1024];
                    networkStream.Read(bytesFrom, 0, (int)bytesFrom.Length);
                    string parameters = System.Text.Encoding.ASCII.GetString(bytesFrom);

                    text.text = "Params received: " + parameters;

                    //text.text = parameters;
                    HKCamera.SetParameters(parameters);
                    
                    networkStream.Flush();
                }
            }
            catch (Exception ex)
            {
                text.text = ex.ToString();
                oThread.Abort();
                oThread.Join();
            }

            System.Threading.Thread.Sleep(1);
        }
    }

    void stopServer()
    {
        if (oThread != null)
        {
            oThread.Abort();
        }
    }

    void OnDisable()
    {
        stopServer();
    }
}
