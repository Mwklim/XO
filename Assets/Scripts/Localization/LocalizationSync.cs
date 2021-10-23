using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[ExecuteInEditMode]
public class LocalizationSync : MonoBehaviour
{
    public string TableId;
    public Sheet[] Sheets;
 
    private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";
    private static LocalizationSync _instance;
    public static LocalizationSync instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LocalizationSync>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;


            if (!LocalizationManager.LoadStartDictionary())
            {
                LocalizationManager.ReadResourse("StartGame");
                StartCoroutine(LocalizationManager.ReadResourseBackGround());
            }

            DownloadLocalizationStart();

            if (Application.isPlaying)
                DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        ResetLocalization();
    }

    public void DownloadLocalizationStart()
    {
        StartCoroutine(DownloadLocalization());
    }

    public void ResetLocalization()
    {
        if (LocalizationManager.loadLocalization && LoadScene.loadScene.ExpectationLoading.Count == 0)
        {
            LocalizationManager.loadLocalization = false;
            StartCoroutine(LocalizationManager.ReedBackground());
        }
    }

    private IEnumerator DownloadLocalization(int sheets = -1) //скачивание с игры ('-1' - скачать все)
    {
        Debug.Log("Sync started!");

        var dict = new Dictionary<string, UnityWebRequest>();
        SaveLocalizationText textLoad = new SaveLocalizationText();

        for (int i = 0; i < Sheets.Length; i++)
        {
            if (sheets == -1 || sheets == i)
            {
                var url = string.Format(UrlPattern, TableId, Sheets[i].Id);
                Debug.LogFormat("Sync request: {0}", Sheets[i].Name);
                dict.Add(url, UnityWebRequest.Get(url));
            }
        }

        foreach (var entry in dict)
        {
            var url = entry.Key;
            var request = entry.Value;

            if (!request.isDone)
            {
                yield return request.Send();
            }

            if (request.error == null)
            {
                var sheet = Sheets.Single(i => url == string.Format(UrlPattern, TableId, i.Id));

                string textDownload = BytesToString(request.downloadHandler.data);
                if (CheckDownload(textDownload))
                {
                    textLoad.name.Add(sheet.Name);
                    textLoad.text.Add(textDownload);
                    Debug.Log("Sync was successful: " + sheet.Name);
                }
                else Debug.Log("No access to server: " + sheet.Name);
            }
            else
            {
                try
                {
                    throw new Exception(request.error);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }

        if (textLoad.name.Count > 0)
        {
            LocalizationManager.LocalizationText = textLoad;
            LocalizationManager.loadLocalization = true;
        }

        Debug.Log("Sync ended!");
    }

    string BytesToString(byte[] bytes)
    {
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }

    bool CheckDownload(string text, int lengthText = 10) //проверка на загрузку нужной страницы 
    {
        if (text.Length > lengthText)
            text = text.Substring(0, lengthText);

        var indexText = text.Split(',').Select(i => i.Trim()).ToList();

        return (indexText[0] == "Index");
    }



#if UNITY_EDITOR

    public void Sync()
    {
        StopAllCoroutines();
        StartCoroutine(SyncCoroutine());
    }

    private IEnumerator SyncCoroutine()  //скачивание с эмулятора
    {
        Debug.Log("<color=yellow>Sync started, please wait for confirmation message...</color>");

        var dict = new Dictionary<string, UnityWebRequest>();

        foreach (var sheet in Sheets)
        {
            var url = string.Format(UrlPattern, TableId, sheet.Id);

            Debug.LogFormat("Downloading: {0}...", url);

            dict.Add(url, UnityWebRequest.Get(url));
        }

        foreach (var entry in dict)
        {
            var url = entry.Key;
            var request = entry.Value;

            if (!request.isDone)
            {
                yield return request.Send();
            }

            if (request.error == null)
            {
                var sheet = Sheets.Single(i => url == string.Format(UrlPattern, TableId, i.Id));
                var path = System.IO.Path.Combine(AssetDatabase.GetAssetPath(sheet.SaveFolder), sheet.Name + ".csv");
                System.IO.File.WriteAllBytes(path, request.downloadHandler.data);
                Debug.LogFormat("Sheet {0} downloaded to {1}", sheet.Id, path);
            }
            else
            {
                throw new Exception(request.error);
            }
        }
        Debug.Log("<color=green>Localization successfully synced!</color>");
    }

#endif
}

[Serializable]
public struct Sheet
{
    public string Name;
    public long Id;

    public UnityEngine.Object SaveFolder;
}
