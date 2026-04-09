using UnityEngine;
using StarterAssets;
using TMPro;
using System.Collections; 

public class NPCInteract : Interactable
{
    [Header("Dialogue Reference")]
    public ConversationManager conversationManager; // Reference to the script that handles text boxes

    [Header("UI Elements")]
    public GameObject interactButtonObject; // The world-space prompt (e.g., the [F] popup)
    public TextMeshProUGUI interactButtonText; // The actual text component inside the prompt

    [Header("Facing Settings")]
    public Transform playerCamera; // Used to calculate if the player is looking at the NPC
    [Range(0f, 1f)]
    public float facingThreshold = 0.8f; // How directly the player must look (1.0 is perfect alignment)

    // GLOBAL STATE: Tells the Receipt script the player has talked to the NPC at least once
    public static bool hasFinishedFirstTalk = false; 
    
    // LOCAL STATE: Used to toggle between "Initial Talk" and "Hint" logic
    private bool hasShownIntro = false; 
    
    // SAFETY GATE: Prevents the prompt from flickering while variables are being updated
    private bool isBusy = false; 

    void Start()
    {
        // Auto-assign the main camera if we forgot to drag it in the inspector
        if (playerCamera == null && Camera.main != null) 
            playerCamera = Camera.main.transform;

        // Ensure the [F] prompt is hidden when the game starts
        if (interactButtonObject != null) interactButtonObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update(); // Runs the detection logic from the parent Interactable script

        if (conversationManager == null) return;

        // Get talking state once per frame for efficiency
        bool isTalking = conversationManager.IsTalking();
        
        // LOGIC: Show the prompt ONLY if:
        // 1. Player is in trigger range (playerDetected)
        // 2. We aren't currently mid-conversation (!isTalking)
        // 3. We aren't in the middle of a state transition (!isBusy)
        if (playerDetected && !isTalking && !isBusy)
        {
            // Check if player is physically looking at the NPC
            ShowPrompt(IsPlayerFacing());
        }
        else
        {
            // Immediately hide prompt if any condition fails (e.g., player walks away or starts talking)
            if (interactButtonObject.activeSelf) ShowPrompt(false);
        }
    }

    private void LateUpdate()
    {
        // SAFETY: LateUpdate runs after animations/logic.
        // If a conversation just started this frame, force the button off so it doesn't overlap dialogue UI.
        if (conversationManager != null && conversationManager.IsTalking())
        {
            if (interactButtonObject.activeSelf) interactButtonObject.SetActive(false);
        }
    }

    public override void OnInteract()
    {
        // Prevent interaction if manager is missing, busy, or player is looking away
        if (conversationManager == null || isBusy || !IsPlayerFacing()) return;

        // CHOICE LOGIC: Decide which dialogue set to play
        // If we already saw the intro AND the receipt puzzle isn't finished yet...
        if (hasShownIntro && !ReceiptInteract.isReceiptSolved)
        {
            // Play the "Hint" dialogue set
            conversationManager.OnHintPressed();
        }
        else
        {
            // Play the standard/initial dialogue set
            conversationManager.OnInteractPressed();
            
            // Start a delay before the button text changes from "Listen" to "Hint"
            // This prevents a visual "glitch" where the button text swaps while the dialogue is opening
            StartCoroutine(DelayedStateUpdate());
        }

        // Hide prompt immediately upon clicking F
        ShowPrompt(false);
    }

    private IEnumerator DelayedStateUpdate()
    {
        isBusy = true; // Stop Update() from checking facing/prompts temporarily

        // If this was the first time talking...
        if (!ReceiptInteract.isReceiptSolved && !hasShownIntro)
        {
            // Wait 1 second (allows dialogue UI to fully cover the screen/appear)
            yield return new WaitForSeconds(1.0f); 
            
            // Update flags so next interaction triggers a Hint
            hasShownIntro = true;
            hasFinishedFirstTalk = true;
        }

        isBusy = false; // Resume normal Update() checks
    }

    private bool IsPlayerFacing()
    {
        if (playerCamera == null) return false;

        // MATH: Compare the camera's forward vector with the direction toward the NPC
        Vector3 dirToNPC = (transform.position - playerCamera.position).normalized;
        float dot = Vector3.Dot(playerCamera.forward, dirToNPC);

        // If the 'dot' product is high (close to 1), the player is looking at the NPC
        return dot > facingThreshold;
    }

    public override void ShowPrompt(bool show)
    {
        if (interactButtonObject == null) return;

        // Triple-check: we should never show the prompt if we are already talking
        bool canShow = show && !isBusy && (conversationManager != null && !conversationManager.IsTalking());

        if (canShow)
        {
            UpdateLabel(); // Change text to "Listen", "Hint", or "Talk"
            interactButtonObject.SetActive(true);
        }
        else
        {
            interactButtonObject.SetActive(false);
        }
    }

    private void UpdateLabel()
    {
        if (interactButtonText == null) return;

        // Context-aware button labels:
        if (ReceiptInteract.isReceiptSolved)
        {
            interactButtonText.text = "[F] Talk"; // Post-puzzle
        }
        else if (hasShownIntro)
        {
            interactButtonText.text = "[F] Hint"; // Middle of puzzle
        }
        else
        {
            interactButtonText.text = "[F] Listen"; // First meet
        }
    }

    public override void ForceEnd()
    {
        // Emergency cleanup if the interaction is interrupted
        if (conversationManager != null) conversationManager.ForceEnd();
        isBusy = false;
        ShowPrompt(false);
    }
}