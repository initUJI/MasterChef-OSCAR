using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RecipeManager : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        /* Tipi di step di ricetta:
        * 0) Muovi object1 in object2
        * 1) Muovi object1 in object2 senza farlo comparire
        * 2) Muovi object1 in object2 facendo comparire object3
        * 3) Usa object1 su object2, senza cambiare object2 (se ha figli li elimina)
        * 4) Usa object1 sull'oggetto object2, cambiando (il figlio di) object2 in object3
        * 5) Versa il contenuto di object1 in object2, facendo comparire object3 (n=0 cancella il contenuto da object1, n=1 no)
        * 6) Usa lo strumento
        * 7) Usa lo strumento e cambia in object3 il figlio del recipiente contenuto in object1
        * 8) Usa object1 su object2, tenendo in mano object1
        */
        public int type;
        public List<string> object1 = new List<string>();
        public List<string> object2 = new List<string>();
        public List<string> object3 = new List<string>();
        public int n;
        public string audio1;
        public string audio2;
    }

    [System.Serializable]
    public class RecipeStep
    {
        public string Text;
        public List<Step> Step = new List<Step>();
        public Queue<Step> StepQueue;
    }

    [System.Serializable]
    public class Recipe
    {
        public List<string> Ingredients = new List<string>();
        public List<RecipeStep> RecipeStep = new List<RecipeStep>();
        public Queue<RecipeStep> RecipeStepQueue;
    }

    public Recipe recipe = new();

    // Start is called before the first frame update
    void Awake()
    {
        //string readFromFilePath = Application.dataPath + "/Texts/" + GameManager.levelNumber + "/Recipe/Recipe.json";
        //string fileText = File.ReadAllText(readFromFilePath);

        string reseourcePath = "Texts/" + GameManager.levelNumber + "/Recipe/Recipe";
        TextAsset fileTextAsset = Resources.Load<TextAsset>(reseourcePath);
        string fileText = fileTextAsset.text;

        recipe = JsonUtility.FromJson<Recipe>(fileText);

        recipe.RecipeStepQueue = new Queue<RecipeStep>(recipe.RecipeStep);
        foreach(RecipeStep recipeStep in recipe.RecipeStepQueue)
        {
            recipeStep.StepQueue = new Queue<Step>(recipeStep.Step);
        }
    }

    public bool ControlItem(int type, string name, int i)
    {
        Step step = recipe.RecipeStepQueue.Peek().StepQueue.Peek();
        int t = step.type;
        if (i == 1)
        {
            if (step.object1.Contains(name))
            {
                if (type == 0)
                {
                    if (t >= 0 && t <= 2)
                    {
                        return true;
                    }
                }
                else if (type == 3)
                {
                    if (t >= 3 && t <= 5)
                    {
                        return true;
                    }
                }
                else if (type == 6)
                {
                    if (t == 6 || t == 7)
                    {
                        return true;
                    }
                }
                else if (type == 8)
                {
                    if (t == 8)
                    {
                        return true;
                    }
                }
            }
        }
        else if (i == 2)
        {
            if (step.object2.Contains(name))
            {
                if (type == 0)
                {
                    if (t >= 0 && t <= 2)
                    {
                        return true;
                    }
                }
                else if (type == 3)
                {
                    if (t >= 3 && t <= 5)
                    {
                        return true;
                    }
                }
                else if (type == 8)
                {
                    if (t == 8)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool ControlItem(string name, int i)
    {
        Step step = recipe.RecipeStepQueue.Peek().StepQueue.Peek();
        if (i == 1)
        {
            if (step.object1.Contains(name))
            {
                return true;
            }
        }
        else if (i == 2)
        {
            if (step.object2.Contains(name))
            {
                return true;
            }
        }
        return false;
    }

    public void DequeueStep()
    {
        recipe.RecipeStepQueue.Peek().StepQueue.Dequeue();
        if (recipe.RecipeStepQueue.Peek().StepQueue.Count == 0)
        {
            recipe.RecipeStepQueue.Dequeue();
        }
    }
}
