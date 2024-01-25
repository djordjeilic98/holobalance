using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using HoloKit;
using RogoDigital.Lipsync;

public class SceneControllerVirtualCoach : SceneController
{
    public bool currentAnimationPlaying;
    protected IEnumerator WaitForCurrentAnimation()
    {
        // changing currentAnimationPlaying to true does not happen immediately
        // so when method called right after starting animation flag can be false
        // wait for some time to ensure flag has changed
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        yield return wait;
        while (currentAnimationPlaying)
            yield return wait;
    }

    public GameObject avatarCollection;
    public GameObject avatarMale;
    public GameObject avatarFemale;
    public static GameObject avatarCurrent;
    public GameObject chair;
    public GameObject airexPad;
    public GameObject cardFixed;
    public GameObject cardMoving;
    public GameObject avatarLight;

    public GameObject demoControls;

    protected Animator animator;
    static protected Vector3 startCollectionPosition;
    static protected Quaternion startCollectionRotation;

    protected LipSync lipSync;

    protected bool preExerciseInstructions;
    protected int delayBetweenAnimations = 20;
    protected int animationDelay = 3;
    protected bool isAvatarOnSide = false;
    protected float speedUpExercise = 2.0f;

    protected string language,
                     avatar,
                     currentExercise;

    protected bool avatarVisible = true;

    protected bool DontMoveAvatar = false;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        currentAnimationPlaying = false;
        preExerciseInstructions = false;

        language = PlayerPrefs.GetString("Language");
        avatar = PlayerPrefs.GetString("Avatar");

        Dictionary<string, string> languageCodes = new Dictionary<string, string>()
        {
            {"EN",  "en-US"},
            {"DE",  "de-DE"},
            {"GR",  "el-GR"},
            {"PT",  "pt-PT"},
            {"IT",  "it-IT"},
        };

        speechControl.UseLanguageCode = true;
        speechControl.LanguageStr = languageCodes[language];
        speechControl.trimSilence = false;

        if (chair)
        {
            chair.SetActive(false);
        }
        if (avatarFemale && avatarMale)
        {
            if (avatar == "Female")
            {
                avatarFemale.SetActive(true);
                avatarMale.SetActive(false);
                avatarCurrent = avatarFemale;
            }
            else
            {
                avatarFemale.SetActive(false);
                avatarMale.SetActive(true);
                avatarCurrent = avatarMale;
            }
        }

        animator = GetComponentInChildren<Animator>();
        lipSync = GetComponentInChildren<LipSync>();

        if (demoControls)
        {
            demoControls.SetActive(isDemo);
        }

        GameObject ambientLight = GameObject.Find("AmbientLight");
        if (ambientLight)
        {
            ambientLight.GetComponent<Light>().intensity = 1.5f;
        }

