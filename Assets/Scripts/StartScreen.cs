using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject PanelMain;
    public GameObject PanelEsci;
    public bool simulator = false;
     
    public void PlayMouse()
    {
        SetVROrNot.isOnXRDevice = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayVR()
    {
        SetVROrNot.isOnXRDevice = true;
        //SetVROrNot.isSimulatorOn = simulator;
        SceneManager.LoadScene("MainMenuVR");
    }

    public void Esci()
    {
        PanelMain.SetActive(false);
        PanelEsci.SetActive(true);
    }

    // Se siamo nell'editor di Unity settiamo a false UnityEditor.EditorApplication.isPlaying per chiudere il gioco, altrimenti usiamo Application.Quit()
    public void EsciSi()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void EsciNo()
    {
        PanelEsci.SetActive(false);
        PanelMain.SetActive(true);
    }
}
