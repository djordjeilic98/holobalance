using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    public enum Language
    {
        EN,
        GR,
        DE,
        PT,
        IT
    }

    static Dictionary<string, Language> languageCodes = new Dictionary<string, Language>()
    {
        {"EN",  Language.EN},
        {"GR",  Language.GR},
        {"DE",  Language.DE},
        {"PT",  Language.PT},
        {"IT",  Language.IT},
    };

    public Language language;
    string avatar;

    public TextAsset csv;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        string languageStr = PlayerPrefs.GetString("Language");
        avatar = PlayerPrefs.GetString("Avatar");
        language = languageCodes[languageStr];

        LoadLocalizedText();

        //VerboseLocalization();

        //Debug.Log("AC: " + Resources.Load<AudioClip>("en/win"));
    }

    public void LoadLocalizedText()
    {
        localizedText = GetLocalizationDictionary(language, csv.text);


        //string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        //if (File.Exists(filePath))
        //{
        //    string dataAsJson = File.ReadAllText(filePath);
        //    LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

        //    for (int i = 0; i < loadedData.items.Length; i++)
        //    {
        //        localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
        //    }

        //    Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        //}
        //else
        //{
        //    Debug.LogError("Cannot find file!");
        //}

        isReady = true;
    }

    public static Dictionary<string, string> GetLocalizationDictionary(Language language, string localizationText)
    {
        string[,] loadedData = CSVReader.SplitCsvGrid(localizationText);
        Dictionary<string, string> localizedText = new Dictionary<string, string>();
        for (int i = 1; i < loadedData.GetLength(1); i++)
        {
            if (loadedData[0, i] != null)
            {
                //Debug.Log(loadedData[0, i] + " ### " + loadedData[(int)language + 1, i]);
                localizedText.Add(loadedData[0, i], loadedData[(int)language + 1, i]);
            }
        }
        return localizedText;
    }

    public void VerboseLocalization()
    {
        foreach (string key in localizedText.Keys)
        {
            Debug.Log("" + key + ", " + localizedText[key]);
        }
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;

    }

    public bool GetIsReady()
    {
        return isReady;
    }

    public AudioClip GetLocalizedAudio(string key)
    {
        //AssetLoader.LoadAsset<T>(key, CheckLanguageOverrideCode(localizedObject));
        AudioClip audioClip = Resources.Load<AudioClip>(language.ToString() + "/" + avatar + "/" + key);
        return audioClip;
    }

    public static void PlayLocalizedAudio(GameObject gameObject, string key)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = LocalizationManager.instance.GetLocalizedAudio(key); 
        audioSource.Play();
    }

}