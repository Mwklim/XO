using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckVolume : CallEvent
{
    void Start()
    {
        GetComponent<Slider>().value = Settings.volume;
    }

}
