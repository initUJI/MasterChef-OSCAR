using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class TVIngredientsManager : MonoBehaviour
{

    [System.Serializable]
    public class ReadText
    {
        public string Title;
        public string Type;
        public string[] Text;
    }

    public ReadText readText = new();
    public int currentLine;
    public string fileName;
    public GameObject PlayerRig;
    AudioManager audioManager;

    Transform previous;
    Transform next;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = PlayerRig.GetComponent<AudioManager>();
        string readFromFilePath = Application.dataPath + "/Texts/" + GameManager.levelNumber + "/Recipe/" + fileName + ".json";
        string fileText = File.ReadAllText(readFromFilePath);
        readText = JsonUtility.FromJson<ReadText>(fileText);

        currentLine = 0;
        this.transform.Find("Title").GetComponent<TextMeshPro>().text = readText.Title;
        this.transform.Find("Text").GetComponent<TextMeshPro>().text = readText.Text[currentLine];
 
        previous = this.transform.Find("Previous");
        next = this.transform.Find("Next");
        SetButton(previous, false);
        SetButton(next, true);
    }

    public void PreviousText()
    {
        if (currentLine != 0)
        {
            currentLine--;
            this.transform.Find("Text").GetComponent<TextMeshPro>().text = readText.Text[currentLine];
            if (currentLine == readText.Text.Length - 2)
            {
                SetButton(next, true);
                //this.transform.Find("Next").GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 255);
                //this.transform.Find("NextBlocked").gameObject.SetActive(false);
            }
            if (currentLine == 0)
            {
                SetButton(previous, false);
                //this.transform.Find("Previous").GetComponent<Renderer>().material.color = new Color32(150, 150, 150, 255);
                //this.transform.Find("PreviousBlocked").gameObject.SetActive(true);
            }
        }
    }

    public void NextText()
    {
        if (currentLine != readText.Text.Length - 1)
        {
            currentLine++;
            this.transform.Find("Text").GetComponent<TextMeshPro>().text = readText.Text[currentLine];
            if (currentLine == 1)
            {
                SetButton(previous, true);
                //this.transform.Find("Previous").GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 255);
                //this.transform.Find("PreviousBlocked").gameObject.SetActive(false);
            }
            if (currentLine == readText.Text.Length - 1)
            {
                SetButton(next, false);
                //this.transform.Find("Next").GetComponent<Renderer>().material.color = new Color32(150, 150, 150, 255);
                //this.transform.Find("NextBlocked").gameObject.SetActive(true);
            }
        }
    }

    public void PlayAudio()
    {
        string path = "Sounds/Ricette/" + GameManager.levelNumber;
        if (readText.Type == "Steps")
        {
            path = path + "/steps/step_" + (currentLine + 1);
        }
        else if (readText.Type == "Ingredients")
        {
            path = path + "/ingredients/ingredient_" + (currentLine + 1);
        }
        audioManager.StopAudio();
        audioManager.clip = Resources.Load<AudioClip>(path);
        audioManager.PlayAudio();
    }

    void SetButton(Transform button, bool enable)
    {
        button.GetComponent<Collider>().enabled = enable;
        if (enable)
        {
            button.GetComponent<Renderer>().material.color = new Color32(0, 0, 255, 255);
        }
        else
        {
            button.GetComponent<Renderer>().material.color = new Color32(170, 170, 170, 255);
        }
    }
}
