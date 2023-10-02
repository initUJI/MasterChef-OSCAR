using BayatGames.SaveGameFree.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject PanelMenu;
    public GameObject PanelGioca;
    public GameObject PanelNome;
    public GameObject PanelCancella;
    public GameObject PanelRicette;
    public GameObject PanelOpzioni;
    public GameObject PanelCrediti;
    public GameObject PanelEsci;
    public GameObject SaveManager;
    SaveManager savings;
 
    void Start()
    {
        savings = SaveManager.GetComponent<SaveManager>();
        AudioListener.volume = savings.data.option.volume;
        GameManager.volume = savings.data.option.volume;
        for (int i = 0; i < savings.GetN(); i++)
        {
            Button button = PanelGioca.transform.Find("Salvataggio" + i).Find("Cancella" + i).GetComponent<Button>();
            if (savings.data.players[i].empty == true)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
                TextMeshProUGUI text = PanelGioca.transform.Find("Salvataggio" + i).Find("Utente" + i).Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                text.text = savings.data.players[i].username;
            }
        }
        if (GameManager.boot)
        {
            GameManager.boot = false;
        }
        else
        {
            TornaDalGioco();
        }
    }

    public void Gioca()
    {
        PanelMenu.SetActive(false);
        PanelGioca.SetActive(true);
    }

    public void ApriSalvataggio(int i)
    {
        savings.SetISaving(i);
        if (savings.data.players[i].empty == true)
        {
            /* Versione nome
            PanelGioca.SetActive(false);
            PanelNome.SetActive(true);
            */
            // Versione senza nome
            string saveName = "Salvataggio " + i;
            savings.CreatePlayer(saveName);
            Button button = PanelGioca.transform.Find("Salvataggio" + savings.GetISaving()).Find("Cancella" + savings.GetISaving()).GetComponent<Button>();
            button.interactable = true;
            TextMeshProUGUI text = PanelGioca.transform.Find("Salvataggio" + savings.GetISaving()).Find("Utente" + savings.GetISaving()).Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            text.text = saveName;

            GameManager.username = savings.data.players[i].username;
            PanelGioca.SetActive(false);
            PanelRicette.SetActive(true);
        }
        else
        {
            GameManager.username = savings.data.players[i].username;
            PanelGioca.SetActive(false);
            PanelRicette.SetActive(true);
        }
    }

    public void NomeUtenteOk()
    {
        TMP_InputField name = PanelNome.transform.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        if(name.text != "")
        {
            savings.CreatePlayer(name.text);
            Button button = PanelGioca.transform.Find("Salvataggio" + savings.GetISaving()).Find("Cancella" + savings.GetISaving()).GetComponent<Button>();
            button.interactable = true;
            TextMeshProUGUI text = PanelGioca.transform.Find("Salvataggio" + savings.GetISaving()).Find("Utente" + savings.GetISaving()).Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            text.text = name.text;
            name.text = "";

            GameManager.username = savings.data.players[savings.GetISaving()].username;
            PanelNome.SetActive(false);
            PanelRicette.SetActive(true);
        }
    }

    public void NomeUtenteAnnulla()
    {
        TMP_InputField name = PanelNome.transform.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        name.text = "";
        PanelNome.SetActive(false);
        PanelGioca.SetActive(true);
    }

    public void CancellaSalvataggio(int i)
    {
        savings.SetISaving(i);
        PanelGioca.SetActive(false);
        PanelCancella.SetActive(true);
    }

    public void CancellaSi()
    {
        savings.DeletePlayer();
        Button button = PanelGioca.transform.Find("Salvataggio" + savings.GetISaving()).Find("Cancella" + savings.GetISaving()).GetComponent<Button>();
        button.interactable = false;
        TextMeshProUGUI text = PanelGioca.transform.Find("Salvataggio" + savings.GetISaving()).Find("Utente" + savings.GetISaving()).Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        text.text = "-- vuoto --";
        PanelCancella.SetActive(false);
        PanelGioca.SetActive(true);
    }

    public void CancellaNo()
    {
        PanelCancella.SetActive(false);
        PanelGioca.SetActive(true);
    }

    public void GiocaEsci()
    {
        PanelGioca.SetActive(false);
        PanelMenu.SetActive(true);
    }

    public void SelezionaRicetta(int recipeNumber)
    {
        PanelRicette.transform.Find("Book").gameObject.SetActive(false);
        PanelRicette.transform.Find("Difficulty").gameObject.SetActive(true);
        PanelRicette.transform.Find("Difficulty").Find("Title").GetComponent<TextMeshProUGUI>().text = PanelRicette.transform.Find("Book").Find("Page" + recipeNumber).Find("Title").GetComponent<TextMeshProUGUI>().text;
        GameManager.levelNumber = recipeNumber;
    }

    public void IndietroRicette()
    {
        PanelRicette.SetActive(false);
        PanelGioca.SetActive(true);
    }

    public void SelezionaDifficolta(string difficulty)
    {
        PanelRicette.transform.Find("Difficulty").gameObject.SetActive(false);
        
        GameManager.difficulty = difficulty;
        GameManager.exercise1 = false;
        GameManager.exercise2 = false;
        GameManager.exercise3 = false;

        StartCoroutine(LoadSceneAsync("MainScene"));
    }

    IEnumerator LoadSceneAsync(string name)
    {
        PanelRicette.transform.Find("Loading").gameObject.SetActive(true);
        Slider slider = PanelRicette.transform.Find("Loading").GetComponentInChildren<Slider>();        
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }

    public void IndietroDifficolta()
    {
        PanelRicette.transform.Find("Difficulty").gameObject.SetActive(false);
        PanelRicette.transform.Find("Book").gameObject.SetActive(true);
    }

    public void Opzioni()
    {
        PanelOpzioni.transform.Find("Volume").GetComponent<Slider>().value = GameManager.volume;
        PanelOpzioni.transform.Find("Sensitivity").GetComponent<Slider>().value = GameManager.sensitivity;
        PanelMenu.SetActive(false);
        PanelOpzioni.SetActive(true);
    }

    public void OpzioniVR()
    {
        PanelOpzioni.transform.Find("Volume").GetComponent<Slider>().value = GameManager.volume;
        PanelMenu.SetActive(false);
        PanelOpzioni.SetActive(true);
    }

    public void OpzioniEsci()
    {
        PanelOpzioni.SetActive(false);
        PanelMenu.SetActive(true);
        savings.data.option.volume = GameManager.volume;
        savings.data.option.sensitivity = GameManager.sensitivity;
        savings.Save();
    }

    public void Crediti()
    {
        PanelMenu.SetActive(false);
        PanelCrediti.SetActive(true);
    }

    public void CreditiEsci()
    {
        PanelCrediti.SetActive(false);
        PanelMenu.SetActive(true);
    }

    public void Esci()
    {
        PanelMenu.SetActive(false);
        PanelEsci.SetActive(true);
    }

    public void EsciSi()
    {
        // Application.Quit() non funziona nell'editor quindi settiamo UnityEditor.EditorApplication.isPlaying a false per chiudere il gioco
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void EsciNo()
    {
        PanelEsci.SetActive(false);
        PanelMenu.SetActive(true);
    }

    void TornaDalGioco()
    {
        PanelMenu.SetActive(false);
        PanelRicette.SetActive(true);
    }
}
