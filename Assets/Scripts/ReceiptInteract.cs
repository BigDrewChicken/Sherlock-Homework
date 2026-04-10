using UnityEngine;
using StarterAssets;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReceiptInteract : Interactable
{
    public GameObject receiptCanvas;      
    public GameObject hintCanvas;         
    public GameObject interactButton;
    public TextMeshProUGUI hintText; 
    
    [Header("References")]
    public StarterAssetsInputs playerInputs;
    public MonoBehaviour playerController;
    public GameObject narrativeDialogueCanvas; 

    [Header("Error Settings")]
    public GameObject errorCanvas; 
    public TextMeshProUGUI errorText; 

    public static bool isReceiptSolved = false; 

    private bool isInteracting = false;
    private bool isSolved = false;
    private bool hasSeenNarrative = false;
    private bool isShowingError = false; 

    private void OnValidate()
    {
#if UNITY_EDITOR
        // This schedules the deactivation to happen AFTER Unity is done validating
        EditorApplication.delayCall += DisableCanvasesInEditor;
#endif
    }

    private void DisableCanvasesInEditor()
    {
#if UNITY_EDITOR
        // Unsubscribe immediately so it only runs once per change
        EditorApplication.delayCall -= DisableCanvasesInEditor;

        if (this == null || Application.isPlaying) return;

        if (receiptCanvas != null) receiptCanvas.SetActive(false);
        if (hintCanvas != null) hintCanvas.SetActive(false);
        if (narrativeDialogueCanvas != null) narrativeDialogueCanvas.SetActive(false);
        if (errorCanvas != null) errorCanvas.SetActive(false);
#endif
    }

    void Start()
    {
        isReceiptSolved = false;
        ForceEnd(); // Clean state at start
    }

    // ... (Keep the rest of your Update, OnInteract, etc., exactly as they were)
    
    protected override void Update()
    {
        if (isShowingError && Input.GetKeyDown(KeyCode.F))
        {
            CloseErrorHint();
        }

        base.Update();

        if (isInteracting && playerInputs != null)
        {
            playerInputs.move = Vector2.zero;
            playerInputs.look = Vector2.zero;
            playerInputs.jump = false;
            playerInputs.sprint = false;
        }
    }

    public override void OnInteract()
    {
        if (isSolved || isShowingError) return;
        ShowPrompt(false);

        if (!NPCInteract.hasFinishedFirstTalk)
        {
            ShowWorldHint("(Hmm, food and receipt unattended.. did something happen? Best I check around to see what happened.)");
            return;
        }

        if (!hasSeenNarrative)
        {
            OpenNarrative();
            return;
        }

        EnterReceiptPuzzle();
    }

    public void ShowMathError(string message)
    {
        if (errorCanvas != null)
        {
            isShowingError = true;
            if (errorText != null) errorText.text = message;
            errorCanvas.SetActive(true);
        }
    }

    private void CloseErrorHint()
    {
        isShowingError = false;
        if (errorCanvas != null) errorCanvas.SetActive(false);
    }

    private void OpenNarrative()
    {
        isInteracting = true;
        if (playerController != null) playerController.enabled = false;
        if (playerInputs != null) playerInputs.SetCursorLocked(false);
        if (narrativeDialogueCanvas != null) narrativeDialogueCanvas.SetActive(true);
        hasSeenNarrative = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void EnterReceiptPuzzle()
    {
        if (narrativeDialogueCanvas != null) narrativeDialogueCanvas.SetActive(false);
        isInteracting = true;
        if (playerController != null) playerController.enabled = false;
        if (playerInputs != null) playerInputs.SetCursorLocked(false);
        if (receiptCanvas != null) receiptCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowWorldHint(string message)
    {
        if (hintCanvas != null)
        {
            if (hintText != null) hintText.text = message;
            hintCanvas.SetActive(true);
            CancelInvoke("HideHint");
            Invoke("HideHint", 3f);
        }
    }

    private void HideHint() { if (hintCanvas != null) hintCanvas.SetActive(false); }

    public override void ShowPrompt(bool show)
    {
        if (isSolved || isShowingError) return;
        if (interactButton != null) interactButton.SetActive(show);
    }

    public override void ForceEnd()
    {
        isInteracting = false;
        isShowingError = false;
        if (playerController != null) playerController.enabled = true;
        if (receiptCanvas != null) receiptCanvas.SetActive(false);
        if (hintCanvas != null) hintCanvas.SetActive(false);
        if (narrativeDialogueCanvas != null) narrativeDialogueCanvas.SetActive(false);
        if (errorCanvas != null) errorCanvas.SetActive(false);
        if (playerInputs != null) playerInputs.SetCursorLocked(true);
    }

    public void CloseInteraction(bool solvedCorrectly)
    {
        if (solvedCorrectly) 
        {
            isSolved = true;
            isReceiptSolved = true; 
        }
        ForceEnd();
    }
}