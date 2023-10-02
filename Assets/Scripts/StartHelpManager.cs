using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHelpManager : MonoBehaviour
{
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    PauseManager pauseManager;

    void Start()
    {
        if (GameManager.help)
        {
            GameManager.help = false;
            pauseManager = pauseCanvas.GetComponent<PauseManager>();
            pauseManager.freeze = true;

            if (!SetVROrNot.isOnXRDevice)
            {
                //MOUSE
                //Disattiva il canvas principale
                gameCanvas.transform.Find("Panel").gameObject.SetActive(false);

                //Disattiva il lock del mouse
                Cursor.lockState = CursorLockMode.None;

                this.transform.Find("MouseHelp").gameObject.SetActive(true);
                this.transform.Find("VRHelp").gameObject.SetActive(false);
            }
            else
            {
                //VR
                // ATTIVA I MURI INVISIBILI OPPURE PORTA L'UTENTE IN UNA STANZA PER LEGGERE
                //ATTIVA L'HELP INIZIALE PER VR
                this.transform.Find("VRHelp").gameObject.SetActive(true);
                this.transform.Find("MouseHelp").gameObject.SetActive(false);
            }
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void CloseHelp()
    {
        this.gameObject.SetActive(false);
        pauseManager.freeze = false;
        if (!SetVROrNot.isOnXRDevice)
        {
            gameCanvas.transform.Find("Panel").gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {

        }
    }
}
