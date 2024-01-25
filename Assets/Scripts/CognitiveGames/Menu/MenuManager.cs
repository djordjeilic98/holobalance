using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject MainPanel;
    public GameObject ExcercisesPanel;
    public GameObject GamesPanel;

    public GameObject excerciseScene;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoToMenu() {
        HideMenu();

        MainPanel.SetActive(true);
       
        //excerciseScene.SetActive(false);
    }

    public void GoToExcercisesMenu()
    {
        HideMenu();

        ExcercisesPanel.SetActive(true);
    }

    public void GoToGamesMenu()
    {
        HideMenu();

        GamesPanel.SetActive(true);
    }

    public void HideMenu()
    {
        MainPanel.SetActive(false);
        ExcercisesPanel.SetActive(false);
        GamesPanel.SetActive(false);
    }

    /*public void GoToExcercise(int excerciseIndex)
    {
        excerciseScene.SetActive(true);

        excerciseScene.GetComponent<ObjectTrackingGame>().InitGame(excerciseIndex);
    }*/
}
