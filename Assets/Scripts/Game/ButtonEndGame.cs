using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEndGame : MonoBehaviour
{
    [SerializeField] GameObject prefabEndGame;
    GameObject exit;

    public void Exit()
    {
        exit = Instantiate(prefabEndGame);
        StatsCallback callback = ExitNow;
        exit.GetComponent<Window>().callback.Add(callback);
    }

    void ExitNow(object exit)
    {
        LoadScene.loadScene.LoadSceneAsync("Menu");
    }
}
