using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GamesParameters
{
    public CathingGameParameters[] catching_game;
    public RememberPreviousParameters[] remember_previous;
    public AnimalFeedingParameters[] animal_feeding;
    public PreparingFoodParameters[] preparing_food;
    public MemoryGameParameters[] memory_game;
    public MarbleGameParameters[] marble_game;
}

public class ProgressionLoader : MonoBehaviour {

    public static ProgressionLoader Instance;
    public GamesParameters gameParameters;

    private void Awake()
    {
        Instance = this;
        LoadParameters();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadParameters()
    {
        TextAsset json = Resources.Load<TextAsset>("progression");
        gameParameters = JsonUtility.FromJson<GamesParameters>(json.text);
        //Debug.Log("Params: " + gameParameters.catching_game.Length);
    }
}
