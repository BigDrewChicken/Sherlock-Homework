using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using StarterAssets;

[System.Serializable]
public class DialogueLine
{
    public string speakerName; 
    [TextArea(3, 10)]           
    public string text;
}

public class ConversationManager : MonoBehaviour
{
    [Header("Dialogue Sets")]
    public DialogueLine[] initialConversation;
    public DialogueLine[] solvedConversation;

    [Header("UI References")]
    public Button interactButton; 
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;     
    public TextMeshProUGUI dialogueText;

    [Header("Input & Movement")]
    public StarterAssetsInputs playerInputs;
    public float inputCooldown = 0.5f; 
    
    [Header("Typing Settings")]
    public float typingSpeed = 0.04f; 

    [Header("External Script Reference")]
    // Updated this to match your new script name
    public ReceiptBehavior receiptBehavior; 

    private DialogueLine[] activeConversation; 
    private bool isTalking = false;
    private int index = 0;
    private float nextPressTime = 0f;
    
    private Coroutine typingCoroutine;
    private bool isCurrentlyTyping = false;
    private string currentProcessedText; 

    void Start()
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }

    void Update()
    {
        if (isTalking && playerInputs != null)
        {
            playerInputs.move = Vector2.zero;
        }
    }

    public void OnInteractPressed()
    {
        if (Time.time < nextPressTime) return;

        if (!isTalking) 
        {
            activeConversation = ReceiptInteract.isReceiptSolved ? solvedConversation : initialConversation;
            StartCoroutine(AnimateButtonAndStart());
        }
        else 
        {
            Advance();
        }

        nextPressTime = Time.time + inputCooldown;
    }

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

    void UpdateStep()
    {
        if (activeConversation != null && index < activeConversation.Length)
        {
            if (nameText != null) nameText.text = activeConversation[index].speakerName;
            
            string rawText = activeConversation[index].text;
            currentProcessedText = rawText;
            
            // Reference the newly renamed script
            if (receiptBehavior != null)
            {
                float total = receiptBehavior.GetTargetSum();
                float totalTax = receiptBehavior.GetTargetTotalWithTax();

                currentProcessedText = currentProcessedText
                    .Replace("{total}", total.ToString("F0"))
                    .Replace("{totalTax}", totalTax.ToString("F2"));
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

        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }

    public void ForceEnd()
    {
        if (isTalking) End();
    }

    public bool IsTalking() => isTalking;
}