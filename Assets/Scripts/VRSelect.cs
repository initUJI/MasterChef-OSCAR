using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRSelect : MonoBehaviour
{
    //Transform parentOriginal;
    //Vector3 positionOriginal;
    //Quaternion rotationOriginal;

    public void SelectEntered(SelectEnterEventArgs select)
    {
        if (VRManager.timeout)
        {
            return;
        }
        else if (!VRManager.pauseManager.paused && !VRManager.pauseManager.freeze)
        {
            VRManager.interactor.Action(select.interactableObject.transform.gameObject);
            if (VRManager.interactor.handOccupied)
            {
                if (!VRManager.positionSaved)
                {
                    VRManager.parentOriginal = VRManager.interactor.hand.transform.parent;
                    VRManager.positionOriginal = VRManager.interactor.hand.transform.localPosition;
                    VRManager.rotationOriginal = VRManager.interactor.hand.transform.rotation;
                    //VRManager.scaleOriginal = VRManager.interactor.hand.transform.localScale;

                    VRManager.positionSaved = true;

                    //VRManager.interactor.hand.transform.SetParent(select.interactorObject.transform, true);
                    VRManager.interactor.hand.transform.parent = select.interactorObject.transform;
                    if (select.interactorObject.transform.gameObject.name.Contains("Left"))
                    {
                        //Left
                        VRManager.interactor.hand.transform.localPosition = new Vector3(1 * 0.03f, -0.02f, 0f);
                        VRManager.interactor.hand.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
                    }
                    else
                    {
                        //Right
                        VRManager.interactor.hand.transform.localPosition = new Vector3(-1 * 0.03f, -0.02f, 0f);
                        VRManager.interactor.hand.transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
                    }
                    VRManager.interactor.hand.GetComponent<Collider>().enabled = false;
                }
            }
            else
            {
                if (VRManager.positionSaved)
                {
                    if (!VRManager.interactor.movable)
                    {
                        //VRManager.interactor.hand.transform.SetParent(VRManager.parentOriginal, true);
                        VRManager.interactor.hand.transform.parent = VRManager.parentOriginal;
                        VRManager.interactor.hand.transform.localPosition = VRManager.positionOriginal;
                        if (VRManager.interactor.destroyable)
                        {
                            Destroy(VRManager.interactor.hand);
                            VRManager.interactor.destroyable = false;
                        }
                    }
                    else
                    {
                        VRManager.interactor.movable = false;
                    }
                    VRManager.interactor.hand.transform.rotation = VRManager.rotationOriginal;
                    VRManager.interactor.hand.GetComponent<Collider>().enabled = true;
                    //VRManager.interactor.hand.transform.localScale = VRManager.scaleOriginal;
                    VRManager.positionSaved = false;
                    VRManager.interactor.hand = null;
                }
            }
            VRManager.Timeout(0.5f);
        }
    }

    public void HoverEntered(HoverEnterEventArgs hover)
    {
        VRManager.ChangeHUD(hover.interactableObject.transform.name);
    }

    public void LastHoverExited(HoverExitEventArgs hover)
    {
        VRManager.ChangeHUD("");
    }
}
