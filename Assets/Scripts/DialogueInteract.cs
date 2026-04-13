using UnityEngine;
using VIDE_Data;

public class DialogueInteract : MonoBehaviour
{
    [Header("References")]
    public ConversationManager manager;
    public GameObject promptUI;

    private VIDE_Assign myNPC;
    private bool playerInRange = false;

    void Start()
    {
        // Gets VIDE_Assign from the parent (The Dictionary or NPC)
        myNPC = GetComponentInParent<VIDE_Assign>();

        if (manager == null)
            manager = FindFirstObjectByType<ConversationManager>();

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (manager == null || myNPC == null) return;

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!manager.IsTalking)
            {
                // Logic to start the conversation
                manager.StartConversation(myNPC);
                if (promptUI != null) promptUI.SetActive(false);
            }
            else
            {
                // Logic to advance the story to the next node
                manager.HandleInteraction();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptUI != null && manager != null && !manager.IsTalking)
            {
                promptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptUI != null) promptUI.SetActive(false);

            if (manager != null && manager.IsTalking)
            {
                manager.ForceEnd();
            }
        }
    }
}