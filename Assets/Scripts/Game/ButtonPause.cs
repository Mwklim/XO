using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPause : MonoBehaviour
{
    [SerializeField] GameObject prefabPause;
    GameObject pause;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (pause == null)
                Pause();
        }
    }

    public void Pause()
    {
        if (!Game.game.endGame)
            pause = Instantiate(prefabPause);
    }

}
