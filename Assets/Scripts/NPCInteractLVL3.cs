using UnityEngine;
using StarterAssets;
using TMPro;
using System.Collections;

public class NPCInteractLVL3 : Interactable
{
    [Header("Dialogue Reference")]
    // Specifically linked to your L3 Manager
    public ConversationManager_L3 conversationManager;

    [Header("UI Elements")]
    public GameObject interactButtonObject;
    public TextMeshProUGUI interactButtonText;

    [Header("Win State UI")]
    public GameObject winCanvas;
    public GameObject dialogueUI;

    [Header("Facing Settings")]
    public Transform playerCamera;
    [Range(0f, 1f)] public float facingThreshold = 0.7f; // Set to 0.7 for better reliability

    // Flags
    public static bool hasFinishedFirstTalk = false;
    private bool hasShownIntro = false;
    private bool isBusy = false;
    private bool winTriggered = false;
    private bool didStartFinalDialogue = false;

    private StarterAssetsInputs playerInputs;
    private MonoBehaviour movementScript;

    private static GameObject currentActivePromptOwner;

    void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        playerInputs = FindFirstObjectByType<StarterAssetsInputs>();
        
        if (playerInputs != null)
        {
            movementScript = playerInputs.GetComponent("FirstPersonController") as MonoBehaviour;
            if (movementScript == null)
                movementScript = playerInputs.GetComponent("ThirdPersonController") as MonoBehaviour;
        }

        if (interactButtonObject != null) interactButtonObject.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(false);
    }

    protected override void Update()
    {
        // 1. Run the Base Update (This detects the 'F' key press)
        base.Update();

        if (conversationManager == null || winTriggered) return;

        bool isTalking = conversationManager.IsTalking();

        // 2. Win Sequence Trigger
        if (ReceiptInteract.isReceiptSolved && didStartFinalDialogue && !isTalking)
        {
            TriggerWinSequence();
            return;
        }

        // 3. Prompt Visibility Logic
        if (playerDetected && !isTalking && !isBusy && IsPlayerFacing())
        {
            ShowPrompt(true);
        }
        else if (currentActivePromptOwner == gameObject)
        {
            ShowPrompt(false);
        }
    }

    // ⭐ THIS IS CALLED BY THE BASE 'INTERACTABLE' SCRIPT
    public override void OnInteract()
    {
        if (conversationManager == null || winTriggered) return;

        // If dialogue is already open, pressing 'F' should Advance it
        if (conversationManager.IsTalking())
        {
            conversationManager.OnInteractPressed(); 
            return;
        }

        // Standard interaction checks
        if (isBusy || !IsPlayerFacing()) return;

        // Determine which dialogue set to start
        if (ReceiptInteract.isReceiptSolved)
        {
            conversationManager.OnInteractPressed();
            didStartFinalDialogue = true; 
        }
        else if (hasShownIntro)
        {
            // Now correctly calls the Clue dialogue
            conversationManager.OnHintPressed();
        }
        else
        {
            conversationManager.OnInteractPressed();
            StartCoroutine(DelayedStateUpdate());
        }

        ShowPrompt(false);
    }

    private void TriggerWinSequence()
    {
        winTriggered = true;
        if (interactButtonObject != null) interactButtonObject.SetActive(false);
        if (dialogueUI != null) dialogueUI.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerInputs != null)
        {
            playerInputs.move = Vector2.zero;
            playerInputs.look = Vector2.zero;
            playerInputs.cursorLocked = false;
            playerInputs.cursorInputForLook = false;

            if (playerInputs.TryGetComponent<CharacterController>(out CharacterController cc))
                cc.enabled = false;

            if (movementScript != null)
                movementScript.enabled = false;
        }
    }

    private IEnumerator DelayedStateUpdate()
    {
        isBusy = true;
        yield return new WaitForSeconds(0.5f);
        hasShownIntro = true;
        hasFinishedFirstTalk = true;
        isBusy = false;
    }

    private bool IsPlayerFacing()
    {
        if (playerCamera == null && Camera.main != null) playerCamera = Camera.main.transform;
        if (playerCamera == null) return false;

        Vector3 dirToNPC = (transform.position - playerCamera.position).normalized;
        float dot = Vector3.Dot(playerCamera.forward, dirToNPC);
        return dot > facingThreshold;
    }

    public override void ShowPrompt(bool show)
    {
        if (interactButtonObject == null || winTriggered) return;

        if (show)
        {
            if (currentActivePromptOwner != null && currentActivePromptOwner != gameObject) return;
            UpdateLabel();
            interactButtonObject.SetActive(true);
            currentActivePromptOwner = gameObject;
        }
        else if (currentActivePromptOwner == gameObject)
        {
            interactButtonObject.SetActive(false);
            currentActivePromptOwner = null;
        }
    }

    private void UpdateLabel()
    {
        if (interactButtonText == null) return;
        if (ReceiptInteract.isReceiptSolved) interactButtonText.text = "[F] Finish";
        else if (hasShownIntro) interactButtonText.text = "[F] Hint";
        else interactButtonText.text = "[F] Listen";
    }

    public override void ForceEnd()
    {
        if (conversationManager != null) conversationManager.ForceEnd();
        isBusy = false;
        ShowPrompt(false);
    }
}