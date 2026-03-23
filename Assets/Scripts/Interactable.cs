using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    [Header("Interaction UI")]
    public GameObject interactUI;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera interactionCamera;

    [Header("Player")]
    public GameObject player;

    [Header("Dialog")]
    public GameObject dialogPanel;
    [TextArea(3, 5)] public string dialogMessage = "Hello!";
    public GameObject exitButton; // 👈 DRAG YOUR EXIT BUTTON HERE!

    [Header("Trigger")]
    public bool useTrigger = true;
    public LayerMask playerLayerMask = 1 << 6;

    [Header("Cursor")]
    public bool lockCursorOnInteract = true;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.F;
    public KeyCode exitKey = KeyCode.Escape;

    private bool playerInRange = false;
    private bool isInteracting = false;
    private TextMeshProUGUI dialogText;
    private bool wasCursorLocked = false;

    void Awake()
    {
        if (dialogPanel != null)
            dialogText = dialogPanel.GetComponentInChildren<TextMeshProUGUI>();
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (useTrigger && col != null)
            col.isTrigger = true;

        SetUIState(false);
        SetDialogState(false);
        SetInteractionCamera(false);
    }

    void Update()
    {
        if (playerInRange && !isInteracting && Input.GetKeyDown(interactKey))
        {
            StartInteraction();
        }

        if (isInteracting && Input.GetKeyDown(exitKey))
        {
            StopInteraction();
        }
    }

    void StartInteraction()
    {
        Debug.Log($"Starting interaction with {gameObject.name} (Press {exitKey} or click button to exit)");
        isInteracting = true;

        wasCursorLocked = Cursor.lockState == CursorLockMode.Locked;
        if (lockCursorOnInteract)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        SwitchToInteractionCamera();
        SetUIState(false);
        SetDialogState(true); // 👈 This now shows dialog + exit button!
        
        if (dialogText != null)
            dialogText.text = dialogMessage;
    }

    public void StopInteraction()
    {
        Debug.Log($"Stopping interaction with {gameObject.name}");
        isInteracting = false;

        if (lockCursorOnInteract)
        {
            Cursor.lockState = wasCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !wasCursorLocked;
        }

        SwitchToMainCamera();
        SetDialogState(false); // 👈 This hides dialog + exit button!
        SetUIState(playerInRange);
    }

    // 💎 UI Button support (auto-connects if you drag button!)
    public void ExitInteractionButton()
    {
        StopInteraction();
    }

    private void SwitchToInteractionCamera()
    {
        if (mainCamera != null) mainCamera.gameObject.SetActive(false);
        if (interactionCamera != null) interactionCamera.gameObject.SetActive(true);
    }

    private void SwitchToMainCamera()
    {
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (interactionCamera != null) interactionCamera.gameObject.SetActive(false);
    }

    private void SetUIState(bool active)
    {
        if (interactUI != null) interactUI.SetActive(active);
    }

    private void SetDialogState(bool active)
    {
        if (dialogPanel != null) dialogPanel.SetActive(active);
        if (exitButton != null) exitButton.SetActive(active); // 👈 AUTO SHOW/HIDE!
    }

    private void SetInteractionCamera(bool active)
    {
        if (interactionCamera != null) interactionCamera.gameObject.SetActive(active);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger && IsPlayer(other.gameObject))
        {
            Debug.Log($"Player entered trigger of {gameObject.name} (Press {interactKey} to interact)");
            playerInRange = true;
            if (!isInteracting && interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (useTrigger && IsPlayer(other.gameObject))
        {
            Debug.Log($"Player exited trigger of {gameObject.name}");
            playerInRange = false;
            if (!isInteracting && interactUI != null)
                interactUI.SetActive(false);
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        if (obj.CompareTag("Player")) return true;
        if (((1 << obj.layer) & playerLayerMask) != 0) return true;
        if (player != null && obj == player) return true;
        return false;
    }

    public void TriggerInteraction()
    {
        if (playerInRange && !isInteracting) StartInteraction();
    }

    public bool IsPlayerInRange => playerInRange;

    private void OnDrawGizmosSelected()
    {
        if (useTrigger)
        {
            Gizmos.color = playerInRange ? Color.green : Color.yellow;
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}