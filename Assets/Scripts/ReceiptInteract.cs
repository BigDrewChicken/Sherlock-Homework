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
    public DialogueLine[] preNPCDialogue;
    public DialogueLine[] postNPCDialogue;
    public DialogueLine[] mathErrorSubtotal;
    public DialogueLine[] mathErrorTax;
    public DialogueLine[] solvedDialogue;

    [Header("UI & Input")]
    public GameObject receiptCanvas;
    public GameObject interactButton; 
    public TextMeshProUGUI interactButtonText; // Drag the Label here
    public string customPromptText = "[F] Interact"; // Set this in Inspector

    public StarterAssetsInputs playerInputs;
    public MonoBehaviour playerController;
    public Transform playerCamera;

    [Header("Facing Settings")]
    [Range(0f, 1f)] public float facingThreshold = 0.8f;

    // GLOBAL STATE
    public static bool isReceiptSolved = false;
    
    // STATIC TRACKING
    private static GameObject currentActivePromptOwner;

    // LOCAL STATES
    private bool isInteracting = false;
    private bool hasSeenIntroDialogue = false;
    private bool waitingForPuzzleOpen = false;

    // ... (Keep OnValidate and DisableCanvasesInEditor from previous version)

    void Start()
    {
        isReceiptSolved = false;
        if (playerCamera == null && Camera.main != null) playerCamera = Camera.main.transform;
        ForceEnd(); 
    }

    protected override void Update()
    {
        base.Update();

        bool isTalking = conversationManager != null && conversationManager.IsTalking();

        if (waitingForPuzzleOpen && !isTalking)
        {
            waitingForPuzzleOpen = false;
            OpenReceiptPuzzle();
        }

        // PROMPT LOGIC
        if (playerDetected && !isInteracting && !isTalking && IsPlayerFacing())
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

        // Input freezing logic
        if ((isInteracting || isTalking) && playerInputs != null)
        {
            playerInputs.move = Vector2.zero;
            if (isInteracting) playerInputs.look = Vector2.zero;
            playerInputs.jump = false;
            playerInputs.sprint = false;
        }

        if (isInteracting && isTalking && Input.GetKeyDown(KeyCode.F))
        {
            conversationManager.OnInteractPressed();
        }
    }

    private void LateUpdate()
    {
        if (conversationManager != null && conversationManager.IsTalking())
        {
            if (currentActivePromptOwner == gameObject) ShowPrompt(false);
        }
    }

    public override void ShowPrompt(bool show)
    {
        if (interactButton == null) return;

        if (show)
        {
            // Don't show if the NPC already has the "stick"
            if (currentActivePromptOwner != null && currentActivePromptOwner != gameObject) return;

            // FORCE the text to "[F] Interact"
            if (interactButtonText != null) 
            {
                interactButtonText.text = customPromptText;
            }

            interactButton.SetActive(true);
            currentActivePromptOwner = gameObject;
        }
        else
        {
            // Only hide if we were the ones showing it
            if (currentActivePromptOwner == gameObject)
            {
                interactButton.SetActive(false);
                currentActivePromptOwner = null;
            }
        }
    }

    // ... (Rest of the script: OnInteract, OpenReceiptPuzzle, TriggerMathError, IsPlayerFacing, ForceEnd, CloseInteraction)
    // Ensure you keep the existing logic for those methods from your previous script
    
    public override void OnInteract()
    {
        if (conversationManager != null && conversationManager.IsTalking())
        {
            conversationManager.OnInteractPressed();
            return;
        }

        if (isInteracting || !IsPlayerFacing()) return;

        if (isReceiptSolved)
        {
            conversationManager.StartManualConversation(solvedDialogue);
            return;
        }

        if (!NPCInteract.hasFinishedFirstTalk)
        {
            conversationManager.StartManualConversation(preNPCDialogue);
            return;
        }

        if (!hasSeenIntroDialogue)
        {
            hasSeenIntroDialogue = true;
            waitingForPuzzleOpen = true; 
            conversationManager.StartManualConversation(postNPCDialogue); 
            return;
        }

        OpenReceiptPuzzle();
    }

    public void OpenReceiptPuzzle()
    {
        isInteracting = true;
        if (playerController != null) playerController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (playerInputs != null) playerInputs.SetCursorLocked(false);
        if (receiptCanvas != null) receiptCanvas.SetActive(true);
        ShowPrompt(false);
    }

    public void TriggerMathError(bool isSubtotalError)
    {
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
        isInteracting = false;
        waitingForPuzzleOpen = false;
        if (playerController != null) playerController.enabled = true;
        if (receiptCanvas != null) receiptCanvas.SetActive(false);
        if (playerInputs != null) playerInputs.SetCursorLocked(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ShowPrompt(false);
    }

    public void CloseInteraction(bool solvedCorrectly)
    {
        if (solvedCorrectly) isReceiptSolved = true; 
        ForceEnd();
    }
}