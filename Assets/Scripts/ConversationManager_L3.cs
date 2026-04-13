using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using StarterAssets;
using Cinemachine;

[System.Serializable]
public class DialogueLine_L3
{
    public string speakerName;
    [TextArea(3, 10)]
    public string text;
}

public class ConversationManager_L3 : Interactable
{
    [Header("Dialogue Sets")]
    public DialogueLine_L3[] initialConversation;
    public DialogueLine_L3[] solvedConversation;

    [Header("UI References")]
    public Button interactButton;
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject interactPromptUI; 

    [Header("Input, Movement & Facing")]
    public StarterAssetsInputs playerInputs;

    [Header("Clue System")]
    public bool isGivingClue = false;
    public DialogueLine_L3[] clueDialogueLines;

    public float inputCooldown = 0.5f;

    [Header("Typing Settings")]
    public float typingSpeed = 0.04f;

    [Header("External Script Reference")]
    public ReceiptBehavior receiptBehavior;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera cmMain;
    public CinemachineVirtualCamera cmBen;

    private DialogueLine_L3[] activeConversation;
    private bool isTalking = false;
    private int index = 0;
    private float nextPressTime = 0f;

    private Coroutine typingCoroutine;
    private bool isCurrentlyTyping = false;
    private string currentProcessedText;

    void Start()
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
        if (interactPromptUI != null) interactPromptUI.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (isTalking && playerInputs != null)
        {
            playerInputs.move = Vector2.zero;
            playerInputs.look = Vector2.zero;
        }
    }

    // --- ⭐ THIS WAS THE MISSING PIECE ---
    public void OnHintPressed()
    {
        if (isTalking || Time.time < nextPressTime) return;

        if (clueDialogueLines != null && clueDialogueLines.Length > 0)
        {
            activeConversation = clueDialogueLines;
            Begin();
            nextPressTime = Time.time + inputCooldown;
        }
    }

    public override void OnInteract()
    {
        if (Time.time < nextPressTime) return;

        if (!isTalking)
        {
            if (isGivingClue)
                activeConversation = clueDialogueLines;
            else
                activeConversation = ReceiptInteract.isReceiptSolved ? solvedConversation : initialConversation;

            Begin();
        }
        else
        {
            Advance();
        }

        nextPressTime = Time.time + inputCooldown;
    }

    public override void ShowPrompt(bool show)
    {
        if (interactPromptUI != null)
            interactPromptUI.SetActive(show);
    }

    public override void ForceEnd()
    {
        if (isTalking) End();
    }

    public void OnInteractPressed() 
    {
        OnInteract();
    }

    void Begin()
    {
        isTalking = true;
        index = 0;

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        ShowPrompt(false); 
        SwitchCamera(cmBen, cmMain);
        UpdateStep();
    }

    void UpdateStep()
    {
        if (activeConversation != null && index < activeConversation.Length)
        {
            if (nameText != null)
                nameText.text = activeConversation[index].speakerName;

            string rawText = activeConversation[index].text;
            currentProcessedText = rawText;

            if (receiptBehavior != null)
            {
                float total = receiptBehavior.GetTargetSum();
                currentProcessedText = currentProcessedText
                    .Replace("{total}", total.ToString("F0"))
                    .Replace("{totalTax}", total.ToString("F2"));
            }

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(currentProcessedText));
        }
    }

    IEnumerator TypeText(string textToType)
    {
        isCurrentlyTyping = true;
        dialogueText.text = "";

        foreach (char letter in textToType.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isCurrentlyTyping = false;
        typingCoroutine = null;
    }

    void Advance()
    {
        if (isCurrentlyTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = currentProcessedText;
            isCurrentlyTyping = false;
            return;
        }

        index++;
        if (activeConversation != null && index < activeConversation.Length)
            UpdateStep();
        else
            End();
    }

    void End()
    {
        isTalking = false;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isCurrentlyTyping = false;

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        SwitchCamera(cmMain, cmBen);
        
        if (playerDetected) ShowPrompt(true);
    }

    public void EnableClueDialogue() => isGivingClue = true;
    public void DisableClueDialogue() => isGivingClue = false;
    public bool IsTalking() => isTalking;

    private void SwitchCamera(CinemachineVirtualCamera activeCam, CinemachineVirtualCamera inactiveCam)
    {
        if (activeCam == null || inactiveCam == null) return;
        activeCam.Priority = 20;
        inactiveCam.Priority = 10;
    }
}