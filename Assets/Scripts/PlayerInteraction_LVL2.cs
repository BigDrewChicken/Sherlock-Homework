using UnityEngine;
using VIDE_Data;

public class PlayerInteraction_LVL2 : MonoBehaviour
{
    public float interactDistance = 4f;
    public GameObject interactPromptUI;
    public ConversationManager_LVL2 diagManager;
    public GameProgression_LVL2 progression;

    void Update()
    {
        if (diagManager.IsTalking)
        {
            interactPromptUI.SetActive(false);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance))
        {
            DialogueInteract_LVL2 script = hit.collider.GetComponent<DialogueInteract_LVL2>();

            if (script != null)
            {
                interactPromptUI.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    HandleLogic(script);
                }
                return;
            }
        }
        interactPromptUI.SetActive(false);
    }

    void HandleLogic(DialogueInteract_LVL2 script)
    {
        // FINAL BLOW: No more if/else checks for names or progression flags.
        // This bypasses the "Objective Locked" logic entirely.
        if (script.assignedDialogue != null)
        {
            diagManager.StartConversation(script.assignedDialogue);
        }
        else
        {
            // If it fails now, it's because the NPC is missing the VIDE_Assign component.
            Debug.LogError("Error: " + script.npcName + " is missing its assignedDialogue!");
        }
    }
}