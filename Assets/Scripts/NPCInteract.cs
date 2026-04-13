using UnityEngine;
using StarterAssets;
using TMPro;
using System.Collections;

public class NPCInteract : Interactable
{
    [Header("Dialogue Reference")]
    public ConversationManager conversationManager;

    [Header("UI Elements")]
    public GameObject interactButtonObject;
    public TextMeshProUGUI interactButtonText;

    [Header("Win State UI")]
    public GameObject winCanvas;
    public GameObject dialogueUI;

    [Header("Facing Settings")]
    public Transform playerCamera;
    [Range(0f, 1f)] public float facingThreshold = 0.8f;

    // Flags
    public static bool hasFinishedFirstTalk = false;
    private bool hasShownIntro = false;
    private bool isBusy = false;
    private bool winTriggered = false;
    private bool didStartFinalDialogue = false;

    // Timer to prevent early win
    private float winSilenceTimer = 0f;
    private const float requiredSilenceTime = 0.5f; // Must be quiet for 1.5s to win

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
        base.Update();

        if (conversationManager == null || winTriggered)
        {
            if (winTriggered && playerInputs != null) playerInputs.move = Vector2.zero;
            return;
        }

        bool isTalking = conversationManager.IsTalking();

        // --- LEVEL 1 WIN CHECK (WITH SAFETY TIMER) ---
        if (ReceiptInteract.isReceiptSolved && didStartFinalDialogue)
        {
            if (!isTalking)
            {
                // Start counting silence
                winSilenceTimer += Time.deltaTime;
                
                // Only trigger win if the conversation has been closed for 1.5 seconds
                if (winSilenceTimer >= requiredSilenceTime)
                {
                    TriggerWinSequence();
                    return;
                }
            }
            else
            {
                // Reset timer if they are currently clicking through dialogue
                winSilenceTimer = 0f;
            }
        }

        // Prompt Logic
        if (playerDetected && !isTalking && !isBusy && IsPlayerFacing())
        {
            ShowPrompt(true);
        }
        else if (currentActivePromptOwner == gameObject)
        {
            ShowPrompt(false);
        }
    }

    public override void OnInteract()
    {
        if (conversationManager == null || isBusy || !IsPlayerFacing() || winTriggered) return;

        // Advance dialogue if already talking
        if (conversationManager.IsTalking())
        {
            conversationManager.OnInteractPressed();
            return;
        }

        if (ReceiptInteract.isReceiptSolved)
        {
            conversationManager.OnInteractPressed();
            // Start the sequence that leads to the win
            didStartFinalDialogue = true;
            winSilenceTimer = 0f; 
        }
        else if (hasShownIntro)
        {
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
        if (!ReceiptInteract.isReceiptSolved && !hasShownIntro)
        {
            hasShownIntro = true;
            hasFinishedFirstTalk = true;
        }
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