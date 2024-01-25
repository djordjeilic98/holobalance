using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsBuilder {

    string headerJson = "";
    string dataJson = "";

    public ResultsBuilder(string excerciseId, string startTime, float excerciseDuration)
    {
        headerJson = string.Format(@"{{
            ""results"" : {{
                ""exercise_id"": ""{0}"",
                ""start_time"": ""{1}"",
                ""exercise_duration"": ""{2}"",
                <DATA>
            }}
            }}", excerciseId, startTime, excerciseDuration);
    }

    public void AddSummary(string paramiD, string paramTitle, float paramData)
    {
        if (dataJson != "")
        {
            dataJson += ",\n";
        }
        dataJson += string.Format(@"
                ""{0}"": {{
                    ""data"": [
                        ""{2}""
                    ],
                    ""title"": ""{1}"",
                    ""labels"": [
                        ""{1}""
                    ],
                    ""type"": ""summary""
                }}", paramiD, paramTitle, paramData);
    }

    public void AddSummary(string paramiD, string paramTitle, string paramData)
    {
        if (dataJson != "")
        {
            dataJson += ",\n";
        }
        dataJson += string.Format(@"
                ""{0}"": {{
                    ""data"": [
                        ""{2}""
                    ],
                    ""title"": ""{1}"",
                    ""labels"": [
                        ""{1}""
                    ],
                    ""type"": ""summary""
                }}", paramiD, paramTitle, paramData);
    }

    public void AddBarChart(string paramiD, string paramTitle, string firstLabel, string secondLabel, List<float> ReactionTime, List<bool> CorrectAnswer)
    {
        if (dataJson != "")
        {
            dataJson += ",\n";
        }

        string barchartData = "";
        for (int i = 0; i < ReactionTime.Count; i++)
        {
            barchartData += string.Format(@"[
                            ""{0}"",
                            ""{1}"",
                            ""{2}""
                        ]", (i + 1), ReactionTime[i], (CorrectAnswer[i] ? 0.0f : 1.0f));
            if (i < ReactionTime.Count - 1)
            {
                barchartData += ",\n";
            }
        }

        dataJson += string.Format(@"
        ""{0}"": {{
                ""data"": [
                        {1}
                    ],
                    ""title"": ""{2}"",
                    ""labels"": [
                        ""{3}"",
                        ""{4}""
                    ],
                    ""type"": ""barchart""
                }}", paramiD, barchartData, paramTitle, firstLabel, secondLabel);
    }

    public void AddBarChart(string paramiD, string paramTitle, string firstLabel, List<float> TotalTime)
    {
        if (dataJson != "")
        {
            dataJson += ",\n";
        }

        string barchartData = "";
        for (int i = 0; i < TotalTime.Count; i++)
        {
            barchartData += string.Format(@"[
                            ""{0}"",
                            ""{1}""
                        ]", (i + 1), TotalTime[i]);
            if (i < TotalTime.Count - 1)
            {
                barchartData += ",\n";
            }
        }

        dataJson += string.Format(@"
        ""{0}"": {{
                ""data"": [
                        {1}
                    ],
                    ""title"": ""{2}"",
                    ""labels"": [
                        ""{3}""
                    ],
                    ""type"": ""barchart""
                }}", paramiD, barchartData, paramTitle, firstLabel);
    }

    public string getJSON() {
        return headerJson.Replace("<DATA>", dataJson);
    }
}
