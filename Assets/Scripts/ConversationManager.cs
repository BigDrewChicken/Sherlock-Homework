using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string text;
}

public class ConversationManager : MonoBehaviour
{
    public Button interactButton;
    public GameObject dialogueCanvas;
    public TextMeshProUGUI dialogueText;
    public DialogueLine[] conversation;

    private bool isTalking = false;
    private int index = 0;

    void Start()
    {
        if (interactButton != null) interactButton.gameObject.SetActive(false);
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }

    public void ShowPrompt(bool show)
    {
        if (!isTalking && interactButton != null) 
            interactButton.gameObject.SetActive(show);
    }

    public void OnInteractPressed()
    {
        if (!isTalking) StartCoroutine(AnimateButtonAndStart());
        else Advance();
    }

    IEnumerator AnimateButtonAndStart()
    {
        if (interactButton != null)
        {
            interactButton.targetGraphic.color = interactButton.colors.pressedColor;
            yield return new WaitForSeconds(0.1f);
            interactButton.targetGraphic.color = interactButton.colors.normalColor;
            interactButton.gameObject.SetActive(false);
        }
        Begin();
    }

    void Begin()
    {
        isTalking = true;
        index = 0;
        if (dialogueCanvas != null) dialogueCanvas.SetActive(true);
        UpdateStep();
    }

    void UpdateStep()
    {
        if (index < conversation.Length)
        {
            if (dialogueText != null) dialogueText.text = conversation[index].text;
        }
    }

    void Advance()
    {
        index++;
        if (index < conversation.Length) UpdateStep();
        else End();
    }

    void End()
    {
        isTalking = false;
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }

    public void ForceEnd()
    {
        if (isTalking) End();
    }

    public bool IsTalking()
    {
        return isTalking;
    }
}