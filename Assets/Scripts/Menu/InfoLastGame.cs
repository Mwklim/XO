using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoLastGame : MonoBehaviour
{
    [SerializeField] Text InfoResult;
    [SerializeField] Text ResultRating;

    void Start()
    {
        SetResult();
        LocalizationManager.LocalizationChanged += SetResult;
    }

    public void OnDestroy()
    {
        LocalizationManager.LocalizationChanged -= SetResult;
    }

    void SetResult()
    {
        if (Settings.gameLaunch > 0)
        {
            InfoResult.text = LocalizationManager.Localize("Menu.Result_" + Settings.result);
            ResultRating.text = Settings.result == ResultGame.Draw ? "+0" : (Settings.result == ResultGame.Victory ? "+100" : "-100");
        }
        else gameObject.SetActive(false);
    }
}
