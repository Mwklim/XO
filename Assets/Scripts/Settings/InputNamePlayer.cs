using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNamePlayer : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = Settings.namePlayer;
        GetComponent<InputField>().text = Settings.namePlayer;

        var endEdit = new InputField.SubmitEvent();
        endEdit.AddListener(EndEditName);
        GetComponent<InputField>().onEndEdit = endEdit;
    }

    void EndEditName(string name)
    {
        Settings.namePlayer = name;
        Settings.SaveSettings();
    }
}
