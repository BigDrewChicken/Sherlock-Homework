using UnityEngine;

public class InteractableBeaker : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject scienceQuestionPanel; // Drag your ScienceQuestionPanel here
    public GameObject interactPrompt;       // Optional: A "Press F to Investigate" text

    private bool isPlayerNearby = false;

    void Start()
    {
       
        if (scienceQuestionPanel != null)
            scienceQuestionPanel.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            scienceQuestionPanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


            if (interactPrompt != null)
                interactPrompt.SetActive(false);

        
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true); // Show "Press F"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);

            // Hide the question if the player walks away
            if (scienceQuestionPanel != null)
                scienceQuestionPanel.SetActive(false);
        }
    }
}