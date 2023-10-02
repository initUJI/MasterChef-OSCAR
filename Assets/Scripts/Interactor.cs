using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Interactor : MonoBehaviour
{
    public bool handOccupied = false;
    List<GameObject> cart;
    public GameObject hand;
    public bool destroyable = false;
    public bool movable = false;

    public GameObject recipeManagerObject;
    RecipeManager recipeManager;

    bool ingredientsComplete = false;
    public GameObject canvasObject;
    public GameObject canvasVRObject;
    UIManager UI;
    
    Coroutine coroutine;
    public GameObject kitchenPlatforms;
    List<Transform> platforms;
    AudioManager audioManager;
    public GameObject exerciseCanvas;
    ExerciseManager exerciseManager;
    public GameObject exerciseCanvasVR;
    ExerciseManager exerciseManagerVR;
    public GameObject TVIngredientsObject;
    TVManager TVIngredients;

    void Start()
    {
        hand = null;
        cart = new List<GameObject>();
        recipeManager = recipeManagerObject.GetComponent<RecipeManager>();
        audioManager = this.GetComponent<AudioManager>();
        exerciseManager = exerciseCanvas.GetComponent<ExerciseManager>();
        exerciseManagerVR = exerciseCanvasVR.GetComponent<ExerciseManager>();

        if (!SetVROrNot.isOnXRDevice)
        {
            UI = canvasObject.GetComponent<UIManager>();
        }
        else
        {
            UI = canvasVRObject.GetComponent<UIManager>();
        }
        coroutine = UI.StartCoroutine(UI.StopTransition());

        TVIngredients = TVIngredientsObject.transform.Find("Screen").GetComponent<TVManager>();
        platforms = new();
        for(int i=0; i < kitchenPlatforms.transform.childCount; i++)
        {
            platforms.Add(kitchenPlatforms.transform.GetChild(i));
        }
    }

    /* Tag creati:
    - Ingredient, assegnato a tutti gli ingredienti presenti nella dispensa
    - Grabbable, assegnato a tutti gli utensili presenti in cucina
    - Interactable, assegnato agli oggetti interagibili (es. forno)
    - Animated, assegnato agli oggetti animati (es. frigo)
    - Teleport, assegnato alle piattaforme di teleport
    - Kitchen, assegnato agli oggetti di posizionamento kitchen
    - NPC, assegnato ai giudici
    - Button, bottone del televisore
    */

    public void Action(GameObject hit)
    {
        switch (hit.tag)
        {
            case "Animated":
                {
                    Animator anim = hit.transform.GetComponent<Animator>();
                    anim.SetBool("isOpening", !anim.GetBool("isOpening"));
                    if (anim.GetBool("isOpening"))
                    {
                        PlayAudio("Cooking/Fridge Open");
                    }
                    else
                    {
                        PlayAudio("Cooking/Fridge Close");
                    }
                }
                break;
            case "Teleport":
                {
                    this.transform.position = hit.transform.position;
                }
                break;
            case "Ingredient":
                {
                    if (!ingredientsComplete)
                    {
                        UI.StopCoroutine(coroutine);
                        if (recipeManager.recipe.Ingredients.Contains(hit.name))
                        {
                            GameObject g = hit;
                            cart.Add(g);
                            g.SetActive(false);
                            recipeManager.recipe.Ingredients.Remove(hit.name);
                            if (!recipeManager.recipe.Ingredients.Contains(hit.name))
                            {
                                TVIngredients.UpdateIngredients(hit.name);
                            }
                            string message = "Ingredient taken<br>" + hit.name;
                            InfoMessage(message);
                            PlayAudio("GUI/Minimal UI Sounds/Coarse Click_Minimal UI Sounds");
                            if (recipeManager.recipe.Ingredients.Count == 0)
                            {
                                ingredientsComplete = true;
                                SpawnIngredients();
                                InfoMessage("Ingredient list completed<br>Return in to the kitchen counter", CalcolaDurata(message));
                                PlayAudio("Exercises/Correct Answer _ Royalty Free Sound Effects #shorts");
                            }
                        }
                        else
                        {
                            foreach (GameObject g in cart)
                            {
                                if (g.name == hit.name)
                                {
                                    ErrorMessage("Ingredient already taken");
                                    return;
                                }
                            }
                            ErrorMessage("Ingredient not in the recipe");
                        }
                    }
                    else if (recipeManager.recipe.RecipeStepQueue.Count == 0)
                    {
                        ErrorMessage("Recipe completed<br>Talk to the judges for the exercises");
                    }
                    else
                    {
                        if (!hit.transform.parent.CompareTag("Kitchen"))
                        {
                            ErrorMessage("All ingredients have been taken");
                        }
                        else if (handOccupied)
                        {
                            if (recipeManager.ControlItem(3, hit.name, 2) || recipeManager.ControlItem(0, hit.name, 2) || recipeManager.ControlItem(8, hit.name, 2))
                            {
                                Interaction(hand, hit);
                            }
                            else
                            {
                                ErrorMessage();
                            }
                        }
                        else if (recipeManager.ControlItem(hit.name, 1))
                        {
                            hand = hit;
                            handOccupied = true;
                            PlayAudio(recipeManager.recipe.RecipeStepQueue.Peek().StepQueue.Peek().audio1);
                        }
                        else
                        {
                            ErrorMessage();
                        }
                    }
                }
                break;
            case "Grabbable":
                {
                    if (recipeManager.recipe.RecipeStepQueue.Count == 0)
                    {
                        ErrorMessage("Recipe completed<br>Talk to the judges for the exercises");
                    }
                    else if (ingredientsComplete)
                    {
                        if (recipeManager.ControlItem(hit.name, 1))
                        {
                            hand = hit;
                            handOccupied = true;
                            PlayAudio(recipeManager.recipe.RecipeStepQueue.Peek().StepQueue.Peek().audio1);
                        }
                        else
                        {
                            ErrorMessage();
                        }
                    }
                    else
                    {
                        ErrorMessage("Take all ingredients first");
                    }
                }
                break;
            case "Kitchen":
                {
                    if (recipeManager.recipe.RecipeStepQueue.Count == 0)
                    {
                        ErrorMessage("Recipe completed<br>Talk to the judges for the exercises");
                    }
                    else if (ingredientsComplete)
                    {
                        if (handOccupied)
                        {
                            if (recipeManager.ControlItem(0, hit.name, 2) || (recipeManager.ControlItem(3, hit.name, 2) && hit.transform.childCount == 0))
                            {
                                Interaction(hand, hit);
                            }
                            else
                            {
                                ErrorMessage();
                            }
                        }
                        else if (recipeManager.ControlItem(hit.name, 1))
                        {
                            hand = hit;
                            handOccupied = true;
                            PlayAudio(recipeManager.recipe.RecipeStepQueue.Peek().StepQueue.Peek().audio1);
                        }
                        else
                        {
                            ErrorMessage();
                        }
                    }
                    else
                    {
                        ErrorMessage("Take all ingredients first");
                    }
                    break;
                }
            case "Interactable":
                {
                    if (recipeManager.recipe.RecipeStepQueue.Count == 0)
                    {
                        ErrorMessage("Recipe completed<br>Go to the judges");
                    }
                    else if (ingredientsComplete)
                    {
                        if (handOccupied)
                        {
                            if (recipeManager.ControlItem(0, hit.name, 2))
                            {
                                Interaction(hand, hit);
                            }
                            else
                            {
                                ErrorMessage();
                            }
                        }
                        else
                        {
                            if (recipeManager.ControlItem(6, hit.name, 1))
                            {
                                Interaction(hit);
                            }
                            else if (recipeManager.ControlItem(0, hit.name, 1))
                            {
                                hand = hit.transform.GetChild(hit.transform.childCount - 1).gameObject;
                                handOccupied = true;
                                PlayAudio(recipeManager.recipe.RecipeStepQueue.Peek().StepQueue.Peek().audio1);
                            }
                            else
                            {
                                ErrorMessage();
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage("Take all ingredients first");
                    }
                }
                break;
            case "NPC":
                {
                    // == invece di >=
                    if (recipeManager.recipe.RecipeStepQueue.Count == 0)
                    {
                        if (!SetVROrNot.isOnXRDevice)
                        {
                            if (!exerciseCanvas.activeSelf)
                            {
                                exerciseManager.InitializeExercise("Exercise" + GameManager.difficulty + hit.name[^1]);
                                //exerciseCanvas.SetActive(true);
                            }
                        }
                        else
                        {
                            exerciseManagerVR.InitializeExercise("Exercise" + GameManager.difficulty + hit.name[^1]);
                        }

                    }
                    else if (!ingredientsComplete)
                    {
                        ErrorMessage("You haven't finished yet,<br>go back in the pantry and take the ingredients!");
                    }
                    else
                    {
                        ErrorMessage("You haven't finished yet,<br>go back to the kitchen counter!");
                    }
                }
                break;
            case "Button":
                {
                    if (hit.name == "Previous")
                    {
                        hit.GetComponentInParent<TVManager>().PreviousText();
                    }
                    else if (hit.name == "Next")
                    {
                        hit.GetComponentInParent<TVManager>().NextText();
                    }
                    else if (hit.name == "Audio")
                    {
                        hit.GetComponentInParent<TVManager>().PlayAudio();
                    }
                    PlayAudio("GUI/Minimal UI Sounds/Click 02_Minimal UI Sounds");
                }
                break;
            case "VRUI":
                {
                    if (hit.name == "Tablet")
                    {
                        if (handOccupied)
                        {
                            hand.transform.parent = VRManager.parentOriginal;
                            hand.transform.localPosition = VRManager.positionOriginal;
                            hand.transform.rotation = VRManager.rotationOriginal;
                            hand.SetActive(true);
                            VRManager.positionSaved = false;
                        }
                        else
                        {
                            handOccupied = true;
                        }
                        hand = hit;
                        hand.GetComponent<BoxCollider>().enabled = false;
                        hand.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
                        hand.GetComponent<TabletManager>().SetPause(true);
                    }
                }
                break;
        }
    }

    void Interaction(GameObject object1, GameObject object2)
    {
        RecipeManager.Step step = recipeManager.recipe.RecipeStepQueue.Peek().StepQueue.Peek();
        PlayAudio(step.audio2);
        StopMessage();
        switch (step.type)
        {
            case 0:
                {
                    object1.transform.parent = object2.transform;
                    object1.transform.position = object2.transform.position;
                    object1.transform.eulerAngles = new Vector3(object1.transform.eulerAngles.x, object1.transform.eulerAngles.y, object1.transform.eulerAngles.z);
                    recipeManager.DequeueStep();
                    handOccupied = false;
                    movable = true;
                    if (object2.name == "Oven")
                    {
                        InfoMessage(step.n + " minutes later...");
                    }
                }
                break;
            case 1:
                {
                    step.object1.Remove(object1.name);
                    if (step.object1.Count == 0)
                    {
                        recipeManager.DequeueStep();
                    }
                    handOccupied = false;
                    destroyable = true;

                }
                break;
            case 2:
                {
                    step.object1.Remove(object1.name);
                    if (step.object1.Count == 0)
                    {
                        recipeManager.DequeueStep();
                    }
                    GameObject obj = Resources.Load<GameObject>("Prefabs/" + step.object3[0]);
                    //PrefabUtility.InstantiatePrefab(obj, object2.transform);
                    var o = Instantiate(obj, object2.transform);
                    o.name = step.object3[0];
                    step.object3.Remove(obj.name);
                    if (step.object3.Count == 0)
                    {
                        step.type = 1;
                    }
                    handOccupied = false;
                    destroyable = true;
                }
                break;
            case 3:
                {
                    recipeManager.DequeueStep();
                    if (object1.transform.childCount > 0)
                    {
                        Destroy(object1.transform.GetChild(object1.transform.childCount - 1).gameObject);
                    }
                    handOccupied = false;
                }
                break;
            case 4:
                {
                    int i = step.object1.IndexOf(object1.name);
                    Transform parent = object2.transform.parent;
                    GameObject obj = Resources.Load<GameObject>("Prefabs/" + step.object3[i]);
                    Destroy(object2);
                    //PrefabUtility.InstantiatePrefab(obj, parent);
                    var o = Instantiate(obj, parent);
                    o.name = step.object3[i];
                    if (object1.CompareTag("Ingredient"))
                    {
                        step.object1.Remove(object1.name);
                        step.object3.RemoveAt(i);
                        if (step.object1.Count == 0)
                        {
                            recipeManager.DequeueStep();
                        }
                    }
                    else
                    {
                        recipeManager.DequeueStep();
                    }
                    handOccupied = false;
                }
                break;
            case 5:
                {
                    Transform parent = object2.transform;
                    GameObject obj = Resources.Load<GameObject>("Prefabs/" + step.object3[0]);
                    //PrefabUtility.InstantiatePrefab(obj, parent);
                    var o = Instantiate(obj, parent);
                    o.name = step.object3[0];
                    step.n--;
                    if (step.n == 0)
                    {
                        Destroy(object1.transform.GetChild(object1.transform.childCount - 1).gameObject);
                        recipeManager.DequeueStep();
                    }
                    handOccupied = false;
                }
                break;
            case 8:
                {
                    recipeManager.DequeueStep();
                    Destroy(object2);
                }
                break;
        }
        if (recipeManager.recipe.RecipeStepQueue.Count == 0)
        {
            PlayAudio("Exercises/Crowd-applause");
        }
    }

    void Interaction(GameObject object1)
    {
        RecipeManager.Step step = recipeManager.recipe.RecipeStepQueue.Peek().StepQueue.Peek();
        PlayAudio(step.audio1);
        StopMessage();
        switch (step.type)
        {
            case 6:
                {
                    recipeManager.DequeueStep();
                }
                break;
            case 7:
                {
                    Transform parent = object1.transform.GetChild(object1.transform.childCount - 1).transform;
                    Destroy(parent.GetChild(object1.transform.childCount - 1).gameObject);
                    GameObject obj = Resources.Load<GameObject>("Prefabs/" + step.object3[0]);
                    var o = Instantiate(obj, parent);
                    o.name = step.object3[0];
                    recipeManager.DequeueStep();
                }
                break;
        }
        GameObject light = object1.transform.Find("Light").gameObject;
        light.SetActive(!light.activeSelf);
        if (recipeManager.recipe.RecipeStepQueue.Count == 0)
        {
            PlayAudio("Exercises/Crowd-applause");
        }
    }

    void SpawnIngredients()
    {
        GameObject g;
        for (int i = 0; i < cart.Count; i++)
        {
            g = cart[i];
            g.transform.parent = platforms[i];
            g.transform.position = platforms[i].position;
            g.transform.eulerAngles = new Vector3(g.transform.eulerAngles.x, this.transform.eulerAngles.y, g.transform.eulerAngles.z);
            g.SetActive(true);
        }
    }

    void PlayAudio(string name)
    {
        if (name != "")
        {
            audioManager.clip = Resources.Load<AudioClip>("Sounds/" + name);
            audioManager.PlayAudio();
        }
    }

    void ErrorMessage()
    {
        ErrorMessage(recipeManager.recipe.RecipeStepQueue.Peek().Text);
    }

    void ErrorMessage(string message)
    {
        UI.StopCoroutine(coroutine);
        coroutine = UI.StartCoroutine(UI.TextTransition(message, new Color32(202, 0, 0, 255), 0, CalcolaDurata(message)));
    }

    void InfoMessage(string message, float wait = 0)
    {
        UI.StopCoroutine(coroutine);
        coroutine = UI.StartCoroutine(UI.TextTransition(message, new Color32(255, 255, 255, 255), wait, CalcolaDurata(message)));
    }

    float CalcolaDurata(string message)
    {
        return message.Length / 10;
    }

    void StopMessage()
    {
        UI.StopCoroutine(coroutine);
        coroutine = UI.StartCoroutine(UI.StopTransition());
    }
}