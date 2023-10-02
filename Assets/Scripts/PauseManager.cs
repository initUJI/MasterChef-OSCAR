using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public bool paused = false;
    public bool freeze = false;
    public GameObject CanvasGioco;

    public GameObject PanelPausa;
    public GameObject PanelOpzioni;
    public GameObject PanelAiuto;
    public GameObject PanelEsci;
    public GameObject SaveManager;
    SaveManager savings;

    void Start()
    {
        savings = SaveManager.GetComponent<SaveManager>();
    }

    public void Riprendi()
    {
        paused = false;
        this.gameObject.SetActive(false);
        if (!freeze)
        {
            Cursor.lockState = CursorLockMode.Locked;
            CanvasGioco.transform.Find("Panel").gameObject.SetActive(true);
        }
    }

    public void Opzioni()
    {
        PanelOpzioni.transform.Find("Volume").GetComponent<Slider>().value = GameManager.volume;
        PanelOpzioni.transform.Find("Sensitivity").GetComponent<Slider>().value = GameManager.sensitivity;
        PanelPausa.SetActive(false);
        PanelOpzioni.SetActive(true);
    }

    public void OpzioniEsci()
    {
        PanelOpzioni.SetActive(false);
        PanelPausa.SetActive(true);
        savings.data.option.volume = GameManager.volume;
        savings.data.option.sensitivity = GameManager.sensitivity;
        savings.Save();
    }

    public void Aiuto()
    {
        PanelPausa.SetActive(false);
        PanelAiuto.SetActive(true);
    }

    public void AiutoEsci()
    {
        PanelAiuto.SetActive(false);
        PanelPausa.SetActive(true);
    }

    public void Esci()
    {
        PanelPausa.SetActive(false);
        PanelEsci.SetActive(true);
    }

    public void EsciSi()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void EsciNo()
    {
        PanelEsci.SetActive(false);
        PanelPausa.SetActive(true);
    }

    public void ResetPanel()
    {
        PanelPausa.SetActive(true);
        PanelOpzioni.SetActive(false);
        PanelAiuto.SetActive(false);
        PanelEsci.SetActive(false);
    }
}