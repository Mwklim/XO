using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Text RatingPlayer;

    [SerializeField] GameObject prefabInputName;
    GameObject inputName;

    void Start()
    {
        Settings.LoadSettings();
        RatingPlayer.text = Settings.rating.ToString();
    }


    public void LoadGame()
    {
        if (Settings.firstStart && Settings.namePlayer == "Player")
        {
            inputName = Instantiate(prefabInputName);

            StatsCallback callback = LoadGameNow;
            inputName.GetComponent<Window>().callback.Add(callback);
        }
        else
        {
            LoadGameNow(new object());
        }
    }

    public void LoadGameNow(object load)
    {
        Settings.firstStart = false;
        Settings.SaveSettings();
        LoadScene.loadScene.LoadSceneAsync("Game");
    }


}


