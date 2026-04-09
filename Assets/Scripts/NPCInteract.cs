using UnityEngine;
using TMPro;

public class NPCInteract : Interactable
{
    [Header("Dialogue Reference")]
    public ConversationManager conversationManager; // The single manager that handles both sets

    [Header("UI Elements")]
    public GameObject interactButtonObject; 
    public TextMeshProUGUI interactButtonText; 

    public static bool hasFinishedFirstTalk = false;

    void Start()
    {
        // Start with the button disabled
        if (interactButtonObject != null)
        {
            interactButtonObject.SetActive(false);
        }
    }

    public override void OnInteract()
    {
        // Simply tell the manager to handle the interaction
        if (conversationManager != null)
        {
            conversationManager.OnInteractPressed();
        }
        
        // Hide button immediately when interaction starts
        ShowPrompt(false);
    }

    public override void ShowPrompt(bool show)
    {
        if (interactButtonObject != null)
        {
            if (show)
            {
                UpdateLabel(); 
                interactButtonObject.SetActive(true);
            }
            else
            {
                interactButtonObject.SetActive(false);
            }
        }
    }

    private void UpdateLabel()
    {
        if (interactButtonText == null) return;

        // The NPC script still controls the button label based on global puzzle state
        if (ReceiptInteract.isReceiptSolved)
        {
            interactButtonText.text = "[F] Talk";
        }
        else
        {
            interactButtonText.text = "[F] Listen";
        }
    }

    public override void ForceEnd()
    {
        if (conversationManager != null) conversationManager.ForceEnd();
        ShowPrompt(false);
    }

    protected override void Update()
    {
        base.Update(); 

        if (conversationManager == null) return;

        // 1. Check if the player is in range AND the NPC is NOT currently talking
        if (playerDetected && !conversationManager.IsTalking())
        {
            // Update the global flag for the receipt's logic gate
            if (!hasFinishedFirstTalk && !ReceiptInteract.isReceiptSolved)
            {
                hasFinishedFirstTalk = true; 
            }
            
            // Only show the prompt if it's not already visible
            if (!interactButtonObject.activeSelf)
            {
                ShowPrompt(true);
            }
        }
        // 2. If the player walks away OR the NPC starts talking, hide the prompt
        else 
        {
            if (interactButtonObject != null && interactButtonObject.activeSelf)
            {
                ShowPrompt(false);
            }
        }
    }
}