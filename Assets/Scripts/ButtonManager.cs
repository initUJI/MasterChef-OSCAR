using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{    public void Click()
    {
        AudioManager.AudioStart("Sounds/GUI/button_click");
    }
}
