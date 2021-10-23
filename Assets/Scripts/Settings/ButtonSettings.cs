using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonSettings : MonoBehaviour
{
    [SerializeField] GameObject prefabSettings;
    GameObject settings;

    public void SettingsOpen()
    {
        settings = Instantiate(prefabSettings);

        StatsCallback callback = SetVolume;
        settings.GetComponent<Window>().callback.Add(callback);

        for (int i = 0; i < 2; i++)
            InitializedSetLang();
    }

    void InitializedSetLang()
    {
        StatsCallback callback = SetLang;
        settings.GetComponent<Window>().callback.Add(callback);
    }

    void SetVolume(object volume)
    {
        Settings.volume = (float)volume;
        Settings.SaveSettings();
        AudioVolume.audioVolume.audioSource.volume = (float)volume;
    }

    void SetLang(object lang)
    {
        LocalizationManager.Language = lang.ToString();
    }
}
