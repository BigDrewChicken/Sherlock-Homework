using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using StarterAssets;

// This class defines what a single "page" of dialogue looks like in the Inspector.
[System.Serializable]
public class DialogueLine
{
    public string speakerName; 
    [TextArea(3, 10)] // Makes the text box bigger in the Unity Inspector.
    public string text;
    public AudioClip voiceLine; // Audio file to play for this specific line.
}

public class ConversationManager : MonoBehaviour
{
    [Header("Dialogue Sets")]
    // Different arrays of dialogue for different game states.
    public DialogueLine[] initialConversation;
    public DialogueLine[] solvedConversation;
    public DialogueLine[] hintConversation;

    [Header("UI References")]
    public Button interactButton; 
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;      
    public TextMeshProUGUI dialogueText;

    [Header("Audio Settings")]
    public AudioSource audioSource; 

    [Header("Input & Movement")]
    public StarterAssetsInputs playerInputs; // Reference to stop player movement during talk.
    public float inputCooldown = 0.5f; // Prevents accidental double-clicking.
    
    [Header("Typing Settings")]
    public float typingSpeed = 0.04f; // Time delay between each letter appearing.

    [Header("External Script Reference")]
    public ReceiptBehavior receiptBehavior; // Used to pull dynamic numbers (like prices) into text.

    // Private state tracking
    private DialogueLine[] activeConversation; 
    private bool isTalking = false;
    private int index = 0; // Which line of the current conversation are we on?
    private float nextPressTime = 0f;
    
    private Coroutine typingCoroutine;
    private bool isCurrentlyTyping = false;
    private string currentProcessedText; 

    void Start()
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
        
        // Auto-grab AudioSource if you forgot to drag it in the inspector.
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // If talking, force the player's move input to zero so they can't walk away.
        if (isTalking && playerInputs != null)
        {
            playerInputs.move = Vector2.zero;
        }
    }

    // Triggered by the UI Button or Interacting.
    public void OnInteractPressed()
    {
        if (Time.time < nextPressTime) return;

        if (!isTalking) 
        {
            // Pick the conversation based on whether the receipt puzzle is finished.
            activeConversation = ReceiptInteract.isReceiptSolved ? solvedConversation : initialConversation;
            StartCoroutine(AnimateButtonAndStart());
        }
        else 
        {
            Advance(); // Go to next line.
        }

        nextPressTime = Time.time + inputCooldown;
    }

    // Specific trigger for a "Hint" button if you have one.
    public void OnHintPressed()
    {
        if (Time.time < nextPressTime) return;

        if (!isTalking) 
        {
            activeConversation = hintConversation;
            StartCoroutine(AnimateButtonAndStart());
        }
        else 
        {
            Advance();
        }

        nextPressTime = Time.time + inputCooldown;
    }

    // Gives visual feedback to the button before starting.
    IEnumerator AnimateButtonAndStart()
    {
        if (interactButton != null)
        {
            interactButton.targetGraphic.color = interactButton.colors.pressedColor;
            yield return new WaitForSeconds(0.1f);
            interactButton.targetGraphic.color = interactButton.colors.normalColor;
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

    // Handles the setup for the CURRENT line of dialogue.
    void UpdateStep()
    {
        if (activeConversation != null && index < activeConversation.Length)
        {
            // 1. Set Name UI
            if (nameText != null) nameText.text = activeConversation[index].speakerName;
            
            // 2. Play Audio
            if (audioSource != null)
            {
                audioSource.Stop();
                if (activeConversation[index].voiceLine != null)
                {
                    audioSource.clip = activeConversation[index].voiceLine;
                    audioSource.Play();
                }
            }

            // 3. Process Dynamic Text (The "Replace" Logic)
            // This searches for tags like {total} in your string and replaces them with real numbers.
            string rawText = activeConversation[index].text;
            currentProcessedText = rawText;
            
            if (receiptBehavior != null)
            {
                currentProcessedText = currentProcessedText
                    .Replace("{total}", receiptBehavior.GetTargetSum().ToString("F0"))
                    .Replace("{totalTax}", receiptBehavior.GetTargetTotalWithTax().ToString("F2"))
                    .Replace("{amountGiven}", receiptBehavior.GetAmountGiven().ToString("F2"))
                    .Replace("{change}", receiptBehavior.GetCorrectChange().ToString("F2"))
                    .Replace("{wrongChange}", receiptBehavior.GetWrongChange().ToString("F2"));
            }

            // 4. Start the "Typewriter" effect.
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(currentProcessedText));
        }
    }

    // Coroutine that types text out letter-by-letter.
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

    // Logic for moving to the next line or closing the box.
    void Advance()
    {
        // IF we are still typing, "Advance" will just finish the sentence instantly.
        if (isCurrentlyTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = currentProcessedText; 
            isCurrentlyTyping = false;
            return; 
        }

        // IF the sentence was already done, move to the next line in the array.
        index++;
        
        if (activeConversation != null && index < activeConversation.Length) 
        {
            UpdateStep();
        }
        else 
        {
            End(); // End of the array reached.
        }
    }

    void End()
    {
        isTalking = false;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isCurrentlyTyping = false;

        if (audioSource != null) audioSource.Stop();
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }

    // Call this if the player walks away mid-sentence.
    public void ForceEnd()
    {
        if (isTalking) End();
    }

    public bool IsTalking() => isTalking;

    // Allows other scripts to "Force" a specific conversation to start.
    public void StartManualConversation(DialogueLine[] customLines)
    {
        if (isTalking || customLines == null || customLines.Length == 0) return;
        activeConversation = customLines;
        Begin();
    }
}