using RogoDigital.Lipsync;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioFeedback : MonoBehaviour {

    public string winKey;
    public string loseKey;
    public string infoKey;

    public Sprite infoImage;

    private AudioClip winSound;
    private AudioClip loseSound;
    private AudioClip infoSound;

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();

        LoadSounds();
    }

    void LoadSounds()
    {
        if (winSound == null)
        {
            winSound = Resources.Load<LipSyncData>(SceneControllerCTG.GetLocalizationPath() + SceneControllerCTG.WIN).clip;//LocalizationManager.instance.GetLocalizedAudio(winKey);
            loseSound = Resources.Load<LipSyncData>(SceneControllerCTG.GetLocalizationPath() + SceneControllerCTG.LOSE).clip; //LocalizationManager.instance.GetLocalizedAudio(loseKey);
            infoSound = LocalizationManager.instance.GetLocalizedAudio(infoKey);
        }
    }

    public void Win()
    {
        LoadSounds();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = winSound;
        audioSource.Play();
    }

    public void Lose()
    {
        LoadSounds();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = loseSound;
        audioSource.Play();
    }

    public void Info()
    {
        LoadSounds();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = infoSound;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }


}
