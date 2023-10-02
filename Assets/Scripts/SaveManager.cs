using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Numero di salvataggi massimo
    public const int N = 6;

    // Indice del salvataggio corrente
    public static int iSaving = 1;
    
    // Dati dei salvataggio
    public Data data = new();
    
    // Percorso relativo del salvataggio
    public string path = "savings.json";

    // Percorso totale del salvataggio
    string fullPath;


    // Struttura dei singoli livelli
    [System.Serializable]
    public struct Level
    {
        public bool unlocked;
        public bool completed;
        public int score;
        public int highScore;

        public Level(bool unlocked, bool completed)
        {
            this.unlocked = unlocked;
            this.completed = completed;
            score = 0;
            highScore = 0;
        }
    }

    // Struttura dei singoli giocatori
    [System.Serializable]
    public class Player
    {
        public bool empty;
        public string username;
        //public string password;
        public List<Level> levels;

        public Player()
        {
            this.empty = true;
            this.username = null;
            //this.password = null;
            levels = new List<Level>() {
                    new Level (true, false),
                    new Level (false, false)
                };
        }
    }

    // Struttura delle opzioni
    [System.Serializable]
    public class Option
    {
        public float volume;
        public float sensitivity;

        public Option()
        {
            this.volume = 1f;
            this.sensitivity = 1f;
        }
    }

    // Struttura del salvataggio
    [System.Serializable]
    public class Data
    {
        public Player[] players = new Player[N];
        public Option option;

        public Data()
        {
            for (int i = 0; i < N; i++)
            {
                players[i] = new Player();
            }
            option = new Option();
        }
    }   

    // Appena viene caricato lo script, chiamo la funzione Awake per caricare (se presente) il salvataggio, oppure per salvarne uno nuovo
    void Awake()
    {
        fullPath = Application.dataPath + "/" + path;
        if (File.Exists(fullPath))
        {
            Load();
            GameManager.volume = data.option.volume;
            GameManager.sensitivity = data.option.sensitivity;
        }
        else
        {
            Save();
        }
    }

    public void Save()
    {
        string strOutput = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, strOutput);
    }

    public void Load()
    {
        string strInput = File.ReadAllText(fullPath);
        data = JsonUtility.FromJson<Data>(strInput);
    }

    // Imposta il numero del salvataggio selezionato
    public void SetISaving(int i)
    {
        iSaving = i;
    }

    // Restituisce il numero del salvataggio selezionato
    public int GetISaving()
    {
        return iSaving;
    }

    // Crea un nuovo giocatore senza un username
    public void CreatePlayer()
    {
        data.players[iSaving].empty = false;
        Save();
    }

    // Crea un nuovo giocatore con un username
    public void CreatePlayer(string username)
    {
        data.players[iSaving].empty = false;
        data.players[iSaving].username = username;
        Save();
    }

    // Cancella i dati del giocatore in posizione nSalvataggio
    public void DeletePlayer()
    {
        data.players[iSaving] = new Player();
        Save();
    }

    // Salva le opzioni per il gioco desktop
    public void SaveOptions(float volume, float sensitivity)
    {
        data.option.volume = volume;
        data.option.sensitivity = sensitivity;
        GameManager.volume = volume;
        GameManager.sensitivity = sensitivity;
        Save();
    }

    // Salva le opzioni per il gioco desktop
    public void SaveOptions(float volume)
    {
        data.option.volume = volume;
        GameManager.volume = volume;
        Save();
    }

    // Restituisce il numero massimo di salvataggi
    public int GetN()
    {
        return N;
    }
}
