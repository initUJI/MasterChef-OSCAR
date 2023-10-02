using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TabletManager : MonoBehaviour
{
    public GameObject CanvasPausa;
    PauseManager pauseManager;

    public GameObject PanelPausa;
    public GameObject PanelOpzioni;
    public GameObject PanelAiuto;
    public GameObject PanelEsci;

    GameObject canvas;   
    Transform parentOriginal;
    Vector3 positionOriginal;
    Quaternion rotationOriginal;

    public GameObject SaveManager;
    SaveManager savings;

    void Start()
    {
        if (SetVROrNot.isOnXRDevice)
        {
            savings = SaveManager.GetComponent<SaveManager>();
            pauseManager = CanvasPausa.GetComponent<PauseManager>();
            canvas = this.GetComponentInChildren<Canvas>(true).gameObject;
            parentOriginal = this.transform.parent;
            positionOriginal = this.transform.localPosition;
            rotationOriginal = this.transform.rotation;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SetPause(bool b)
    {
        pauseManager.paused = b;
    }

    public void Riprendi()
    {
        this.transform.parent = parentOriginal;
        this.transform.localPosition = positionOriginal;
        this.transform.rotation = rotationOriginal;
        this.GetComponent<BoxCollider>().enabled = true;
        canvas.SetActive(false);
        VRManager.positionSaved = false;
        VRManager.interactor.hand = null;
        VRManager.interactor.handOccupied = false;
        SetPause(false);
    }

    public void Opzioni()
    {
        PanelOpzioni.transform.Find("Volume").GetComponent<Slider>().value = GameManager.volume;
        PanelPausa.SetActive(false);
        PanelOpzioni.SetActive(true);
    }

    public void OpzioniEsci()
    {
        PanelOpzioni.SetActive(false);
        PanelPausa.SetActive(true);
        savings.data.option.volume = GameManager.volume;
        savings.Save();
    }

    public void Aiuto()
    {

    }

    public void Esci()
    {
        PanelPausa.SetActive(false);
        PanelEsci.SetActive(true);
    }

    public void EsciSi()
    {
        SceneManager.LoadScene("MainMenuVR");
    }

    public void EsciNo()
    {
        PanelEsci.SetActive(false);
        PanelPausa.SetActive(true);
    }

}
