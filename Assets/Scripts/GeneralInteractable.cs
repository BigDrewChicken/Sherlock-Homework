using UnityEngine;
using TMPro;
using System.Collections; // Needed for Coroutines

public class GeneralInteractable : Interactable
{
    [Header("UI Reference")]
    public GameObject interactButtonObject;
    public TextMeshProUGUI interactButtonText;

    [Header("Dialogue Content")]
    public ConversationManager conversationManager;
    public DialogueLine[] lines;

    [Header("Settings")]
    public string promptOverride = "[F] Interact";
    public bool interactOnlyOnce = false;
    
    private bool hasInteracted = false;
    private bool isDialogueActive = false; 

    private static GameObject currentActivePromptOwner;

    protected override void Update()
    {
        base.Update();

        bool isTalking = conversationManager != null && conversationManager.IsTalking();

        // ADVANCEMENT LOGIC
        // Only allow F to advance if we are actually the one talking
        if (isTalking && isDialogueActive && Input.GetKeyDown(KeyCode.F))
        {
            conversationManager.OnInteractPressed();
        }

        // Auto-reset when the manager finishes
        if (!isTalking && isDialogueActive)
        {
            isDialogueActive = false;
        }

        // PROMPT VISIBILITY
        if (playerDetected && !isTalking && !hasInteracted)
        {
            ShowPrompt(true);
        }
        else
        {
            if (currentActivePromptOwner == gameObject)
            {
                ShowPrompt(false);
            }
        }
    }

    public override void ShowPrompt(bool show)
    {
        if (interactButtonObject == null) return;

        if (show)
        {
            if (currentActivePromptOwner != null && currentActivePromptOwner != gameObject) return;
            if (interactButtonText != null) interactButtonText.text = promptOverride;
            
            interactButtonObject.SetActive(true);
            currentActivePromptOwner = gameObject;
        }
        else
        {
            if (currentActivePromptOwner == gameObject)
            {
                interactButtonObject.SetActive(false);
                currentActivePromptOwner = null;
            }
        }
    }

    public override void OnInteract()
    {
        if (conversationManager == null || conversationManager.IsTalking() || hasInteracted) return;

        // Use a Coroutine to start so the typewriter effect doesn't glitch
        StartCoroutine(StartDialogueRoutine());
    }

    private IEnumerator StartDialogueRoutine()
    {
        conversationManager.StartManualConversation(lines);
        
        // Wait until the end of the frame so the 'F' press that triggered 
        // this doesn't get counted as a "skip" press in Update
        yield return new WaitForEndOfFrame();
        
        isDialogueActive = true;

        if (interactOnlyOnce)
        {
            hasInteracted = true;
        }
    }

    public override void ForceEnd()
    {
        isDialogueActive = false;
        ShowPrompt(false);
    }
}