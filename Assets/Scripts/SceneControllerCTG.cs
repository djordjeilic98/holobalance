using RogoDigital.Lipsync;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneControllerCTG : SceneControllerVirtualCoach
{
    IDictionary<string, int> idMaps = new Dictionary<string, int>();

    public static string WIN = "bph0085";
    public static string LOSE = "bph0079";

    private static SceneControllerCTG internalInstance;

    private void Awake()
    {
        idMaps.Add("holobalance_exergame_s2_sitting_1", 0);
        idMaps.Add("holobalance_exergame_s2_sitting_2", 1);
        idMaps.Add("holobalance_exergame_s2_standing_1", 2);
        idMaps.Add("holobalance_exergame_s2_standing_2", 3);
        idMaps.Add("holobalance_exergame_s2_standing_3", 4);
        idMaps.Add("holobalance_exergame_s2_walking_1", 5);
        idMaps.Add("holobalance_exergame_s2_walking_2", 6);
        idMaps.Add("holobalance_exergame_s2_walking_3", 7);
        idMaps.Add("holobalance_exergame_s2_walking_4", 8);
        idMaps.Add("holobalance_cognitive_s3_memory", 9);
        idMaps.Add("holobalance_cognitive_s3_catching_food", 10);
        idMaps.Add("holobalance_cognitive_s3_remember_previous", 11);
        idMaps.Add("holobalance_cognitive_s3_bridge_crossing", 12);
        idMaps.Add("holobalance_cognitive_s3_animal_feeding", 13);
        idMaps.Add("holobalance_cognitive_s3_preparing_animal_food", 14);

        internalInstance = this;
        DontMoveAvatar = true;
    }

    public override void Start()
    {
        base.Start();

        avatarCollection.SetActive(false);
        avatarVisible = false;
    }

    public void ShowAvatar(bool show)
    {
        avatarVisible = show;
        avatarCollection.SetActive(show);
    }

    public override void OnRequest(string message)
    {
        CancelInvoke("SendDone");
        ScenePayload payload = ScenePayload.FromJSON(message);
        switch (payload.action)
        {
            case "VirtualCoach":
            case "CognitiveGames":
                base.LoadScene(payload);
                break;
            case "CognitiveGamesStart":
                //ShowAvatar(payload);
                ConnectionManager.Instance().Standalone = false;
                StartGame(payload);
                break;
            case "ShowInstructions":
                ShowInstructions(payload);
                CheckInstructions(payload);
                break;
            case "ShowAvatar":
                ShowAvatar(payload);
                break;
            default:
                base.OnRequest(message);
                break;

                //SITTING
                //INTERRUPTIONS
                //ShowAvatar, Interruptions, MovementIsIncorecct
                //ShowAvatar, Interruptions, PleaseStop - FINISH

                //STANDING
                //ShowAvatar, Interruptions, MovementIsIncorecct
                //ShowAvatar, Interruptions, PleaseStop - FINISH

                //WALKING
                //ShowAvatar, Interruptions, MovementIsIncorecct
                //ShowAvatar, Interruptions, PleaseStop - FINISH

                //BENDING
                //ShowAvatar, Interruptions, MovementIsIncorecct
                //ShowAvatar, Interruptions, PleaseStop - FINISH
                //ShowAvatar, Event, ReachUp
                //ShowAvatar, Event, BendDown

        }
    }

    protected IEnumerator SendDone(ScenePayload payload, int delay = 0)
    {
        yield return new WaitForSeconds(delay);
        payload.status = "Done";
        StartCoroutine(ConnectionManager.Instance().PostHttpRequest(payload.ToJSON()));
    }

    protected void ShowAvatar(ScenePayload payload)
    {
        switch (payload.type)
        {
            case "Event":
                //ShowExercise(payload);
                if (payload.value == "BendDown" || payload.value == "ReachUp")
                {
                    BendingExcercise bendEx = GetComponent<GameManager>().currentGame as BendingExcercise;
                    bendEx.BendingDone();
                    payload.status = "Done";
                    StartCoroutine(ConnectionManager.Instance().PostHttpRequest(payload.ToJSON()));
                }
                if (payload.value == "DropBall")
                {
                    StandingExcercise standingEx = GetComponent<GameManager>().currentGame as StandingExcercise;
                    standingEx.DropBall();
                    StartCoroutine(SendDone(payload, 3));
                }
                break;
            default:
                StartCoroutine(NotSupportedEvent(payload));
                break;
        }
    }

    protected void CheckInstructions(ScenePayload payload)
    {
        /*LipSyncData data = null;
        data = GetLipSyncData(payload.value);
        if (data != null)
        {
            GetComponent<AudioSource>().clip = data.clip;
            GetComponent<AudioSource>().Play();
        }*/

        GameManager gm = GetComponent<GameManager>();
        switch (payload.value)
        {
            /*case "bph0053": //You lost your balance.
            case "bph0047": //The movement is incorrect.
                gm.FinishGame(false);
                //Invoke("Lose", 2);
                break;*/
            case "bph0083": //PleaseStop
                gm.FinishGame(true);
                //Invoke("Win", 2);
                break;
            case "bph0082": //PleaseStart
                gm.currentGame.RunScenario();
                break;
        }
    }

    private void Win()
    {
        GameManager gm = GetComponent<GameManager>();
        gm.FinishGame(true);
    }

    private void Lose()
    {
        GameManager gm = GetComponent<GameManager>();
        gm.FinishGame(false);
    }

    private ScenePayload lastPayload;

    void StartGame(ScenePayload payload)
    {
        int gameIndex = idMaps[payload.type];
        int progressionLevel = int.Parse(payload.value);

        //string[] values = payload.value.Split(' ');
        //Debug.Log(values[0] + ", " + values[1]);
        //int gameIndex = int.Parse(values[0].Substring(1, values[0].Length - 1));
        //int progressionLevel = int.Parse(values[1].Substring(1, values[1].Length - 1));
        lastPayload = payload;
        GetComponent<GameManager>().StartGame(gameIndex, progressionLevel);

        /*if (GetComponent<GameManager>().GetGameIndex(GetComponent<GameManager>().currentGame) < 9)
        {

            float infoLength = GetComponent<GameManager>().currentGame.GetComponent<AudioSource>().clip.length;
            //Debug.Log("Audio length: " + infoLength);

            lastPayload = payload;
            Invoke("SendDone", infoLength);
        }*/
        
    }

    public void SendDone(IEnumerator function)
    {
        StartCoroutine(ShowAndPost(function, lastPayload));
    }

    public void SendDone()
    {
        lastPayload.status = "Done";
        StartCoroutine(ConnectionManager.Instance().PostHttpRequest(lastPayload.ToJSON()));
    }

    public static string GetLocalizationPath()
    {
        return internalInstance.language + "/" + internalInstance.avatar + "/";
    }

    public IEnumerator ExergameInfo(string animationName, string infoText)
    {
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(animationName);
        if (animationName == "SIEX1P01")
        {
            yield return Exercise(SittingBird(0));
        }
        if (animationName == "SIEX2P23")
        {
            yield return Exercise(SittingBird(1));
        }
        if (animationName == "STEX1P13")
        {
            yield return Exercise(Standing1(0));
        }
        if (animationName == "STEX3P0")
        {
            yield return Exercise(Standing3P01(0));
        }
        if (animationName == "STEX3P3")
        {
            yield return Exercise(Standing3P23(0));
        }
        if (animationName == "WEX1P01")
        {
            yield return Exercise(Walking1(0));
        }
        if (animationName == "WEX2P01")
        {
            yield return Exercise(Walking2(0));
        }
        if (animationName == "WEX3P01")
        {
            yield return Exercise(Walking3(0));
        }
        if (animationName == "WEX3P23")
        {
            yield return Exercise(Walking3(2));
        }

        if (infoText != "")
        {
            yield return ShowLipSync(infoText);
        }
    }

    protected IEnumerator SittingBird(int progression = 0)
    {
        yield return SittingIntro();

        EventManager.TriggerEvent("Instructions");
        animator.SetTrigger("Instructions");

        if (progression == 0)
        {
            yield return ShowLipSync("sitting_1_info");
        } else if (progression == 1)
        {
            yield return ShowLipSync("sitting_2_info");
        }

        yield return WaitForCurrentAnimation();
    }
}
