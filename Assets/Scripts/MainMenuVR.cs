using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuVR : MonoBehaviour
{
    public GameObject Simulator;
    public bool forceSimulator;
    void Awake()
    {
        SetVROrNot.isOnXRDevice = true;
        //Simulator.SetActive(SetVROrNot.isSimulatorOn);
        Simulator.SetActive(forceSimulator);
    }
}
