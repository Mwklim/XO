using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadScene : WindowInfo
{
    [SerializeField] Text InfoLoadScene;
    [SerializeField] Text TextProgress;
    [SerializeField] Image ImageProgress;

    private string scene = "Menu";

    public List<ProgressLoading> ExpectationLoading = new List<ProgressLoading>();//ожидание загрузки

    private static LoadScene _loadScene;
    public static LoadScene loadScene
    {
        get
        {
            if (_loadScene == null)
            {
                _loadScene = FindObjectOfType<LoadScene>();
                if (Application.isPlaying)
                    DontDestroyOnLoad(_loadScene.gameObject);
            }
            return _loadScene;
        }
    }

    void Awake()
    {
        if (_loadScene == null)
        {
            _loadScene = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _loadScene)
                Destroy(this.gameObject);
        }
    }

    void Start()
    {
        LoadSceneAsync(scene, "StartGame");
    }

    public void LoadSceneAsync(string nameScene, string nameText = "")
    {
        scene = nameScene;
        InfoLoadScene.text = LocalizationManager.Localize("Load." + nameScene + nameText);
        StartCoroutine(AsyncLoad());
    }

    IEnumerator AsyncLoad()
    {
        SetProgress(0);//сброс информации
        yield return SetBackground(1f);
        yield return null;
        yield return SetBasicInfo(1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;

        ProgressLoading progressLoading = new ProgressLoading();//для имитации длительной загрузки
        ExpectationLoading.Add(progressLoading);

        float progressCount = ExpectationLoading.Count + 1;
        float smoothProgress = 0;

        while (!operation.isDone)
        {
            float progress = (operation.progress / 0.9f) / progressCount;

            if (progressLoading.progress < 1f)//для имитации длительной загрузки
            {
                progressLoading.progress += 0.5f * Time.deltaTime;
                if (progressLoading.progress >= 1f) ExpectationLoading.Remove(progressLoading);
            }

            for (int i = 0; i < progressCount - 1; i++)
            {
                if (i < ExpectationLoading.Count)
                    progress += ExpectationLoading[i].progress / progressCount;
                else
                    progress += 1f / progressCount;
            }

            smoothProgress = CustomMethods.AnimParameter(smoothProgress, progress, (progress - smoothProgress) * 5f);
            SetProgress(smoothProgress);

            if (ExpectationLoading.Count == 0)
            {
                SetProgress(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        yield return SetBasicInfo(0);
        yield return null;
        yield return SetBackground(0);
    }

    void SetProgress(float progress)
    {
        ImageProgress.fillAmount = progress;
        TextProgress.text = string.Format("{0:0}%", progress * 100);
    }
}

public class ProgressLoading
{
    public float progress;
}
