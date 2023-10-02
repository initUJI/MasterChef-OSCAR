using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Transform textAreaObject;
    TextMeshProUGUI textArea;
    Image textBackground;
    public float transitionDuration = 0.5f;

    void Awake()
    {
        textAreaObject = this.transform.Find("Panel").Find("TextArea");
        textArea = textAreaObject.GetComponentInChildren<TextMeshProUGUI>();
        textArea.faceColor = new Color32(255, 255, 255, 0);
        textArea.outlineColor = new Color32(0, 0, 0, 0);
        textBackground = textAreaObject.GetComponent<Image>();
        textBackground.color = new Color32(255, 255, 255, 0);
        transitionDuration /= 60;
    }

    public IEnumerator TextTransition(string text,Color32 color, float wait, float textDuration = 0.5f)
    {
        yield return new WaitForSeconds(wait);
        textArea.text = text;
        for (int i = 1; i <= 64; i++)
        {
            textArea.faceColor = new Color32(color.r, color.g, color.b, (byte)(i * 4 - 1));
            textArea.outlineColor = new Color32(0, 0, 0, (byte)(i * 4 - 1));
            textBackground.color = new Color32(255, 255, 255, (byte)(i * 2 - 1));
            yield return new WaitForSeconds(transitionDuration);
        }

        yield return new WaitForSeconds(textDuration);

        for (int i = 64; i > 0; i--)
        {
            textArea.faceColor = new Color32(color.r, color.g, color.b, (byte)(i * 4 - 1));
            textArea.outlineColor = new Color32(0, 0, 0, (byte)(i * 4 - 1));
            textBackground.color = new Color32(255, 255, 255, (byte)(i * 2 - 1));
            yield return new WaitForSeconds(transitionDuration);
        }
        textArea.faceColor = new Color32(color.r, color.g, color.b, 0);
        textArea.outlineColor = new Color32(0, 0, 0, 0);
    }

    public IEnumerator StopTransition()
    {
        textArea.faceColor = new Color32(255, 255, 255, 0);
        textArea.outlineColor = new Color32(0, 0, 0, 0);
        textBackground.color = new Color32(255, 255, 255, 0);
        yield return new WaitForSeconds(0);
    }
}