        if (isDemo || isWindowsOS)
        {
            if (avatarLight)
            {
                avatarLight.SetActive(true);
            }
        }
    }

    public override void OnRequest(string message)
    {
        ScenePayload payload = ScenePayload.FromJSON(message);
        switch (payload.action)
        {
            case "ShowExercise":
                ShowExercise(payload);
                break;
            case "ShowInstructions":
                ShowInstructions(payload);
                break;
            default:
                base.OnRequest(message);
                break;
        }
    }

    protected void PostExerciseSetup()
    {
        switch(currentExercise)
        {
            case "VC holobalance_sitting_1 P0":
            case "VC holobalance_sitting_1 P1":
            case "VC holobalance_sitting_2 P0":
            case "VC holobalance_sitting_2 P1":
            case "VC holobalance_sitting_2 P2":
            case "VC holobalance_sitting_2 P3":
                if(!isWindowsOS)
                    cardFixed.SetActive(true);
                break;
            case "VC holobalance_sitting_1 P2":
            case "VC holobalance_sitting_1 P3":
                if (!isWindowsOS)
                    cardMoving.SetActive(true);
                break;
        }
    }

    protected void SetupSceneAssets()
    {
        avatarCurrent.transform.Find("Walking Targets").gameObject.SetActive(false);
        cardFixed.SetActive(false);
        cardMoving.SetActive(false);
        airexPad.SetActive(false);
        chair.SetActive(false);

        if (currentExercise.Contains("sitting"))
            chair.SetActive(true);
        else if (currentExercise.Contains("holobalance_standing_2"))
            airexPad.SetActive(true);
        else if (currentExercise.Contains("VC holobalance_walking_2 P1"))
            avatarCurrent.transform.Find("Walking Targets").gameObject.SetActive(true);
    }

    protected void SetupAvatarPosition()
    {
        if (isWindowsOS)
            if (currentExercise.Contains("sitting"))
            {
                if (isDemo)
                    avatarCollection.transform.position = new Vector3(0.0f, -1.0f, 2.5f);
                else
                    avatarCollection.transform.position = new Vector3(0.0f, -1.0f, 2.5f);
            }
            else if (currentExercise.Contains("standing"))
                avatarCollection.transform.position = new Vector3(0.0f, -1.0f, 2.5f);
            else if (currentExercise.Contains("walking"))
                avatarCollection.transform.position = new Vector3(0.0f, -1.0f, 6.0f);
    }

    protected void SetupAnimator()
    {
        switch (currentExercise)
        {
            // sit yaw - hand up
            case "VC holobalance_sitting_1 P0":
            // sit yaw - hand up - increase speed
            case "VC holobalance_sitting_1 P1":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SIEX1P01");
                break;
            // sit yaw head and hand
            case "VC holobalance_sitting_1 P2":
            // sit yaw head and hand - increase speed
            case "VC holobalance_sitting_1 P3":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SIEX1P23");
                break;
            // sit pitch - hand up
            case "VC holobalance_sitting_2 P0":
            // sit pitch hand up - increase speed
            case "VC holobalance_sitting_2 P1":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SIEX2P01");
                break;
            // sit pitch - hand down - wider move
            case "VC holobalance_sitting_2 P2":
            // sit pitch - hand down - wider move - increase speed
            case "VC holobalance_sitting_2 P3":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SIEX2P23");
                break;
            // sit bend - eyes open
            case "VC holobalance_sitting_3 P0":
            // sit bend - eyes closed
            case "VC holobalance_sitting_3 P1":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SIEX3P01");
                break;
            // sit bend sideways - eyes open
            case "VC holobalance_sitting_3 P2":
            // sit bend sideways - eyes closed
            case "VC holobalance_sitting_3 P3":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SIEX3P23");
                break;
            // stand maintain balance - eyes open - feet width apart
            case "VC holobalance_standing_1 P0":
            // stand maintain balance - eyes closed - feet width apart
            case "VC holobalance_standing_1 P2":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX1P02");
                break;
            // stand maintain balance - eyes open - feet closed together*
            case "VC holobalance_standing_1 P1":
            // stand maintain balance - eyes closed - feet closed together
            case "VC holobalance_standing_1 P3":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX1P13");
                break;
            // stand maintain balance on cushion - eyes open - feet width apart
            case "VC holobalance_standing_2 P0":
            // stand maintain balance on cushion - eyes closed - feet width apart
            case "VC holobalance_standing_2 P2":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX2P01"); // ???
                break;
            // maintain balance on cushion - eyes open - feet closed together
            case "VC holobalance_standing_2 P1":
            // maintain balance on cushion - eyes closed - feet closed together
            case "VC holobalance_standing_2 P3":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX2P13");
                break;
            // stand bend - feet width apart 
            case "VC holobalance_standing_3 P0":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX3P0");
                break;
            // stand bend - feet closed together*
            case "VC holobalance_standing_3 P1":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX3P1");
                break;
            // stand reach
            case "VC holobalance_standing_3 P2":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX3P2");
                break;
            // stand reach - heels rise
            case "VC holobalance_standing_3 P3":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX3P3");
                break;
            // stand turn
            case "VC holobalance_standing_4 P0":
            case "VC holobalance_standing_4 P1":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("STEX4P01");
                break;
            // walk horizon
            case "VC holobalance_walking_1 P0":
            // walk horizon - increase speed
            case "VC holobalance_walking_1 P1":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("WEX1P01");
                break;
            // walk yaw
            case "VC holobalance_walking_2 P0":
            // walk yaw - increase speed
            case "VC holobalance_walking_2 P1":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("WEX2P01");
                break;
            // walk yaw - wider
            case "VC holobalance_walking_2 P2":
            // walk yaw - wider - increase speed
            case "VC holobalance_walking_2 P3":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("WEX2P23");
                break;
            // walk pitch
            case "VC holobalance_walking_3 P0":
            // walk pitch - increase speed
            case "VC holobalance_walking_3 P1":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("WEX3P01");
                break;
            // walk v *
            case "VC holobalance_walking_3 P2":
            // walk v - increase speed
            case "VC holobalance_walking_3 P3":
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("WEX3P23");
                break;
        }
        animator.SetFloat("exercise speed", 1.0f);
    }

    protected void SetupScene()
    {      
        SetupSceneAssets();
        SetupAvatarPosition();
        SetupAnimator();
    }

    protected int GetProgression(string exercise)
    {
        return System.Int32.Parse(exercise[exercise.Length - 1].ToString());
    }

    protected virtual void ShowExercise(ScenePayload payload)
    {
        string exercise = payload.value;
        if (exercise != "RepeatExercise")
            currentExercise = exercise;

        SetupScene();

        switch (exercise)
        {
            case "VC holobalance_sitting_1 P0":
            case "VC holobalance_sitting_1 P1":
            case "VC holobalance_sitting_1 P2":
            case "VC holobalance_sitting_1 P3":
                StartCoroutine(ShowAndPost(Exercise(Sitting1(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_sitting_2 P0":
            case "VC holobalance_sitting_2 P1":
            case "VC holobalance_sitting_2 P2":
            case "VC holobalance_sitting_2 P3":
                StartCoroutine(ShowAndPost(Exercise(Sitting2(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_sitting_3 P0":
            case "VC holobalance_sitting_3 P1":
            case "VC holobalance_sitting_3 P2":
            case "VC holobalance_sitting_3 P3":
                StartCoroutine(ShowAndPost(Exercise(Sitting3(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_standing_1 P0":
            case "VC holobalance_standing_1 P1":
            case "VC holobalance_standing_1 P2":
            case "VC holobalance_standing_1 P3":
                StartCoroutine(ShowAndPost(Exercise(Standing1(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_standing_2 P0":
            case "VC holobalance_standing_2 P1":
            case "VC holobalance_standing_2 P2":
            case "VC holobalance_standing_2 P3":
                StartCoroutine(ShowAndPost(Exercise(Standing2(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_standing_3 P0":
            case "VC holobalance_standing_3 P1":
                StartCoroutine(ShowAndPost(Exercise(Standing3P01(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_standing_3 P2":
            case "VC holobalance_standing_3 P3":
                StartCoroutine(ShowAndPost(Exercise(Standing3P23(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_standing_4 P0":
            case "VC holobalance_standing_4 P1":
                StartCoroutine(ShowAndPost(Exercise(Standing4(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_walking_1 P0":
            case "VC holobalance_walking_1 P1":
                StartCoroutine(ShowAndPost(Exercise(Walking1(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_walking_2 P0":
            case "VC holobalance_walking_2 P1":
            case "VC holobalance_walking_2 P2":
            case "VC holobalance_walking_2 P3":
                StartCoroutine(ShowAndPost(Exercise(Walking2(GetProgression(exercise))), payload));
                break;
            case "VC holobalance_walking_3 P0":
            case "VC holobalance_walking_3 P1":
            case "VC holobalance_walking_3 P2":
            case "VC holobalance_walking_3 P3":
                StartCoroutine(ShowAndPost(Exercise(Walking3(GetProgression(exercise))), payload));
                break;
            case "RepeatExercise":
                StartCoroutine(ShowAndPost(RepeatExercise(), payload));
                break;
            default:
                StartCoroutine(NotSupportedEvent(payload));
                break;
        }
    }

    protected IEnumerator Exercise(IEnumerator ex, int delay = 0)
    {
        MoveAvatarInFront();
        yield return new WaitForSeconds(delay);

        if (preExerciseInstructions)
            yield return ShowLipSync("bph0017");

        yield return ex;

        // second round of exercises
        StartInstructions();
        yield return WaitForCurrentAnimation();
        // This is required if eyes were closed
        BlinkControl.Instance.OpenEyes();
        // This is required only for standing ex 2
        animator.SetTrigger("StepDown");
        yield return WaitForCurrentAnimation();

        MoveAvatarOnSide();
        yield return ShowLipSync("bph0084");
    }

    protected void SpeedUpExercise(float multiplier = 2.0f)
    {
        animator.SetFloat("exercise speed", multiplier);
    }

    #region Sitting Exercises

    protected IEnumerator SittingIntro()
    {
        // Sit up with your back straight, as much as possible, think tall and try to keep shoulders relaxed.
        yield return ShowLipSync("bph0001");
        // Your bottom should touch the back of your chair.
        yield return ShowLipSync("bph0002");
        // Try to equally distribute your weight on both hips.
        yield return ShowLipSync("bph0003");
        // Feet should be kept flat on the floor.
        yield return ShowLipSync("bph0004");
    }
    protected IEnumerator Sitting1(int progression = 0)
    {
        yield return SittingIntro();

        if (progression == 1 || progression == 3)
            SpeedUpExercise();

        StartInstructions();
        if (progression == 0 || progression == 1)
            // While focusing on a word placed at eye level and arm's length, turn your head left and right always trying to keeping the target in focus for 1 minute.
            yield return ShowLipSync("bph0005");
        else
            // While focusing on a card placed at eye level and arm's length, turn both your head and the card left and right in opposite directions always trying to keeping the target in focus
            yield return ShowLipSync("bph0006");

        if (progression == 1 || progression == 3)
            // Gradually increase the speed as the exercise becomes easier until you are able to practice at a very fast speed.
            yield return ShowLipSync("bph0009");
        yield return WaitForCurrentAnimation();
    }

    protected IEnumerator Sitting2(int progression = 0)
    {
        yield return SittingIntro();

        if (progression == 1 || progression == 3)
            SpeedUpExercise();

        StartInstructions();
        if (progression == 0 || progression == 1)
            // While focusing on a word placed at eye level and arm's length, turn your head up and down always trying to keeping the target in focus for 1 minute.
            yield return ShowLipSync("bph0007");
        else
            // Move your head up to look at the ceiling and down to look at the ground for 1 minute.
            yield return ShowLipSync("bph0008");

        if (progression == 1 || progression == 3)
            // Gradually increase the speed as the exercise becomes easier until you are able to practice at a very fast speed.
            yield return ShowLipSync("bph0009");

        yield return WaitForCurrentAnimation();
    }

    protected IEnumerator Sitting3(int progression = 0)
    {
        yield return SittingIntro();

        // Try to maintain your head in an upright position, looking straight ahead. No bending forward or back in the starting position.
        yield return ShowLipSync("bph0010");

        if(progression == 1 || progression == 3)
            BlinkControl.Instance.CloseEyes();

        StartInstructions();

        if (progression == 0 || progression == 1)
            // The aim is to bend over as if to pick up an object off the floor and return to upright position.
            yield return ShowLipSync("bph0011");
        else
            // The aim is to bend over sideways as if to pick up an object off the floor and return to upright position.
            yield return ShowLipSync("bph0012");
        
        // As you bend over make sure to bring chin to chest and return head to normal upright as you come up.
        yield return ShowLipSync("bph0013");
       
        if (progression == 0 || progression == 2)
            // Keep your eyes open.
            yield return ShowLipSync("bph0014");
        else
            // Keep your eyes closed.
            yield return ShowLipSync("bph0015");
        
        // Repeat 5 times.
        yield return ShowLipSync("bph0016");

        yield return WaitForCurrentAnimation();
    }
    #endregion

    #region Standing Exercises
    protected IEnumerator Standing1(int progression = 0)
    {
        // Stand with your head and upper body in an upright position.
        yield return ShowLipSync("bph0018");
        if (progression == 0 || progression == 2)
            // Place your feet hip width apart.
            yield return ShowLipSync("bph0019");
        else
            // Stand with feet closed together, if possible, heels and toes touching.
            yield return ShowLipSync("bph0020");

        if (progression == 0 || progression == 1)
            // Keep your eyes open.
            yield return ShowLipSync("bph0014");
        else
        {
            // Keep your eyes closed.
            BlinkControl.Instance.CloseEyes();
            yield return ShowLipSync("bph0015");
        }
            

        // For this exercise, you are going to maintain your balance in this position for one minute.
        yield return ShowLipSync("bph0021");

        // As soon as you feel pressure on the toes or heels return to the center position with weight across the ball of the foot.
        yield return ShowLipSync("bph0022");
    }

    protected IEnumerator Standing2(int progression = 0)
    {
        // Stand with your head and upper body in an upright position.
        yield return ShowLipSync("bph0018");

        StartInstructions();
        // Stand on the foam cushion.
        yield return ShowLipSync("bph0023");
        if (progression == 0 || progression == 2)
            // Place your feet hip width apart.
            yield return ShowLipSync("bph0019");
        else
            // Stand with feet closed together, if possible, heels and toes touching.
            yield return ShowLipSync("bph0020");

        if (progression == 0 || progression == 1)
            // Keep your eyes open.
            yield return ShowLipSync("bph0014");
        else
        {
            // Keep your eyes closed.
            BlinkControl.Instance.CloseEyes();
            yield return ShowLipSync("bph0015");
        }

        // For this exercise, you are going to maintain your balance in this position for one minute.
        yield return ShowLipSync("bph0021");

        // As soon as you feel pressure on the toes or heels return to the center position with weight across the ball of the foot.
        yield return ShowLipSync("bph0022");

        animator.SetTrigger("StepDown");
        yield return WaitForCurrentAnimation();
    }

    protected void StartInstructions()
    {
        EventManager.TriggerEvent("Instructions");
        animator.SetTrigger("Instructions");
    }

    protected IEnumerator Standing3P01(int progression = 0)
    {
        // Stand with your head and upper body in an upright position.
        yield return ShowLipSync("bph0018");
        if (progression == 0)
            // Place your feet hip width apart.
            yield return ShowLipSync("bph0019");
        else
            // Stand with feet closed together, if possible, heels and toes touching.
            yield return ShowLipSync("bph0020");

        // For this exercise, you are going to bend over as if to pick up an object off the floor.
        yield return ShowLipSync("bph0024");

        StartInstructions();

        // As you bend over make sure to bring chin to chest and return head to normal upright as you come up.
        yield return ShowLipSync("bph0013");

        yield return WaitForCurrentAnimation();
    }

    protected IEnumerator Standing3P23(int progression = 0)
    {
        // Stand with your head and upper body in an upright position.
        yield return ShowLipSync("bph0018");
        // Place your feet hip width apart.
        yield return ShowLipSync("bph0019");

        // For this exercise, you are going to try to Reach up as if to take an item out of a cupboard.
        yield return ShowLipSync("bph0025");

        StartInstructions();

        if (progression == 2)
            // Tilt head back slightly as you reach up.
            yield return ShowLipSync("bph0026");
        else
            // Tilt head back slightly as you reach up and lift your heels off the floor (heel rise).
            yield return ShowLipSync("bph0027");

        // Repeat 5 times.
        yield return ShowLipSync("bph0016");

        yield return WaitForCurrentAnimation();
    }

    protected IEnumerator Standing4(int progression = 0)
    {
        // Stand with your head and upper body in an upright position.
        yield return ShowLipSync("bph0018");
        // Place your feet hip width apart.
        yield return ShowLipSync("bph0019");

        // For this exercises, you are going to turn 180 degrees to face the opposite direction.
        yield return ShowLipSync("bph0028");

        if (progression == 1) SpeedUpExercise(1.25f);
        StartInstructions();

        // When turning to the left, start with your left foot. When turning to the right, start with your right foot. As you turn try to take big steps, and keep your weight over the ball
        yield return ShowLipSync("bph0029");

        // Try not to swivel turn.
        yield return ShowLipSync("bph0030");

        // You will perform 3 turns to the left and 3 turns to the right.
        yield return ShowLipSync("bph0031");

        yield return WaitForCurrentAnimation();
    }

    #endregion

    #region Walking Exercises
    protected IEnumerator Walking1(int progression = 0)
    {
        // Stand with your head and upper body in an upright position.
        yield return ShowLipSync("bph0018");
        // Place your feet hip width apart.
        yield return ShowLipSync("bph0019");

        if (progression == 1) SpeedUpExercise(1.5f);
        StartInstructions();

        // Walk across the room for one minute making sure to look out straight ahead at the horizon, but do not fixate your gaze on an object.
        yield return ShowLipSync("bph0032");
        // Try to avoid looking down at the floor constantly.
        yield return ShowLipSync("bph0033");
        // Try to make sure to shift your body weight forward with each step and swing your arms.
        yield return ShowLipSync("bph0034");

        if (progression == 1)
            // Gradually increase the walking speed as the exercise becomes easier.
            yield return ShowLipSync("bph0035");
        yield return WaitForCurrentAnimation();
    }

    protected IEnumerator Walking2(int progression = 0)
    {
        // Stand with your head and upper body in an upright position.
        yield return ShowLipSync("bph0018");

        // Place your feet hip width apart.
        yield return ShowLipSync("bph0019");

        if (progression == 1 || progression == 3) SpeedUpExercise(1.5f);
        StartInstructions();

        if (progression == 0 || progression == 1)
            // Walk across the room for one minute making sure to turn your head left and right to look back and forth between the two targets.
            yield return ShowLipSync("bph0036");
        else
            // Walk across the room for one minute making sure to turn your head left and right scanning the room as when crossing the road .
            yield return ShowLipSync("bph0037");

        // Try to make sure to shift your body weight forward with each step and swing your arms.
        yield return ShowLipSync("bph0034");

        if (progression == 1 || progression == 3)
            // Gradually increase the walking speed as the exercise becomes easier.
            yield return ShowLipSync("bph0035");

        yield return WaitForCurrentAnimation();
    }

    protected IEnumerator Walking3(int progression = 0)
    {
        // Stand with your head and upper body in an upright position.
        yield return ShowLipSync("bph0018");

        // Place your feet hip width apart.
        yield return ShowLipSync("bph0019");

        if (progression == 1 || progression == 3) SpeedUpExercise(1.5f);
        StartInstructions();

        if (progression == 0 || progression == 1)
            // Walk across the room for one minute making sure to move your head up to look at the ceiling and down to look at the ground.
            yield return ShowLipSync("bph0038");
        else
            // Walk across the room for one minute making sure to practice diagonal v shaped head movements. Look up to the left, down centre and up to the right. Can start in either direction.
            yield return ShowLipSync("bph0039");

        // Try to make sure to shift your body weight forward with each step and swing your arms.
        yield return ShowLipSync("bph0034");

        if (progression == 1 || progression == 3)
            // Gradually increase the walking speed as the exercise becomes easier.
            yield return ShowLipSync("bph0035");

        yield return WaitForCurrentAnimation();
    }
    #endregion

    protected IEnumerator RepeatExercise(int delay = 0)
    {
        MoveAvatarInFront();
        StartInstructions();
        yield return WaitForCurrentAnimation();
        // This is required only for standing ex 2 otherwise has no effect
        animator.SetTrigger("StepDown");
        yield return WaitForCurrentAnimation();
        MoveAvatarOnSide();
    }

    protected void ShowInstructions(ScenePayload payload)
    {
        LipSyncData data = null;
        data = GetLipSyncData(payload.value);
        if (data == null)
        {
            StartCoroutine(NotSupportedEvent(payload));
            return;
        }

        ProcessQuestions(ref payload);

        StartCoroutine(ShowAndPost(ShowLipSync(data), payload));
    }

    protected virtual IEnumerator PlayExercise() { yield return null; }
    protected IEnumerator ShowLipSync(string lipsync)
    {
        yield return ShowLipSync(GetLipSyncData(lipsync));
    }

    protected IEnumerator ShowLipSync(LipSyncData data, int delay = 0)
    {
        yield return new WaitForSeconds(delay);
        if (lipSync && avatarVisible)
        {
            lipSync.Play(data);
        } else
        {
            GetComponent<AudioSource>().clip = data.clip;
            GetComponent<AudioSource>().Play();
        }
        yield return new WaitForSeconds(data.length);
    }

    protected IEnumerator ShowLipSyncList(List<LipSyncData> data, int delay = 0)
    {
        yield return new WaitForSeconds(delay);
        foreach(LipSyncData value in data)
        {
            if (lipSync)
            {
                lipSync.Play(value);
            } else
            {
                GetComponent<AudioSource>().clip = value.clip;
                GetComponent<AudioSource>().Play();
            }
            yield return new WaitForSeconds(value.length);
        }
    }

    protected string GetLipSyncPath(string lipsync)
    {
        return language + "/" + avatar + "/" + lipsync;
    }

    protected LipSyncData GetLipSyncData(string lipsync)
    {
        string path = GetLipSyncPath(lipsync);
        return Resources.Load<LipSyncData>(path);
    }
    protected IEnumerator ShowAndPost(IEnumerator animation, ScenePayload payload)
    {
        yield return animation;

        // sitting exercise cards no longer needed 
        PostExerciseSetup();

        payload.status = "Done";

        if (isDemo)
        {
            demoControls.SetActive(true);
            yield return null;
        }

        if (payload.speechRecognition)
        {
            yield return RecognizeSpeech(payload);
        }
        
        yield return ConnectionManager.Instance().PostHttpRequest(payload.ToJSON());
    }

    protected void MoveAvatarOnSide()
    {
        if (isWindowsOS || DontMoveAvatar)
            return;

        avatarCollection.transform.Rotate(new Vector3(0, 1, 0), -25);
        avatarCollection.transform.Translate(new Vector3(1.0f, 0.0f, 0.0f));
        isAvatarOnSide = true;
    }

    protected void MoveAvatarInFront()
    {
        if (isAvatarOnSide)
        {
            avatarCollection.transform.Translate(new Vector3(-1.0f, 0.0f, 0.0f));
            avatarCollection.transform.Rotate(new Vector3(0, 1, 0), 25);
            isAvatarOnSide = false;
        }
    }

    protected void ProcessQuestions(ref ScenePayload payload)
    {
        string questionsYesNo = "bph0073 bph0075 bph0076 bph0077 bph0078 bph0087 bph0088 bph0089 bph0100 bph0101";
        string questionsMildModerateSevere = "bph0074 bph0090 bph0091 bph0092 bph0094";
        string questionsNoLowModerateSevere = "bph00110";

        Dictionary<string, string[]> wordsYesNo = new Dictionary<string, string[]>()
        {
            {"EN",  new string [] {"yes", "no" } },
            {"DE",  new string [] { "ja", "nein" } },
            {"GR",  new string [] { "ναι", "όχι" } },
            {"PT",  new string [] { "sim", "não" } },
            {"IT",  new string [] { "sì", "no" } },
        };

        Dictionary<string, string[]> wordsMildModerateSevere = new Dictionary<string, string[]>()
        {
            {"EN",  new string [] { "mild", "mild", "moderate", "moderate", "moderate", "severe", "severe" } },
            {"DE",  new string [] { "leicht", "leicht", "mittel", "mittel", "mittel", "sehr", "sehr" } },
            {"GR",  new string [] { "είναι ήπια", "είναι ήπια", "μέτρια", "μέτρια", "μέτρια", "σοβαρή", "σοβαρή"} },
            {"PT",  new string [] { "ligeira", "ligeira", "moderada", "moderada", "moderada", "severa", "severa"} },
            {"IT",  new string [] { "lievi", "lieve", "moderate", "moderato", "moderata", "gravi", "grave" } },
        };

        Dictionary<string, string[]> wordsNoLowModerateSevere = new Dictionary<string, string[]>()
        {
            {"EN",  new string [] { "no", "low", "moderate", "severe" } },
            {"DE",  new string [] { "nein", "niedrig", "mäßig", "schwer" } },
            {"GR",  new string [] { "όχι", "χαμηλός", "μέτριος", "αυστηρός" } },
            {"PT",  new string [] { "ausente", "baixo", "moderado", "severo" } },
            {"IT",  new string [] { "assente", "bassa", "moderata", "grave" } },
        };

        if (questionsYesNo.Contains(payload.value))
        {
            payload.allowedRecognitionWords = wordsYesNo[language];
            payload.allowedRecognitionWordsEnglish = wordsYesNo["EN"];
            payload.speechRecognition = true;
        }
        else if(questionsMildModerateSevere.Contains(payload.value))
        {
            payload.allowedRecognitionWords = wordsMildModerateSevere[language];
            payload.allowedRecognitionWordsEnglish = wordsMildModerateSevere["EN"];
            payload.speechRecognition = true;
        }
        else if(questionsNoLowModerateSevere.Contains(payload.value))
        {
            payload.allowedRecognitionWords = wordsNoLowModerateSevere[language];
            payload.allowedRecognitionWordsEnglish = wordsNoLowModerateSevere["EN"];
            payload.speechRecognition = true;
        }
    }
}
