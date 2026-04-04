using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public float interactDistance = 3f; // Gaano kalayo bago makausap
    public KeyCode interactKey = KeyCode.F; // Yung F button na sabi ni Drewster

    void Update()
    {
        // Gagawa tayo ng invisible laser mula sa gitna ng camera
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // I-check kung may tinamaan ang laser sa loob ng interactDistance
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // I-check kung ang tinamaan ay may tag na "NPC"
            if (hit.collider.CompareTag("NPC"))
            {
                // TODO: Dito natin pwedeng ilabas yung "Press F to Talk" na UI hint mamaya

                // Kapag pinindot ang F
                if (Input.GetKeyDown(interactKey))
                {
                    // Kunin yung VIDE Assign component ni Ben
                    VIDE_Assign npcDialogue = hit.collider.GetComponent<VIDE_Assign>();

                    if (npcDialogue != null)
                    {
                        // Hahanapin nito yung DialogueManager at ipapasa si Ben
                        FindObjectOfType<DialogueManager>().StartDialogue(npcDialogue);
                    }
                }
            }
        }
    }
}