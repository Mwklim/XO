using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckLang : CallEvent
{
    void Start()
    {
        Localize();
        LocalizationManager.LocalizationChanged += Localize;
    }

    public void OnDestroy()
    {
        LocalizationManager.LocalizationChanged -= Localize;
    }

    void Localize()
    {
        GetComponent<Text>().color = (gameObject.name == LocalizationManager.Language) ? new Color(1f, 0, 0.5f) : new Color(1f, 1f, 1f);
    }
}
