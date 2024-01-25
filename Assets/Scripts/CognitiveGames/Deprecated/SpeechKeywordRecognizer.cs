#if UNITY_STANDALONE_WIN
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.Events;

/*
More information: https://docs.microsoft.com/en-us/windows/uwp/design/input/specify-the-speech-recognizer-language
*/

public class SpeechKeywordRecognizer : MonoBehaviour
{
    //public GameController gameController;

    public GameObject MainMenu;
    public GameObject[] Scenes;
    public Sprite[] Badges;
    public SpriteRenderer badgeImage;

    private KeywordRecognizer m_Recognizer;
	private String language;

    private List<KeywordRecognizer> recognizers = new List<KeywordRecognizer>();

    [System.Serializable]
    public class SpeachControl
    {
        public string[] keywords;
        public UnityEvent Do;
    }

    public SpeachControl[] speachControl;

    enum State
    {
        MAIN_MENU,
        GAME
    }

    private State state;
	
    void Start()
    {
        foreach(SpeachControl s in speachControl)
        {
            KeywordRecognizer recognizer = new KeywordRecognizer(s.keywords);
            recognizer.OnPhraseRecognized += OnPhraseRecognized;
            recognizer.Start();
            recognizers.Add(recognizer);
        }

		/*m_Recognizer = new KeywordRecognizer(m_MenuKeywords);
        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        m_Recognizer.Start();*/

        state = State.MAIN_MENU;
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        //builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        //builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());

        foreach (SpeachControl s in speachControl)
        {
            foreach(string keyword in s.keywords)
            {
                if (args.text.Equals(keyword))
                {
                    s.Do.Invoke();
                }
            }
        }


        /*if (state == State.MAIN_MENU)
        {
            for (int i = 0; i < m_MenuKeywords.Length; i++)
            {
                if (args.text.Equals(m_MenuKeywords[i]))
                {
                    GoToGame(i);
                    break;
                }
            }
        } 

        if (state == State.GAME)
        {
            for (int i = 0; i < m_BackKeywords.Length; i++)
            {
                if (args.text.Equals(m_BackKeywords[i]))
                {
                    GoToMainMenu();
                    break;
                }
            }
        }*/
    }

    public void GoToGame(int scenario)
    {
        if (state == State.MAIN_MENU)
        {
            MainMenu.SetActive(false);
            Scenes[scenario].SetActive(true);
            state = State.GAME;

            badgeImage.sprite = Badges[scenario];

            //gameController.StartScenario();
        }
    }

    public void GoToMainMenu()
    {
        if (state == State.GAME)
        {
            MainMenu.SetActive(true);
            foreach (GameObject scene in Scenes)
            {
                scene.SetActive(false);
            }
            state = State.MAIN_MENU;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }
        else if (state == State.MAIN_MENU) { 
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GoToGame(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GoToGame(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GoToGame(3);
            }
        }
    }

}
#endif