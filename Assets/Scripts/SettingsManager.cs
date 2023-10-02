using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public void SetVolume(float x)
    {
        GameManager.volume = x;
        AudioListener.volume = x;
    }

    public float GetVolume()
    {
        return GameManager.volume;
    }

    public void SetSensibilita(float x)
    {
        GameManager.sensitivity = x;
    }

    public float GetSensibilita()
    {
        return GameManager.sensitivity;
    }
}
