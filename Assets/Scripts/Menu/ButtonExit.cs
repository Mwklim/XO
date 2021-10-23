using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonExit : MonoBehaviour
{
    [SerializeField] GameObject prefabExit;
    GameObject exit;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (exit == null)
                Exit();
        }
    }

    public void Exit()
    {
        exit = Instantiate(prefabExit);
        StatsCallback callback = ExitNow;
        exit.GetComponent<Window>().callback.Add(callback);
    }

    void ExitNow(object exit)
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
