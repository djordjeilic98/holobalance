using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MemoryGameParameters
{
    public int numCards = 4;
    public float viewDuration = 5.0f;
}

public class MemoryGameController : AbstractScenarioController {
    [Header("Parameters")]
    public MemoryGameParameters parameters;

    [Space(10)]
    [Header("Settings")]


    //public GameObject GameQuestion;
    //public GameObject GameAnswer;
    //public GameObject GameReward;
    public GameObject[] cards;
    public GameObject[] cardBackgrounds;

    public Transform cardsCenter;
    public Transform cardsAnswerCenter;

    public GameObject NextButton;
    // public GameObject SemenovacButton;
    // public GameObject OkButton;
    // public GameObject SemenovacButton;
   

    private int currentAnswer = 0;
    private int currentAnswer2 = 0;

#if UNITY_STANDALONE_WIN
    public SpeechKeywordRecognizer speachRecognizer;
#endif

    public float DelayForGameStart = 10f;
    public float DelayForQuestion = 10f;
    //public float ScenarioDuration = 60f;
    public float DissapearDelay = 1f;

    //public Color selectedColor;

    public AudioClip[] sounds;

    List<GameObject> answers = new List<GameObject>();

    bool gameStarted = false;
    //bool waitingForAnswer = false;
    bool gameCompleted = false;

    private int[] cardsIndex;
    private int[] cardsAnswerIndex;

    private AudioSource audioSource;

    float cardSize = 0.76f;
    float cardSpace = 0.2f;

    //results data
    float timeNeededForAnswer;
    int score;

    // Use this for initialization
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public override void StartScenario(int progression)
    {
        base.StartScenario(progression);

        parameters = ProgressionLoader.Instance.gameParameters.memory_game[progression];

        currentAnswer = 0;

        gameStarted = false;

        gameObject.SetActive(true);
        NextButton.SetActive(false);
        // SemenovacButton.SetActive(true);
        // OkButton.SetActive(false);
    

        for (int i = 0; i < cardBackgrounds.Length; i++)
        {
            cardBackgrounds[i].SetActive(false);
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetActive(false);
            cards[i].GetComponent<GazeButton>().enabled = false;
        }

        //Audio - "You need to stand still for 60 seconds and look at the animal and you will receive a badge for that. If you move you will scare the animal."
        //audioSource.clip = sounds[0];
        //audioSource.Play();

        //ShowGameQuestion();

        //Invoke("FinishScenario", ScenarioDuration);
    }

    public override void RunScenario()
    {
        base.RunScenario();
        ShowGameQuestion();
    }

    public override void EndScenario()
    {
        base.EndScenario();

        gameObject.SetActive(false);
    }

