using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.F;

    [Header("UI References")]
    public GameObject interactHintUI;

    void Update()
    {
        // SCENARIO 1: MAY KAUSAP NA (Naka-open ang dialogue box)
        if (DialogueManager.instance.dialogPanel.activeSelf)
        {
            // Itago ang Hint UI para malinis ang screen
            if (interactHintUI != null) interactHintUI.SetActive(false);

            // Kapag pinindot ang 'F', magne-next line agad kahit saan pa nakatingin!
            if (Input.GetKeyDown(interactKey))
            {
                DialogueManager.instance.NextLine();
            }

            // Huminto na dito ang code. Huwag nang mag-shoot ng laser.
            return;
        }

        // SCENARIO 2: NORMAL GAMEPLAY (Naglalaman ng paghahanap ng NPC)
        // Default: Itago muna ang hint UI
        if (interactHintUI != null) interactHintUI.SetActive(false);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                // Kung NPC ang tinamaan, ilabas ang "[F] Interact"
                if (interactHintUI != null) interactHintUI.SetActive(true);

                // Kung pinindot ang 'F', simulan ang usapan
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