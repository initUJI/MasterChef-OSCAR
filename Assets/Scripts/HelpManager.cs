using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{

    [System.Serializable]
    public class Help
    {
        public List<string> Pages;
    }

    public Help help = new();
    public List<Sprite> images = new();
    public string helpName;
    Button previous;
    Button next;
    
    int currentLine;

    void Start()
    {
        string reseourcePath = "Texts/Help_" + helpName;
        TextAsset fileTextAsset = Resources.Load<TextAsset>(reseourcePath);
        string fileText = fileTextAsset.text;
        help = JsonUtility.FromJson<Help>(fileText);
        for (int i = 0; i < help.Pages.Count; i++)
        {
            images.Add(Resources.Load<Sprite>("Images/Help/" + helpName + "/" + i));
        }
        this.transform.Find("Img").GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        currentLine = 0;
        previous = this.transform.Find("Previous").GetComponent<Button>();
        next = this.transform.Find("Next").GetComponent<Button>();
        previous.interactable = false;
        ChangePage();
    }

    public void PreviousHelp()
    {
        if (currentLine != 0)
        {
            currentLine--;
            ChangePage();
            if (currentLine == help.Pages.Count - 2)
            {
                next.interactable = true;
            }
            if (currentLine == 0)
            {
                previous.interactable = false;
            }
        }
    }

    public void NextHelp()
    {
        if (currentLine != help.Pages.Count - 1)
        {
            currentLine++;
            ChangePage();
            if (currentLine == 1)
            {
                previous.interactable = true;
            }
            if (currentLine == help.Pages.Count - 1)
            {
                next.interactable = false;
            }
        }
    }

    void ChangePage()
    {
        this.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = help.Pages[currentLine];
        this.transform.Find("Img").GetComponent<Image>().sprite = images[currentLine];
    }
}
