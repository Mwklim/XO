using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : Window
{
    [SerializeField] Text InfoResult;
    [SerializeField] Text ResultRating;
    [SerializeField] float timeDelay = 1.5f;

    float AddRating = 0;
    float nowRating = 0;


    void Start()
    {
        InfoResult.text = LocalizationManager.Localize("Menu.Result_" + Settings.result);
        AddRating = Settings.result == ResultGame.Draw ? 0 : (Settings.result == ResultGame.Victory ? 100 : -100);
        ResultRating.text = "0";

        StartCoroutine(AsyncLoad());
    }

    IEnumerator AsyncLoad()
    {
        yield return SetBackground(0.5f, 0.25f);
        yield return null;
        yield return SetBasicInfo(1f, 0.7f);

        while (timeDelay > 0)
        {
            timeDelay -= Time.deltaTime;

            nowRating = CustomMethods.AnimParameter(nowRating, AddRating, 100f);
            ResultRating.text = (nowRating > 0 ? "+" : "") + string.Format("{0:0}", nowRating);
            yield return null;
        }

        yield return SetBasicInfo(0);

        LoadScene.loadScene.LoadSceneAsync("Menu");

        yield return null;
        yield return SetBackground(0);
    }

}
