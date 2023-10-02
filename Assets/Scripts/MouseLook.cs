using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BayatGames.SaveGameFree.Examples;
using TMPro;

public class MouseLook : MonoBehaviour
{
    public InputActionReference horizontalLook;
    public InputActionReference verticalLook;
    public InputActionReference action;
    public InputActionReference pause;
    //public float lookspeed = 1f;
    public Transform cameraTransform;
    float pitch;
    float yaw;

    Transform cam;
    RaycastHit hit;
    public float distanceView = 7.5f;
    public GameObject gameCanvas;
    UIManager UI;
    GameObject crosshairRed;
    public GameObject pauseCanvas;
    PauseManager pauseManager;
    public GameObject interactorObject;
    Interactor interactor;

    public GameObject hand;
    Transform parentOriginal;
    Vector3 positionOriginal;
    Quaternion rotationOriginal;
    bool positionSaved = false;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        horizontalLook.action.performed += HandleHorizontalLookChange;
        verticalLook.action.performed += HandleVerticalLookChange;
        action.action.performed += HandleAction;
        pause.action.performed += HandlePause;
        cam = this.transform.GetChild(0).transform;
        UI = gameCanvas.GetComponent<UIManager>();
        crosshairRed = gameCanvas.transform.Find("Panel").Find("CrosshairRed").gameObject;
        pauseManager = pauseCanvas.GetComponent<PauseManager>();

        interactor = interactorObject.GetComponent<Interactor>();
    }

    private void HandleHorizontalLookChange(InputAction.CallbackContext obj)
    {
        if(!pauseManager.paused && !pauseManager.freeze)
        {
            yaw += obj.ReadValue<float>();
            transform.localRotation = Quaternion.AngleAxis(yaw * GameManager.sensitivity, Vector3.up);
        }
    }

    private void HandleVerticalLookChange(InputAction.CallbackContext obj)
    {
        if(!pauseManager.paused && !pauseManager.freeze)
        {
            pitch -= obj.ReadValue<float>();
            pitch = Mathf.Clamp(pitch, -90f / GameManager.sensitivity, 90f / GameManager.sensitivity);
            cameraTransform.localRotation = Quaternion.AngleAxis(pitch * GameManager.sensitivity, Vector3.right);
        }
    }

    private void HandleAction(InputAction.CallbackContext obj)
    {
        float keyPressed = obj.ReadValue<float>();
        if (keyPressed == 1 && crosshairRed.activeSelf == true && !pauseManager.paused && !pauseManager.freeze)
        {
            interactor.Action(hit.collider.gameObject);
            if (interactor.handOccupied)
            {
                if (!positionSaved)
                {
                    parentOriginal = interactor.hand.transform.parent;
                    positionOriginal = interactor.hand.transform.localPosition;
                    rotationOriginal = interactor.hand.transform.rotation;
                    positionSaved = true;
                    interactor.hand.transform.parent = hand.transform;
                    interactor.hand.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
            else
            {
                if (positionSaved)
                {
                    if (!interactor.movable)
                    {
                        interactor.hand.transform.parent = parentOriginal;
                        interactor.hand.transform.localPosition = positionOriginal;
                        if (interactor.destroyable)
                        {
                            Destroy(interactor.hand);
                            interactor.destroyable = false;
                        }
                    }
                    else
                    {
                        interactor.movable = false;
                    }
                    interactor.hand.transform.rotation = rotationOriginal;
                    positionSaved = false;
                    interactor.hand = null;
                }
            }
        }
        //keyPressed = 0;
    }

    private void HandlePause(InputAction.CallbackContext obj)
    {
        float keyPressed = obj.ReadValue<float>();
        if(keyPressed == 1 && pauseManager.paused == pauseCanvas.activeSelf)
        {
            pauseManager.paused = !pauseManager.paused;
            if (pauseManager.paused)
            {
                UI.StopAllCoroutines();
                UI.StartCoroutine(UI.StopTransition());
                Cursor.lockState = CursorLockMode.None;
                pauseCanvas.SetActive(true);
                //gameCanvas.SetActive(false);
                gameCanvas.transform.Find("Panel").gameObject.SetActive(false);
            }
            else
            {
                pauseManager.Riprendi();
                pauseManager.ResetPanel();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseManager.paused && !pauseManager.freeze)
        {
            if (Physics.Raycast(cam.position, cam.forward, out hit, distanceView))
            {
                if (!hit.collider.CompareTag("Untagged"))
                {
                    if (hit.collider.CompareTag("Ingredient") || hit.collider.CompareTag("Grabbable") || hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Kitchen") || hit.collider.CompareTag("Animated"))
                    {
                        crosshairRed.GetComponentInChildren<TextMeshProUGUI>().text = hit.collider.gameObject.name;
                    }
                    else
                    {
                        crosshairRed.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    }
                    crosshairRed.SetActive(true);
                }
                else
                {   
                    crosshairRed.SetActive(false);
                }
            }
            else
            {
                crosshairRed.SetActive(false);
            }
        }
    }
}
