using UnityEngine;
using UnityEngine.Events; // Ilagay ito para magamit natin ang events!

public class NPC : MonoBehaviour
{
    public string npcName;
    public string[] dialogueLines;

    // Ito ay gagawa ng box sa Inspector na eksaktong kapareho ng "On Click ()" ng isang Button!
    public UnityEvent onDialogueEnd;

    public void TriggerDialogue()
    {
        // Ipapasa na natin ang sarili niya (this) sa Manager para alam ng game kung sino ang huling nagsalita
        DialogueManager.instance.StartDialogue(npcName, dialogueLines, this);
    }
}