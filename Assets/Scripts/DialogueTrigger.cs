using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.F;

    [Header("UI References")]
    public GameObject interactHintUI;

    // BAGONG HARANG: Ilalagay natin dito ang Quiz Panel
    public GameObject scienceQuestionPanel;

    void Update()
    {
        // HARANG LOGIC: Kung nakabukas ang Science Quiz, bawal mag-scan o pumindot ng F para sa dialogue!
        if (scienceQuestionPanel != null && scienceQuestionPanel.activeSelf)
        {
            if (interactHintUI != null) interactHintUI.SetActive(false);
            return; // Pinapatay natin ang script panandalian habang nagku-quiz para iwas-patong
        }

        // SCENARIO 1: MAY KAUSAP NA (Naka-open ang dialogue box)
        if (DialogueManager.instance != null && DialogueManager.instance.dialogPanel.activeSelf)
        {
            if (interactHintUI != null) interactHintUI.SetActive(false);

            if (Input.GetKeyDown(interactKey))
            {
                DialogueManager.instance.NextLine();
            }
            return;
        }

        // SCENARIO 2: NORMAL GAMEPLAY
        if (interactHintUI != null) interactHintUI.SetActive(false);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                if (interactHintUI != null) interactHintUI.SetActive(true);

                if (Input.GetKeyDown(interactKey))
                {
                    NPC targetNPC = hit.collider.GetComponent<NPC>();
                    if (targetNPC != null)
                    {
                        targetNPC.TriggerDialogue();
                    }
                }
            }
        }
    }
}