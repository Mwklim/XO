using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public delegate void StatsCallback(object value = null);

public class Window : WindowInfo
{
    public List<StatsCallback> callback = new List<StatsCallback>();

    void Start()
    {
        StartCoroutine(ShowLoad());
    }

    public void HideSettings()
    {
        StartCoroutine(HideLoad());
    }

    public void CallEvent(int ev = 0, object value = null)
    {
        callback[ev].Invoke(value);
    }

    public void CallEvent(int ev = 0)
    {
        callback[ev].Invoke();
    }

    public void Call(int ev = 0)
    {
        callback[ev].Invoke();
        HideSettings();
    }

    IEnumerator ShowLoad()
    {
        yield return SetBackground(0.7f);
        yield return null;
        yield return SetBasicInfo(1f);
    }

    IEnumerator HideLoad()
    {
        StopCoroutine(ShowLoad());
        yield return SetBasicInfo(0);
        yield return null;
        yield return SetBackground(0);
        Destroy(gameObject);
    }
}

