using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class SetVROrNot : MonoBehaviour
{
    public GameObject XRRig;
    public GameObject MouseRig;
    public GameObject Simulator;
    public GameObject EventSystem;
    public GameObject Canvas;

    public static bool isOnXRDevice = false;
    public bool forceSimulator;
    private void Awake()
    {
        if (forceSimulator)
        {
            isOnXRDevice = true;
        }
        XRRig.SetActive(isOnXRDevice);
        MouseRig.SetActive(!isOnXRDevice);
        Canvas.SetActive(!isOnXRDevice);
        Simulator.SetActive(forceSimulator);
        EventSystem.GetComponent<XRUIInputModule>().enabled = isOnXRDevice;
        EventSystem.GetComponent<InputSystemUIInputModule>().enabled = !isOnXRDevice;
    }
}
