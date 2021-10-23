using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallEvent : MonoBehaviour
{
    public GameObject GameObjectEvent;
    public int eventNumber;

    public void SetValue(string value)
    {
        GameObjectEvent.GetComponent<Window>().CallEvent(eventNumber, value);
    }

    public void SetValue(float value)
    {
        GameObjectEvent.GetComponent<Window>().CallEvent(eventNumber, value);
    }
}
