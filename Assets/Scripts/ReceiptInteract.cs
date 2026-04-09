using UnityEngine;
using StarterAssets;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReceiptInteract : Interactable
{
    [Header("Conversation References")]
    public ConversationManager conversationManager;
    public DialogueLine[] preNPCDialogue;    // Played if you haven't talked to the NPC yet
    public DialogueLine[] postNPCDialogue;   // Played the first time you look at the receipt after talking to NPC
    public DialogueLine[] mathErrorSubtotal; // Played if the player gets the math wrong
    public DialogueLine[] mathErrorTax;      
    public DialogueLine[] solvedDialogue;    // Played when interacting with the receipt after it's finished

    [Header("UI & Input")]
    public GameObject receiptCanvas;      // The actual Puzzle UI
    public GameObject interactButton;    // The [F] prompt
    public StarterAssetsInputs playerInputs;
    public MonoBehaviour playerController; // To disable player movement logic
    public Transform playerCamera;

    [Header("Facing Settings")]
    [Range(0f, 1f)] public float facingThreshold = 0.8f;

    // GLOBAL STATE: Shared with other scripts to know the puzzle is finished
    public static bool isReceiptSolved = false;
    
    // LOCAL STATES
    private bool isInteracting = false;      // True ONLY when the Puzzle UI is open
    private bool hasSeenIntroDialogue = false; // Tracks if the player has read the "receipt intro"
    private bool waitingForPuzzleOpen = false; // Logic bridge to open UI after dialogue ends

    #region Editor Helper
    // This part ensures that if you leave the receipt UI open in the Unity Editor,
    // it automatically hides itself when you're not playing so it doesn't block your view.
    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += DisableCanvasesInEditor;
#endif
    }

    private void DisableCanvasesInEditor()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall -= DisableCanvasesInEditor;
        if (this == null || Application.isPlaying) return;
        if (receiptCanvas != null) receiptCanvas.SetActive(false);
#endif
    }
    #endregion

    void Start()
    {
        isReceiptSolved = false;
        if (playerCamera == null && Camera.main != null) playerCamera = Camera.main.transform;
        
        // Ensure everything is reset to a clean gameplay state
        ForceEnd(); 
    }

    protected override void Update()
    {
        base.Update();

        // Check if the dialogue system is currently active
        bool isTalking = conversationManager != null && conversationManager.IsTalking();

        // TRANSITION LOGIC:
        // If we were waiting for the intro dialogue to finish, open the puzzle now.
        if (waitingForPuzzleOpen && !isTalking)
        {
            waitingForPuzzleOpen = false;
            OpenReceiptPuzzle();
        }

        // PROMPT LOGIC:
        // Show the [F] prompt if the player is close, not already in the UI, and not talking.
        if (playerDetected && !isInteracting && !isTalking)
        {
            ShowPrompt(IsPlayerFacing());
        }
        else
        {
            ShowPrompt(false);
        }

        // INPUT FREEZING:
        if ((isInteracting || isTalking) && playerInputs != null)
        {
            // Always stop the player from walking away during any interaction
            playerInputs.move = Vector2.zero;
            
            // CAMERA LOCK LOGIC:
            // We only set 'look' to zero if the PUZZLE UI is open (isInteracting).
            // If it's just a monologue (isTalking), we don't zero it, leaving the camera FREE.
            if (isInteracting)
            {
                playerInputs.look = Vector2.zero;
            }

            playerInputs.jump = false;
            playerInputs.sprint = false;
        }

        // DIALOGUE ADVANCEMENT:
        // If the Puzzle UI is open but an error message (dialogue) is appearing over it,
        // allow the [F] key to advance that text.
        if (isInteracting && isTalking && Input.GetKeyDown(KeyCode.F))
        {
            conversationManager.OnInteractPressed();
        }
    }

    private void LateUpdate()
    {
        // Force the prompt button off if a conversation starts to avoid visual overlapping
        if (conversationManager != null && conversationManager.IsTalking())
        {
            if (interactButton != null && interactButton.activeSelf) interactButton.SetActive(false);
        }
    }

    public override void ShowPrompt(bool show)
    {
        if (interactButton != null) interactButton.SetActive(show);
    }

    public override void OnInteract()
    {
        // If dialogue is playing, 'F' acts as the "Next" button
        if (conversationManager != null && conversationManager.IsTalking())
        {
            conversationManager.OnInteractPressed();
            return;
        }

        // Ignore if already in the puzzle or looking away
        if (isInteracting || !IsPlayerFacing()) return;

        // STATE 1: Already Solved
        if (isReceiptSolved)
        {
            conversationManager.StartManualConversation(solvedDialogue);
            return;
        }

        // STATE 2: Haven't talked to NPC yet
        if (!NPCInteract.hasFinishedFirstTalk)
        {
            conversationManager.StartManualConversation(preNPCDialogue);
            return;
        }

        // STATE 3: First time opening receipt after talking to NPC
        if (!hasSeenIntroDialogue)
        {
            hasSeenIntroDialogue = true;
            waitingForPuzzleOpen = true; // Signals Update() to open puzzle once this dialogue is done
            conversationManager.StartManualConversation(postNPCDialogue); 
            return;
        }

        // STATE 4: Normal Puzzle Opening
        OpenReceiptPuzzle();
    }

    public void OpenReceiptPuzzle()
    {
        isInteracting = true;
        
        // Stop the player controller script
        if (playerController != null) playerController.enabled = false;
        
        // FREE THE MOUSE: Allow the player to click on input fields
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (playerInputs != null) playerInputs.SetCursorLocked(false);
        
        // Show the Canvas
        if (receiptCanvas != null) receiptCanvas.SetActive(true);
    }

    public void TriggerMathError(bool isSubtotalError)
    {
        // Plays dialogue without closing the puzzle UI
        DialogueLine[] selectedError = isSubtotalError ? mathErrorSubtotal : mathErrorTax;
        conversationManager.StartManualConversation(selectedError);
    }

    private bool IsPlayerFacing()
    {
        if (playerCamera == null) return false;
        Vector3 dir = (transform.position - playerCamera.position).normalized;
        float dot = Vector3.Dot(playerCamera.forward, dir);
        return dot > facingThreshold;
    }

    public override void ForceEnd()
    {
        // RE-LOCK THE MOUSE: Return control to the camera
        isInteracting = false;
        waitingForPuzzleOpen = false;
        if (playerController != null) playerController.enabled = true;
        if (receiptCanvas != null) receiptCanvas.SetActive(false);
        if (playerInputs != null) playerInputs.SetCursorLocked(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CloseInteraction(bool solvedCorrectly)
    {
        // Called by the "Submit" button on your receipt UI
        if (solvedCorrectly) isReceiptSolved = true; 
        ForceEnd();
    }
}