    public void ShowGameQuestion()
    {

        gameStarted = true;
        //NextButton.SetActive(false);
        cardsIndex = new int[cards.Length];
        for (int i = 0; i < cardsIndex.Length; i++)
        {
            cardsIndex[i] = i;
        }
        for (int i = 0; i < cardsIndex.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, cardsIndex.Length);
            int tempGO = cardsIndex[rnd];
            cardsIndex[rnd] = cardsIndex[i];
            cardsIndex[i] = tempGO;
        }
        Vector3 alignDirection = cardsCenter.transform.right;
        Vector3 alignDirection2 = cardsCenter.transform.up;
        Vector3 startPosition = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection;
        //new variable for new row
        Vector3 startPosition2 = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection - (cardSize + cardSpace) * alignDirection2;
        // move to center if num of cards 6
        if (parameters.numCards == 6)
        {
            startPosition = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection;

            startPosition2 = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection - (cardSize + cardSpace) * alignDirection2;
        }
        // move to center if num of cards 7
        if (parameters.numCards == 7){
            startPosition = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection;

            startPosition2 = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + (cardSize/2) * alignDirection - (cardSize + cardSpace) * alignDirection2;
        }
        for (int i = 0; i < parameters.numCards; i++)
        {
            /*if
            cards[cardsIndex[i]].SetActive(true);
            cards[cardsIndex[i]].transform.position = startPosition;
            cards[cardsIndex[i]].transform.rotation = cardsCenter.transform.rotation;
            da probamo da se izvrsava za i=0,1,2,3
            startPosition += (cardSize + cardSpace) * alignDirection;
            da probamo da se za i=4,5,6 resetuje startposition i onda alignDirection = cardCenter.transform.down

            prvo se pitamo da li je parameters.numCards>=6
            ako nije izvrsava se ovaj kod iznad 
            ako jeste onda se ubacuju jos 2 ifa unutra
            ako je trenutna karta i=0,1,2 ili 3 onda se izvrsava kod iznad
            inace ako je trenutna karta i=4,5 ili 6 onda se resetuje startposition sa onim gore vecotr3 i izvrsava se kod pomeranja karata
            isto kao gore samo se koristi *alignDirection2*/

            //if number of cards under 6, set up all normaly
            if(parameters.numCards < 6){
                cards[cardsIndex[i]].SetActive(true);
                cards[cardsIndex[i]].transform.position = startPosition;
                cards[cardsIndex[i]].transform.rotation = cardsCenter.transform.rotation;
                startPosition += (cardSize + cardSpace) * alignDirection;
            }
            //if number of cards equal 6, set up cards in two rows (2x3)
            else if (parameters.numCards == 6){
                if( i < 3){
                    cards[cardsIndex[i]].SetActive(true);
                    cards[cardsIndex[i]].transform.position = startPosition;
                    cards[cardsIndex[i]].transform.rotation = cardsCenter.transform.rotation;
                    startPosition += (cardSize + cardSpace) * alignDirection;
                }
                else{
                    //startPosition=startPosition2;
                    cards[cardsIndex[i]].SetActive(true);
                    cards[cardsIndex[i]].transform.position = startPosition2;
                    cards[cardsIndex[i]].transform.rotation = cardsCenter.transform.rotation;
                    startPosition2 += (cardSize + cardSpace) * alignDirection;
                }
            }
            //if number of cards equal 7, set up cards in two rows (1x4+1x3)
             else if (parameters.numCards == 7){
                if( i < 4){
                    cards[cardsIndex[i]].SetActive(true);
                    cards[cardsIndex[i]].transform.position = startPosition;
                    cards[cardsIndex[i]].transform.rotation = cardsCenter.transform.rotation;
                    startPosition += (cardSize + cardSpace) * alignDirection;
                }
                else{
                    //startPosition=startPosition2;
                    cards[cardsIndex[i]].SetActive(true);
                    cards[cardsIndex[i]].transform.position = startPosition2;
                    cards[cardsIndex[i]].transform.rotation = cardsCenter.transform.rotation;
                    startPosition2 += (cardSize + cardSpace) * alignDirection;
                }
            }


        }


        //after 10 seceonds invoke game start
        Invoke("ShowGameAnswerInfo", parameters.viewDuration);

