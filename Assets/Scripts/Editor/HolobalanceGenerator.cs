using RogoDigital.Lipsync;
using RogoDigital.Lipsync.AutoSync;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class HolobalanceGenerator : EditorWindow
{
    public float secs = 10f;
    public float startVal = 0f;
    public float progress = 0f;

    static bool running;

    [MenuItem("Window/Holobalance/Regenerate data")]
    static void Init()
    {
        UnityEditor.EditorWindow window = GetWindow(typeof(HolobalanceGenerator));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Regenerate lipsync data"))
        {
            GenerateLipSync();
            startVal = (float)EditorApplication.timeSinceStartup;
        }
        GUILayout.Label(running ? "Regenerating: " + currentDataToGenerate + "/" + lisyncToGenerate.Count: "Not running");
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    public struct LipSyncMetaData
    {
        public string lipsyncFile;
        public string audioFile;
        public string transcript;
    }
    static List<LipSyncMetaData> lisyncToGenerate;
    static int currentDataToGenerate = 0;

    public static void GenerateLipSync()
    {
        lisyncToGenerate = new List<LipSyncMetaData>();
        string[] languageCodes = { "EN", "DE", "GR", "PT" };
        string[] genders = { "Male", "Female" };

        string outputPath = Application.dataPath + @"\AssetsCognitiveGames\Sounds\Resources\";

        using (var reader = new StreamReader(Application.dataPath + @"\Resources\Localization.csv"))
        {
            string localizationText = reader.ReadToEnd();

            foreach (var language in languageCodes)
            {
                LocalizationManager.Language languageCode = LocalizationManager.Language.EN;
                if (language == "EN")
                {
                    languageCode = LocalizationManager.Language.EN;
                }
                else if (language == "DE")
                {
                    languageCode = LocalizationManager.Language.DE;
                }
                else if (language == "GR")
                {
                    languageCode = LocalizationManager.Language.GR;
                }
                else if (language == "PT")
                {
                    languageCode = LocalizationManager.Language.PT;
                }
                foreach (var gender in genders)
                {
                    string path = outputPath + language + "\\" + gender + "\\";

                    Dictionary<string, string> localizationDictionary = LocalizationManager.GetLocalizationDictionary(languageCode, localizationText);

                    foreach (string key in localizationDictionary.Keys)
                    {
                        if (File.Exists(path + key + ".mp3"))
                        {
                            string filePath = @"Assets\AssetsCognitiveGames\Sounds\Resources\" + language + "\\" + gender + "\\";

                            LipSyncMetaData metaData = new LipSyncMetaData();
                            metaData.audioFile = language + "/" + gender + "/" + key;
                            metaData.transcript = localizationDictionary[key];
                            metaData.lipsyncFile = filePath + key + ".asset";

                            //Debug.Log("Processing: " + path + key + ".mp3");
                            lisyncToGenerate.Add(metaData);
                        }
                    }
                }
            }
        }

        if (lisyncToGenerate.Count > 0)
        {
            running = true;
            currentDataToGenerate = 0;
            GenerateAutoSync(lisyncToGenerate[currentDataToGenerate]);
        }
    }

    static LipSyncClipSetup window;

    public static void GenerateAutoSync(LipSyncMetaData metaData)
    {
        window = LipSyncClipSetup.ShowWindow();
        //window.LoadXML(textAsset, clip, true);
        AudioClip clip = Resources.Load(metaData.audioFile, typeof(AudioClip)) as AudioClip;
        if (clip)
        {
            window.Transcript = metaData.transcript;

            LipSyncData data = LipSyncClipSetup.SaveFile(window.settings, metaData.lipsyncFile, false, window.Transcript, window.FileLength, window.PhonemeData.ToArray(), window.EmotionData.ToArray(), window.GestureData.ToArray(), clip);

            int i = EditorPrefs.GetInt("LipSync_DefaultAutoSyncPreset", 0);
            var presets = AutoSyncUtility.GetPresets();
            var orderedModules = new AutoSyncModule[presets[i].modules.Length];

            for (int m = 0; m < presets[i].modules.Length; m++)
            {
                orderedModules[m] = (AutoSyncModule)ScriptableObject.CreateInstance(presets[i].modules[m]);

                if (!orderedModules[m])
                {
                    EditorUtility.DisplayDialog("AutoSync Failed", string.Format("The requested module '{0}' could not be found. You may need to download it from Window/Rogo Digital/Get Extensions.", presets[i].modules[m]), "Ok");
                    return;
                }

                if (!string.IsNullOrEmpty(presets[i].moduleSettings[m]))
                {
                    JsonUtility.FromJsonOverwrite(presets[i].moduleSettings[m], orderedModules[m]);
                }

                orderedModules[m].hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            }

            window.autoSyncInstance.RunSequence(orderedModules, OnAutoSyncDataReady, (LipSyncData)data);
        } else
        {
            Debug.Log("Failed to load audio: " + metaData.audioFile);
            OnAutoSyncDataReady(null, new AutoSync.ASProcessDelegateData(false, "", ClipFeatures.AudioClip));
        }

        //window.changed = false;
        //window.Close();
    }

    public static void OnAutoSyncDataReady(LipSyncData outputData, AutoSync.ASProcessDelegateData eventData)
    {
        if (eventData.success)
        {
            AssetDatabase.SaveAssets();
            //LipSyncClipSetup.SaveFile(window.settings, lisyncToGenerate[currentDataToGenerate].lipsyncFile, false, window.Transcript, window.FileLength, window.PhonemeData.ToArray(), window.EmotionData.ToArray(), window.GestureData.ToArray(), outputData.clip);
        }

        window.changed = false;
        window.Close();

        currentDataToGenerate++;

        //if (currentDataToGenerate >= 1)
        //{
        //    running = false;
        //    return;
        //}

        if (currentDataToGenerate < lisyncToGenerate.Count)
        {
            GenerateAutoSync(lisyncToGenerate[currentDataToGenerate]);
        } else
        {
            running = false;
        }
    }
}
