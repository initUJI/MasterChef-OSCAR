using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRManager : MonoBehaviour
{
    public GameObject interactorObject;
    public static Interactor interactor; //Questo valore lo leggo nel vero manager, che gestisce il click

    public GameObject canvasVRObject;
    public static TextMeshProUGUI textHUD;

    public GameObject pauseCanvas;
    public static PauseManager pauseManager;

    public static Transform parentOriginal;
    public static Vector3 positionOriginal;
    public static Quaternion rotationOriginal;
    public static bool positionSaved = false;
    public static bool timeout = false;
    public static VRManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        interactor = interactorObject.GetComponent<Interactor>();
        pauseManager = pauseCanvas.GetComponent<PauseManager>();
        textHUD = canvasVRObject.transform.Find("Panel").Find("TextHUD").GetComponent<TextMeshProUGUI>();
        textHUD.text = "";
    }

    public static void Timeout(float seconds)
    {
        //instance.StartCoroutine("VRTimeout", seconds);
        instance.StartCoroutine(nameof(VRTimeout), seconds);
    }

    public IEnumerator VRTimeout(float seconds)
    {
        timeout = true;
        yield return new WaitForSeconds(seconds);
        timeout = false;
    }

    public static void ChangeHUD(string s)
    {
        textHUD.text = s;
    }

}