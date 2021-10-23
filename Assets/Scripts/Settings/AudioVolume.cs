using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVolume : MonoBehaviour
{
    public AudioSource audioSource;

    private static AudioVolume _audioVolume;
    public static AudioVolume audioVolume
    {
        get
        {
            if (_audioVolume == null)
            {
                _audioVolume = FindObjectOfType<AudioVolume>();
            }

            return _audioVolume;
        }
    }

    void Awake()
    {
        if (_audioVolume == null)
        {
            Settings.LoadSettings();
            audioSource.volume = Settings.volume;
            _audioVolume = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _audioVolume)
                Destroy(this.gameObject);
        }
    }
}
