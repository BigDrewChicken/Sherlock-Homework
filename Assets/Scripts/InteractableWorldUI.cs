using UnityEngine;
using TMPro;

public class InteractableWorldUI : MonoBehaviour
{
    public GameObject interactUI;        // "Press F" world-space text
    public Camera mainCamera;            // Player camera
    public Camera interactionCamera;     // Interaction camera
    public GameObject player;            // Starter Assets player

    [Header("Dialog UI")]
    public GameObject dialogPanel;       // Dialog panel
    public string dialogMessage = "Hello! ";

    private bool playerInRange = false;
    private bool isInteracting = false;

    void Start()
    {
        interactUI.SetActive(false);
        interactionCamera.gameObject.SetActive(false);
        if(dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!isInteracting)
                StartInteraction();
        }
    }

    void StartInteraction()
    {
        isInteracting = true;

        // Switch cameras
        mainCamera.gameObject.SetActive(false);
        interactionCamera.gameObject.SetActive(true);

        // Disable player movement
        player.SetActive(false);

        // Hide "Press F" UI
        interactUI.SetActive(false);

        // Show dialog
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
            TextMeshProUGUI text = dialogPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = dialogMessage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteracting)
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isInteracting)
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }
}