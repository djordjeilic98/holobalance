using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoSceneControler : MonoBehaviour
{
    public Button btnExSitting = null;
    public Button btnExStanding = null;
    public Button btnExWalking = null;

    public Button btnExP0 = null;
    public Button btnExP1 = null;
    public Button btnExP2 = null;
    public Button btnExP3 = null;

    public Button btnEx1 = null;
    public Button btnEx2 = null;
    public Button btnEx3 = null;
    public Button btnEx4 = null;
    public Button btnSettings = null;
    public Button btnStart = null;

    public Button btnExit = null;

    private string exType, exProgression, exNumber;

    // Start is called before the first frame update
    void Start()
    {
        btnExSitting.onClick.AddListener(OnBtnSitting);
        btnExStanding.onClick.AddListener(OnBtnStanding);
        btnExWalking.onClick.AddListener(OnBtnWalking);

        btnExP0.onClick.AddListener(OnBtnExP0);
        btnExP1.onClick.AddListener(OnBtnExP1);
        btnExP2.onClick.AddListener(OnBtnExP2);
        btnExP3.onClick.AddListener(OnBtnExP3);

        btnEx1.onClick.AddListener(OnBtnEx1);
        btnEx2.onClick.AddListener(OnBtnEx2);
        btnEx3.onClick.AddListener(OnBtnEx3);
        btnEx4.onClick.AddListener(OnBtnEx4);
        btnSettings.onClick.AddListener(OnBtnSettings);
        btnStart.onClick.AddListener(OnStart);

        btnExit.onClick.AddListener(OnExit);
    }

    public void OnBtnSitting()
    {
        exType = "sitting";
        btnExSitting.interactable = false;
        btnExStanding.interactable = true;
        btnExWalking.interactable = true;
    }
    public void OnBtnStanding()
    {
        exType = "standing";
        btnExSitting.interactable = true;
        btnExStanding.interactable = false;
        btnExWalking.interactable = true;
    }
    public void OnBtnWalking()
    {
        exType = "walking";
        btnExSitting.interactable = true;
        btnExStanding.interactable = true;
        btnExWalking.interactable = false;
    }

    public void OnBtnExP0()
    {
        exProgression = "P0";
        btnExP0.interactable = false;
        btnExP1.interactable = true;
        btnExP2.interactable = true;
        btnExP3.interactable = true;
    }
    public void OnBtnExP1()
    {
        exProgression = "P1";
        btnExP0.interactable = true;
        btnExP1.interactable = false;
        btnExP2.interactable = true;
        btnExP3.interactable = true;
    }
    public void OnBtnExP2()
    {
        exProgression = "P2";
        btnExP0.interactable = true;
        btnExP1.interactable = true;
        btnExP2.interactable = false;
        btnExP3.interactable = true;
    }
    public void OnBtnExP3()
    {
        exProgression = "P3";
        btnExP0.interactable = true;
        btnExP1.interactable = true;
        btnExP2.interactable = true;
        btnExP3.interactable = false;
    }

    public void OnBtnEx1()
    {
        exNumber = "1";
        btnEx1.interactable = false;
        btnEx2.interactable = true;
        btnEx3.interactable = true;
        btnEx4.interactable = true;
    }
    public void OnBtnEx2()
    {
        exNumber = "2";
        btnEx1.interactable = true;
        btnEx2.interactable = false;
        btnEx3.interactable = true;
        btnEx4.interactable = true;
    }
    public void OnBtnEx3()
    {
        exNumber = "3";
        btnEx1.interactable = true;
        btnEx2.interactable = true;
        btnEx3.interactable = false;
        btnEx4.interactable = true;
    }
    public void OnBtnEx4()
    {
        exNumber = "4";
        btnEx1.interactable = true;
        btnEx2.interactable = true;
        btnEx3.interactable = true;
        btnEx4.interactable = false;
    }

    public void OnBtnSettings()
    {
        SceneController.isDemo = false;
        SceneManager.LoadScene("StartScene");
    }

    public void OnStart()
    {
        string exercise = "VC holobalance_" + exType + "_" + exNumber + " " + exProgression;
        ScenePayload payload = new ScenePayload();
        payload.action = "ShowExercise";
        payload.value = exercise;
        SceneController.Instance.OnRequest(payload.ToJSON());
        gameObject.SetActive(false);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
