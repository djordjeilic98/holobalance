using HoloKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider eyeDistanceSlider;//50-75mm
    public Text eyeDistanceText;
    public GameObject panel;

    private bool opened;
    // Start is called before the first frame update
    void Start()
    {
        float eyeDistance = PlayerPrefs.GetFloat("EyeDistance", 0.064f);
        eyeDistanceSlider.value = eyeDistance;
        ChangeParams();
    }

    public void ChangeParams()
    {
        float eyeDistance = eyeDistanceSlider.value / 100.0f + 0.064f;
        eyeDistanceText.text = "EYE DISTANCE: " + eyeDistanceSlider.value;
        PlayerPrefs.SetFloat("EyeDistance", eyeDistanceSlider.value);
        HoloKitCamera.Instance.profile.model.eyeDistance = eyeDistance;
        Debug.Log("EyeDistance: " + eyeDistance);
        HoloKitCamera.Instance.UpdateProfile();
    }

    public void Toggle()
    {
        opened = !opened;
        panel.SetActive(opened);
    }
}
