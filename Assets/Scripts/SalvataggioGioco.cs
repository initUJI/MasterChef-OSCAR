using BayatGames.SaveGameFree.Serializers;
using System.Collections.Generic;
using UnityEngine;

namespace BayatGames.SaveGameFree.Examples
{
    public class SalvataggioGioco : MonoBehaviour
    {
        public static int nSalvataggio = 1;
        public const int N = 6;

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

        [System.Serializable]
        public class CustomData
        {
            public Player[] players = new Player[N];
            public float volume;
            public float sensitivity;

            public CustomData()
            {
                for(int i = 0; i < N; i++)
                {
                    players[i] = new Player();
                }
                volume = 1f;
                sensitivity = 1f;
            }
        }

        public CustomData customData;
        public string identifier = "savings";

        public void Awake()
        {
            customData = new CustomData();
            Load();
        }

        // Salva tutti i dati
        public void Save()
        {
            SaveGame.Save<CustomData>(identifier, customData);
        }
        
        // Carica tutti i dati
        public void Load()
        {
            if(SaveGame.Exists(identifier))
            {
                customData = SaveGame.Load<CustomData>(identifier, new CustomData());
            }
        }
        
        // Imposta il numero del salvataggio selezionato
        public void SetNSalvataggio(int i)
        {
            nSalvataggio = i;
        }

        // Restituisce il numero del salvataggio selezionato
        public int GetNSalvataggio()
        {
            return nSalvataggio;
        }

        // Crea un nuovo giocatore senza un username
        public void CreatePlayer()
        {
            customData.players[nSalvataggio].empty = false;
            Save();
        }

        // Crea un nuovo giocatore con un username
        public void CreatePlayer(string username)
        {
            customData.players[nSalvataggio].empty = false;
            customData.players[nSalvataggio].username = username;
            Save();
        }
        
        // Cancella i dati del giocatore in posizione nSalvataggio
        public void DeletePlayer()
        {
            customData.players[nSalvataggio] = new Player();
            Save();
        }

        // Salva le opzioni per il gioco desktop
        public void SaveOptions(float volume, float sensitivity)
        {
            customData.volume = volume;
            customData.sensitivity = sensitivity;
            GameManager.volume = volume;
            GameManager.sensitivity = sensitivity;
            Save();
        }

        // Salva le opzioni per il gioco desktop
        public void SaveOptions(float volume)
        {
            customData.volume = volume;
            GameManager.volume = volume;
            Save();
        }

        // Restituisce il numero massimo di salvataggi
        public int GetN()
        {
            return N;
        }
    }
}