        //GameQuestion.SetActive(true);
        //Invoke("ShowGameAnswer", DelayForQuestion);
    }

    void ShowGameAnswerInfo()
    {
        for (int i = 0; i < parameters.numCards; i++)
        {
            cards[cardsIndex[i]].SetActive(false);
        }
        Invoke("ShowGameAnswer", 1.0f);
    }

    //answer cards, empty cards for store answer cards and answer cards
    void ShowGameAnswer()
    {
        if (!gameStarted)
        {
            return;
        }

        LocalizationManager.PlayLocalizedAudio(gameObject, "game_rememberorder_select");

        //NextButton.SetActive(true);

        cardsAnswerIndex = new int[parameters.numCards];
        for (int i = 0; i < parameters.numCards; i++)
        {
            cardsAnswerIndex[i] = cardsIndex[i];
        }
        for (int i = 0; i < cardsAnswerIndex.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, cardsAnswerIndex.Length);
            int tempGO = cardsAnswerIndex[rnd];
            cardsAnswerIndex[rnd] = cardsAnswerIndex[i];
            cardsAnswerIndex[i] = tempGO;
        }

        Vector3 alignDirection = cardsCenter.transform.right;
        Vector3 alignDirection2 = cardsCenter.transform.up;
        Vector3 startPositionAnswer = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
        Vector3 startPositionAnswer2 = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 - (cardSize + cardSpace) * alignDirection2;
        // move to center if num of cards 6 (cards space for answer)
        if (parameters.numCards == 6)
        {
            startPositionAnswer = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            startPositionAnswer2 = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 - (cardSize + cardSpace) * alignDirection2;
        }
        if (parameters.numCards == 7)
        {
            startPositionAnswer = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            startPositionAnswer2 = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 + (cardSize/2) * alignDirection - (cardSize + cardSpace) * alignDirection2;
        }

        Vector3 startPosition = cardsAnswerCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection;
        Vector3 startPosition2 = cardsAnswerCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection - (cardSize + cardSpace) * alignDirection2;
        // move to center if num of cards 6 (answer cards)
        if (parameters.numCards == 6)
        {
            startPosition = cardsAnswerCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection;
            startPosition2 = cardsAnswerCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection - (cardSize + cardSpace) * alignDirection2;
        }
        if (parameters.numCards == 7)
        {
            startPosition = cardsAnswerCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection;
            startPosition2 = cardsAnswerCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection - (cardSize + cardSpace) * alignDirection2;
        }
       
        for (int i = 0; i < parameters.numCards; i++)
        {
            // cards[cardsAnswerIndex[i]].GetComponent<GazeButton>().enabled = true;
            // cards[cardsAnswerIndex[i]].SetActive(true);
            // cards[cardsAnswerIndex[i]].transform.position = startPosition;
            // cards[cardsAnswerIndex[i]].transform.rotation = cardsAnswerCenter.transform.rotation;

            // cardBackgrounds[i].SetActive(true);
            // cardBackgrounds[i].transform.position = startPosition - 0.01f * cardsCenter.transform.up;
            // cardBackgrounds[i].transform.rotation = cardsAnswerCenter.transform.rotation;

            // startPosition += (cardSize + cardSpace) * alignDirection;

            // cardBackgrounds[10 + i].SetActive(true);
            // cardBackgrounds[10 + i].transform.position = startPositionAnswer + new Vector3(0,0, 0.1f);
            // cardBackgrounds[10 + i].transform.rotation = cardsCenter.transform.rotation;
            // startPositionAnswer += (cardSize + cardSpace) * alignDirection;

            //if number of cards under 6, set up all normaly
             if(parameters.numCards < 6){
                cards[cardsAnswerIndex[i]].GetComponent<GazeButton>().enabled = true;
                cards[cardsAnswerIndex[i]].SetActive(true);
                cards[cardsAnswerIndex[i]].transform.position = startPosition;
                cards[cardsAnswerIndex[i]].transform.rotation = cardsAnswerCenter.transform.rotation;

                cardBackgrounds[i].SetActive(true);
                cardBackgrounds[i].transform.position = startPosition - 0.01f * cardsCenter.transform.up;
                cardBackgrounds[i].transform.rotation = cardsAnswerCenter.transform.rotation;

                startPosition += (cardSize + cardSpace) * alignDirection;

                cardBackgrounds[10 + i].SetActive(true);
                cardBackgrounds[10 + i].transform.position = startPositionAnswer + new Vector3(0,0, 0.1f);
                cardBackgrounds[10 + i].transform.rotation = cardsCenter.transform.rotation;
                startPositionAnswer += (cardSize + cardSpace) * alignDirection;
            }
            //if number of cards equal 6, set up cards in two rows (2x3)
            else if (parameters.numCards == 6){
                if( i < 3){
                    cards[cardsAnswerIndex[i]].GetComponent<GazeButton>().enabled = true;
                    cards[cardsAnswerIndex[i]].SetActive(true);
                    cards[cardsAnswerIndex[i]].transform.position = startPosition;
                    cards[cardsAnswerIndex[i]].transform.rotation = cardsAnswerCenter.transform.rotation;

                    cardBackgrounds[i].SetActive(true);
                    cardBackgrounds[i].transform.position = startPosition - 0.01f * cardsCenter.transform.up;
                    cardBackgrounds[i].transform.rotation = cardsAnswerCenter.transform.rotation;

                    startPosition += (cardSize + cardSpace) * alignDirection;

                    cardBackgrounds[10 + i].SetActive(true);
                    cardBackgrounds[10 + i].transform.position = startPositionAnswer + new Vector3(0,0, 0.1f);
                    cardBackgrounds[10 + i].transform.rotation = cardsCenter.transform.rotation;
                    startPositionAnswer += (cardSize + cardSpace) * alignDirection;
                }
                else{
                    //startPosition=startPosition2;
                    cards[cardsAnswerIndex[i]].GetComponent<GazeButton>().enabled = true;
                    cards[cardsAnswerIndex[i]].SetActive(true);
                    cards[cardsAnswerIndex[i]].transform.position = startPosition2;
                    cards[cardsAnswerIndex[i]].transform.rotation = cardsAnswerCenter.transform.rotation;

                    cardBackgrounds[i].SetActive(true);
                    cardBackgrounds[i].transform.position = startPosition2 - 0.01f * cardsCenter.transform.up;
                    cardBackgrounds[i].transform.rotation = cardsAnswerCenter.transform.rotation;

                    startPosition2 += (cardSize + cardSpace) * alignDirection;

                    cardBackgrounds[10 + i].SetActive(true);
                    cardBackgrounds[10 + i].transform.position = startPositionAnswer2 + new Vector3(0,0, 0.1f);
                    cardBackgrounds[10 + i].transform.rotation = cardsCenter.transform.rotation;
                    startPositionAnswer2 += (cardSize + cardSpace) * alignDirection;
                }
            //if number of cards equal 7, set up cards in two rows (1x4+1x3)
            }else if (parameters.numCards == 7){
                if( i < 4){
                    cards[cardsAnswerIndex[i]].GetComponent<GazeButton>().enabled = true;
                    cards[cardsAnswerIndex[i]].SetActive(true);
                    cards[cardsAnswerIndex[i]].transform.position = startPosition;
                    cards[cardsAnswerIndex[i]].transform.rotation = cardsAnswerCenter.transform.rotation;

                    cardBackgrounds[i].SetActive(true);
                    cardBackgrounds[i].transform.position = startPosition - 0.01f * cardsCenter.transform.up;
                    cardBackgrounds[i].transform.rotation = cardsAnswerCenter.transform.rotation;

                    startPosition += (cardSize + cardSpace) * alignDirection;

                    cardBackgrounds[10 + i].SetActive(true);
                    cardBackgrounds[10 + i].transform.position = startPositionAnswer + new Vector3(0,0, 0.1f);
                    cardBackgrounds[10 + i].transform.rotation = cardsCenter.transform.rotation;
                    startPositionAnswer += (cardSize + cardSpace) * alignDirection;
                }
                else{
                    //startPosition=startPosition2;
                    cards[cardsAnswerIndex[i]].GetComponent<GazeButton>().enabled = true;
                    cards[cardsAnswerIndex[i]].SetActive(true);
                    cards[cardsAnswerIndex[i]].transform.position = startPosition2;
                    cards[cardsAnswerIndex[i]].transform.rotation = cardsAnswerCenter.transform.rotation;

                    cardBackgrounds[i].SetActive(true);
                    cardBackgrounds[i].transform.position = startPosition2 - 0.01f * cardsCenter.transform.up;
                    cardBackgrounds[i].transform.rotation = cardsAnswerCenter.transform.rotation;

                    startPosition2 += (cardSize + cardSpace) * alignDirection;

                    cardBackgrounds[10 + i].SetActive(true);
                    cardBackgrounds[10 + i].transform.position = startPositionAnswer2 + new Vector3(0,0, 0.1f);
                    cardBackgrounds[10 + i].transform.rotation = cardsCenter.transform.rotation;
                    startPositionAnswer2 += (cardSize + cardSpace) * alignDirection;
                }
            }


        }
        answers = new List<GameObject>();

        timeNeededForAnswer = Time.time;

        //Audio: Repeat the previous pattern by speaking the numbers above the cards.
        //audioSource.clip = sounds[2];
        //audioSource.Play();

        //waitingForAnswer = true;
        //GameQuestion.SetActive(false);
        //GameAnswer.SetActive(true);
    }

    //public void SelectAnswer(int answer)
    //{
    //    Debug.Log("select: " + answer);
    //    if (gameStarted && waitingForAnswer)
    //    {
    //        Debug.Log("select: " + answers);
    //        bool selected = answers.Exists(a => a == answer);
    //        if (!selected)
    //        {
    //            cards[answer - 1].GetComponent<SpriteRenderer>().color = selectedColor;
    //            answers.Add(answer);
    //        }

    //        if (answers.Count == solution.Length)
    //        {
    //            Invoke("FinishGame", DissapearDelay);
    //        }
    //    }
    //}

    public void SelectAnswer(GameObject answer)
    {
        if (answers.Contains(answer))
        {
            answers.Remove(answer);
            currentAnswer--;

            int index = 0;
            for (int i = 0; i < parameters.numCards; i++)
            {
                if (cards[cardsAnswerIndex[i]] == answer)
                {
                    index = i;
                    break;
                }
            }
            // old code
            Vector3 alignDirection = cardsCenter.transform.right;
            Vector3 alignDirection2 = cardsCenter.transform.up;
            //Vector3 startPositionAnswer = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            Vector3 startPosition = cardsAnswerCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            if (parameters.numCards == 6)
            {
                startPosition = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            }

            startPosition += (cardSize + cardSpace) * index * alignDirection;

            answer.transform.position = startPosition + new Vector3(0,0, 0.1f);
            answer.transform.rotation = cardsAnswerCenter.transform.rotation;

            if (answers.Count > 0)
            {
                answers[answers.Count - 1].GetComponent<GazeButton>().enabled = true;
            }

            NextButton.SetActive(false);

            //new code
            // if (index < 4)
            // {
            //     Vector3 alignDirection = cardsCenter.transform.right;
            //     Vector3 alignDirection2 = cardsCenter.transform.up;
            //     //Vector3 startPositionAnswer = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            //     Vector3 startPosition = cardsAnswerCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            //     Vector3 startPosition2 = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 - (cardSize + cardSpace) * alignDirection2;

            //     startPosition += (cardSize + cardSpace) * index * alignDirection;

            //     answer.transform.position = startPosition + new Vector3(0,0, 0.1f);
            //     answer.transform.rotation = cardsAnswerCenter.transform.rotation;

            //     if (answers.Count > 0)
            //     {
            //         answers[answers.Count - 1].GetComponent<GazeButton>().enabled = true;
            //     }

            //     NextButton.SetActive(false);
            // }else
            // {
            //     Vector3 alignDirection = cardsCenter.transform.right;
            //     Vector3 alignDirection2 = cardsCenter.transform.up;
            //     //Vector3 startPositionAnswer = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            //     Vector3 startPosition = cardsAnswerCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            //     Vector3 startPosition2 = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 - (cardSize + cardSpace) * alignDirection2;

            //     startPosition2 += (cardSize + cardSpace) * index * alignDirection;

            //     answer.transform.position = startPosition2 + new Vector3(0,0, 0.1f);
            //     answer.transform.rotation = cardsAnswerCenter.transform.rotation;

            //     if (answers.Count > 0)
            //     {
            //         answers[answers.Count - 1].GetComponent<GazeButton>().enabled = true;
            //     }

            //     NextButton.SetActive(false);
            // }
            
        }
        else
        {
            if (answers.Count > 0)
            {
                answers[answers.Count - 1].GetComponent<GazeButton>().enabled = false;
            }
            
            // Vector3 alignDirection = cardsCenter.transform.right;
            // Vector3 alignDirection2 = cardsCenter.transform.up;
            // Vector3 startPosition = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;

            // startPosition += (cardSize + cardSpace) * currentAnswer * alignDirection;

            // answer.transform.position = startPosition;
            // answer.transform.rotation = cardsCenter.transform.rotation;
            // answers.Add(answer);
            // currentAnswer++;
            Vector3 alignDirection = cardsCenter.transform.right;
            Vector3 alignDirection2 = cardsCenter.transform.up;
            Vector3 startPosition = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
            Vector3 startPosition2 = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 - (cardSize + cardSpace) * alignDirection2;
            if (parameters.numCards == 6)
            {
                startPosition = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
                startPosition2 = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 - (cardSize + cardSpace) * alignDirection2;
            }
            if (parameters.numCards == 7)
            {
                startPosition = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
                startPosition2 = cardsCenter.transform.position - ((cardSize * 3) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 + (cardSize/2)*alignDirection - (cardSize + cardSpace) * alignDirection2;
            }
            
            if (parameters.numCards < 6){
                //Vector3 alignDirection = cardsCenter.transform.right;
                //Vector3 alignDirection2 = cardsCenter.transform.up;
                //Vector3 startPosition = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;

                startPosition += (cardSize + cardSpace) * currentAnswer * alignDirection;

                answer.transform.position = startPosition;
                answer.transform.rotation = cardsCenter.transform.rotation;
                answers.Add(answer);
                currentAnswer++;
            }else if (parameters.numCards == 6){
                if (currentAnswer < 3){
                    //Vector3 alignDirection = cardsCenter.transform.right;
                    //Vector3 alignDirection2 = cardsCenter.transform.up;
                    //Vector3 startPosition = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;

                    startPosition += (cardSize + cardSpace) * currentAnswer * alignDirection;

                    answer.transform.position = startPosition;
                    answer.transform.rotation = cardsCenter.transform.rotation;
                    answers.Add(answer);
                    currentAnswer++;
                } else {
                    //Vector3 alignDirection = cardsCenter.transform.right;
                    //Vector3 alignDirection2 = cardsCenter.transform.up;
                    //Vector3 startPosition = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
                    //Vector3 startPosition2 = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 - (cardSize + cardSpace) * alignDirection2;
        
                    //startPosition2 += (cardSize + cardSpace) * currentAnswer * alignDirection;
                    startPosition2 += (cardSize + cardSpace)  * currentAnswer2 * alignDirection;

                    answer.transform.position = startPosition2;
                    answer.transform.rotation = cardsCenter.transform.rotation;
                    answers.Add(answer);
                    currentAnswer++;
                    currentAnswer2++;
                }
            }else if (parameters.numCards == 7){
                if (currentAnswer < 4){
                    //Vector3 alignDirection = cardsCenter.transform.right;
                    //Vector3 alignDirection2 = cardsCenter.transform.up;
                    //Vector3 startPosition = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;

                    startPosition += (cardSize + cardSpace) * currentAnswer * alignDirection;

                    answer.transform.position = startPosition;
                    answer.transform.rotation = cardsCenter.transform.rotation;
                    answers.Add(answer);
                    currentAnswer++;
                } else {
                    //Vector3 alignDirection = cardsCenter.transform.right;
                    //Vector3 alignDirection2 = cardsCenter.transform.up;
                    //Vector3 startPosition = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2;
                    //Vector3 startPosition2 = cardsCenter.transform.position - ((cardSize * parameters.numCards) * alignDirection + (cardSpace * (parameters.numCards - 1)) * alignDirection) / 2 + (cardSize / 2) * alignDirection + cardSize*alignDirection2 - (cardSize + cardSpace) * alignDirection2;
        
                    //startPosition2 += (cardSize + cardSpace) * currentAnswer * alignDirection;
                    startPosition2 += (cardSize + cardSpace)  * currentAnswer2 * alignDirection;

                    answer.transform.position = startPosition2;
                    answer.transform.rotation = cardsCenter.transform.rotation;
                    answers.Add(answer);
                    currentAnswer++;
                    currentAnswer2++;
                }
            }
            
            
            
            

            if (parameters.numCards == answers.Count)
            {
                NextButton.SetActive(true);
                // SemenovacButton.SetActive(false);
                // OkButton.SetActive(true);
            }
        }
    }

    public override void FinishScenario(bool win)
    {
        base.FinishScenario(win);
    }

    private int goodAnswers;

    public void FinishGame()
    {
        if (!gameStarted)
        {
            return;
        }

        timeNeededForAnswer = Time.time - timeNeededForAnswer;

        goodAnswers = 0;
        for (int i = 0; i < parameters.numCards; i++)
        {
            if (cards[cardsIndex[i]] == answers[i])
            {
                goodAnswers++;
            }
        }

        for (int i = 0; i < cardBackgrounds.Length; i++)
        {
            cardBackgrounds[i].SetActive(false);
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetActive(false);
        }

        if (goodAnswers == parameters.numCards)
        {
            score = 1;
        } else
        {
            score = 0;
        }
        currentAnswer = 0;
        currentAnswer2 = 0;
        //GameAnswer.SetActive(false);

        FinishScenario(goodAnswers == parameters.numCards);
    }

    public override string GetEndText(bool win)
    {
        return base.GetEndText(win) + "\n" + goodAnswers + "/" + parameters.numCards;
    }

    void FinishScenario()
    {
        if (!gameStarted)
        {
            return;
        }

        //GameQuestion.SetActive(false);
        //GameAnswer.SetActive(false);

        //GameReward.SetActive(true);

        //audio sound for reward
        audioSource.clip = sounds[5];
        audioSource.Play();
        Invoke("HideReward", 4f);

        gameStarted = false;
        //waitingForAnswer = false;
        gameCompleted = false;
    }

    void HideReward() {
        //GameReward.SetActive(false);
#if UNITY_STANDALONE_WIN
        speachRecognizer.GoToMainMenu();
#endif
    }

    public void CancelScenario()
    {
        gameStarted = false;
        //waitingForAnswer = false;
        gameCompleted = false;
        //GameQuestion.SetActive(false);
        //GameAnswer.SetActive(false);

        CancelInvoke();

        gameObject.SetActive(false);
    }

    public override string GetResultsJSON()
    {
        string question = "";
        string answer = "";
        for (int i = 0; i < parameters.numCards; i++)
        {
            if (i != 0)
            {
                question += ",";
                answer += ",";
            }
            question += cards[cardsIndex[i]].name;
            answer += answers[i].name;
        }

        string currentTimestamp = ResultsSender.GetTimestamp();

        ResultsBuilder results = new ResultsBuilder(gameId, currentTimestamp, duration);
        results.AddSummary("total_cards", "Total cards", parameters.numCards);
        results.AddSummary("time_needed", "Time needed for answer", timeNeededForAnswer);
        results.AddSummary("question", "Question", question);
        results.AddSummary("answer", "Answer", answer);
        results.AddSummary("level", "Progression level", currentProgression + 1);
        results.AddSummary("total_score", "Total Score", score * 100);
        results.AddSummary("exercise_duration", "Duration", duration);

        return results.getJSON();
    }
}
