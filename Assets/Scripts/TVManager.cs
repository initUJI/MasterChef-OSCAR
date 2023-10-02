using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class TVManager : MonoBehaviour
{
    [System.Serializable]
    public class TVFile
    {
        public string Title;
        public string[] Text;
    }

    public TVFile readText = new();
    public int currentLine;
    public GameObject PlayerRig;
    AudioManager audioManager;

    public string mode;
    int rows = 4;
    public GameObject recipeManagerObject;
    RecipeManager recipeManager;
    List<string> ingredients;

    string title;
    List<string> lines;

    Transform previous;
    Transform next;
    

    // Start is called before the first frame update
    void Start()
    {
        audioManager = PlayerRig.GetComponent<AudioManager>();
        recipeManager = recipeManagerObject.GetComponent<RecipeManager>();
        currentLine = 0;
        
        string reseourcePath = "Texts/" + GameManager.levelNumber + "/Recipe/" + mode;
        TextAsset fileTextAsset = Resources.Load<TextAsset>(reseourcePath);
        string fileText = fileTextAsset.text;

        readText = JsonUtility.FromJson<TVFile>(fileText);
        title = readText.Title;
        lines = new(readText.Text);
        switch (mode)
        {
            case "Steps":
                {
                    /*
                    string reseourcePath = "Texts/" + GameManager.levelNumber + "/Recipe/" + mode;
                    TextAsset fileTextAsset = Resources.Load<TextAsset>(reseourcePath);
                    string fileText = fileTextAsset.text;
                    
                    readText = JsonUtility.FromJson<Steps>(fileText);
                    title = readText.Title;
                    lines = new(readText.Text);*/
                    for (int i = 0; i < lines.Count; i++)
                    {
                        lines[i] = (i+1) + ". " + lines[i];
                    }
                }
                break;
            case "Ingredients":
                {
                    ingredients = new(lines);
                    for (int i = 0; i < ingredients.Count; i++)
                    {
                        ingredients[i] = "- " + ingredients[i];
                    }
                    UpdateRows();
                }
                break;
        }

        this.transform.Find("Title").GetComponent<TextMeshPro>().text = title;
        this.transform.Find("Type").GetComponent<TextMeshPro>().text = mode;
        this.transform.Find("Text").GetComponent<TextMeshPro>().text = lines[currentLine];

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
            ChangeText();
            if (currentLine == lines.Count - 2)
            {
                SetButton(next, true);
            }
            if (currentLine == 0)
            {
                SetButton(previous, false);
            }
        }
    }

    public void NextText()
    {
        if (currentLine != lines.Count - 1)
        {
            currentLine++;
            ChangeText();
            if (currentLine == 1)
            {
                SetButton(previous, true);
            }
            if (currentLine == lines.Count - 1)
            {
                SetButton(next, false);
            }
        }
    }

    public void PlayAudio()
    {
        string path = "Sounds/Ricette/" + GameManager.levelNumber;
        if (mode == "Steps")
        {
            path = path + "/steps/step_" + (currentLine + 1);
        }
        else if (mode == "Ingredients")
        {
            path = path + "/ingredienti/ingredients_" + (currentLine + 1);
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

    void ChangeText()
    {
        this.transform.Find("Text").GetComponent<TextMeshPro>().text = lines[currentLine];
    }

    void UpdateRows()
    {
        
        lines = new();
        string line = "";
        int j = 0;
        for (int i = 0; i < ingredients.Count; i++)
        {
            line = line + ingredients[i] + "<br>";
            j++;
            if (j == rows)
            {
                lines.Add(line);
                line = "";
                j = 0;
            }
        }
        if (j > 0)
        {
            lines.Add(line);
        }
    }

    public void UpdateIngredients(string s)
    {
        string query = "- " + s;
        int index = -1;
        foreach (string ingredient in ingredients)
        {
            if (ingredient.Contains(s) && !ingredient.Contains("<s>"))
            {
                index = ingredients.IndexOf(ingredient);
                break;
            }
        }
        ingredients[index] = "<s>" + ingredients[index] + "</s>";
        UpdateRows();
        ChangeText();
    }
}
