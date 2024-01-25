using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResultsSender {

    public static string GetTimestamp()
    {
        string currentTimestamp = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
        //currentTimestamp = "2019-12-09T18:40:25.8103669+02:00";//Hardcoded for testing 
        return currentTimestamp;
    }

    public static IEnumerator PostData(string body)
    {
        string uri = "https://servlets.rrdweb.nl/r2d2/v5.1.1/project/holobalance/ext/portal/call/postTrainingResults";
        var request = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Auth-Token", "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJob2xvcF90ZXN0MDAxQGhvbG9iYWxhbmNlLnJyZHdlYi5ubCIsImlhdCI6MTU3NTg4MDQ4NCwiZXhwIjoxNTc1OTY2ODg0LCJoYXNoIjoiN0QzMUpkeUxFUkhXc3BzNnJuOXlJclNrZEE3QUJ5ckh6WEhreTFzdnZKOD0ifQ.Ax64Zx8aJtVjeCFP5diz3cBTvbioqIYkx5ycRd7gCphPgLiXQYPMkV-rYECMEvwfHc6F65cNnEIycOTVwKdJiQ");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            UnityEngine.Debug.Log("Error: " + request.error + ", " + request.responseCode + ", " + request.downloadHandler.text);
        }
        else
        {
            UnityEngine.Debug.Log("Results posted to RRD dashboard");
            UnityEngine.Debug.Log("Status Code: " + request.responseCode);
        }
    }
}
