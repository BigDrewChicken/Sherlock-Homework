using UnityEngine;

public class InteractableBeaker : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject scienceQuestionPanel; // Drag your ScienceQuestionPanel here
    public GameObject interactPrompt;       // Optional: A "Press F to Investigate" text

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
        // FIX: Idinagdag natin ang check na '!scienceQuestionPanel.activeSelf'
        // Ibig sabihin: Bubuksan lang niya ang quiz KUNG HINDI PA ITO NAKABUKAS.
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (scienceQuestionPanel != null && !scienceQuestionPanel.activeSelf)
            {
                scienceQuestionPanel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (interactPrompt != null)
                    interactPrompt.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            // Ipapakita lang ang prompt kung HINDI nakabukas ang quiz
            if (interactPrompt != null && (scienceQuestionPanel == null || !scienceQuestionPanel.activeSelf))
            {
                interactPrompt.SetActive(true); // Show "Press F"
            }
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