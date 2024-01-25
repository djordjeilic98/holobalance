using System;
using System.IO;
using System.Threading;
using System.Collections;

using System.Net;
using System.Net.Sockets;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(MainThreadDispatcher))]
public class HttpServer : MonoBehaviour
{
    private static HttpServer _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static HttpServer Instance()
    {
        if (!Exists())
        {
            throw new Exception("HttpServer could not find the MainThreadDispatcher object. Please ensure you have added the HttpServer Prefab to your scene.");
        }
        return _instance;
    }

    

    void OnDestroy()
    {
        if (_instance == this) {
            _instance = null;
        }
    }

    public string Port = "8010";
    public int EdgeServerAddress = 8;
    public string EdgeServerPort = "3063";

    private HttpListener listener;
    private Thread listenerThread;

    //void ScanLan()
    //{
    //    object lockObj = new object();
    //    int upCount = 0;
    //
    //    string ipBase = "192.168.1.";
    //    for (int i = 1; i < 255; i++)
    //    {
    //        string ip = ipBase + i.ToString();
    //        System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
    //        System.Net.NetworkInformation.PingReply reply = p.Send(ip, 500);
    //        if (reply != null && reply.Status == IPStatus.Success)
    //        {
    //            //Debug.Log(String.Format("{0} ", ip));
    //            MainThreadDispatcher.Instance().Log(String.Format("{0} ", ip));
    //            lock (lockObj)
    //            {
    //                upCount++;
    //            }
    //        }
    //        else if(reply == null)
    //        {
    //            MainThreadDispatcher.Instance().Log(String.Format("Pinging {0} failed. (Null Reply object?)", ip));
    //            //Debug.Log(String.Format("Pinging {0} failed. (Null Reply object?)", ip));
    //        }
    //    }
    //    MainThreadDispatcher.Instance().Log(String.Format("Scanning done. Found: {0})", upCount));
    //    //Debug.Log(String.Format("Scanning done. Found: {0})", upCount));
    //}
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            var ip = PlayerPrefs.GetInt("EdgeServerAddress");
            EdgeServerAddress = PlayerPrefs.GetInt("EdgeServerAddress");

            listener = new HttpListener();
            listener.Prefixes.Add("http://+:" + Port + "/client/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listener.Start();

            listenerThread = new Thread(StartListener);
            listenerThread.Start();
            //UnityEngine.Debug.Log("Server Started");

            return;
        }
        Destroy(this.gameObject);
    }

    void Start()
    {
        
    }

    public static string GetLocalIP()
    {
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint.Address.ToString();
        }
    }

    public string GetEndpoint()
    {
        return GetLocalIP() + ":" + Port + "/client";
    }

    public string GetServerIP()
    {
        var ipaddress = IPAddress.Parse(GetLocalIP()).GetAddressBytes();
        ipaddress[3] = (byte)EdgeServerAddress;
        //UnityEngine.Debug.Log(ipaddress);

        return new IPAddress(ipaddress).ToString();
    }
    
    public string GetServerEndpoint()
    {
        return "http://" + GetServerIP() + ":" + EdgeServerPort + "/server/";
    }

    public void GetRequest(string uri)
    {
        StartCoroutine(GetHttpRequest(uri));
    }

    private void StartListener()
    {
        while (true)
        {
            var result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne();
        }
    }

    private void ListenerCallback(IAsyncResult result)
    {
        var context = listener.EndGetContext(result);

        UnityEngine.Debug.Log("Method: " + context.Request.HttpMethod);
        UnityEngine.Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);

        if (context.Request.QueryString.AllKeys.Length > 0)
            foreach (var key in context.Request.QueryString.AllKeys)
            {
                UnityEngine.Debug.Log("Key: " + key + ", Value: " + context.Request.QueryString.GetValues(key)[0]);
            }

        if (context.Request.HttpMethod == "POST")
        {
            //Thread.Sleep(1000);
            var data_text = new StreamReader(context.Request.InputStream,
                                context.Request.ContentEncoding).ReadToEnd();
            UnityEngine.Debug.Log(data_text);
            MainThreadDispatcher.Instance().Enqueue(data_text);
        }
        context.Response.Close();
    }

    IEnumerator GetHttpRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(new Uri(uri)))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                UnityEngine.Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                UnityEngine.Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator PostHttpRequest(string uri, string body, System.Action<bool> callback = null)
    {
        var request = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            if (callback != null)
                callback.Invoke(false);

            //UnityEngine.Debug.Log("Error: " + request.error);
        }
        else
        {
            if (callback != null)
                callback.Invoke(true);
            UnityEngine.Debug.Log("OK");
            UnityEngine.Debug.Log("Status Code: " + request.responseCode);
        }
    }
}
