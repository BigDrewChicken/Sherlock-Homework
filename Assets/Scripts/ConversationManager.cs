using UnityEngine;
using VIDE_Data; // Make sure VIDE is installed
using TMPro;

public class ConversationManager : MonoBehaviour
{

    // The current stage of the mystery: 0=Owner, 1=Def1, 2=Def2, 3=Def3
    public int investigationProgress = 0; 

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcText;
    public TextMeshProUGUI nameText;
    public GameObject choiceButtonPrefab; // A button prefab to spawn for Player Nodes
    public Transform choiceContainer;    // The UI parent where buttons appear

    // This is the function DialogueInteract will call
    public void TryStartConversation(string npcName)
    {
        if (VD.isActive) return;

        // Check if player is talking to the correct person in order
        if (CanTalkTo(npcName))
        {
            StartDialogue(npcName);
        }
        else
        {
            Debug.Log("I haven't found enough evidence to talk to " + npcName + " yet.");
            // Optional: Show a "I'm not ready to talk to them" UI message here
        }
    }

    private bool CanTalkTo(string name)
    {
        if (name == "Owner" && investigationProgress == 0) return true;
        if (name == "Defendant 1" && investigationProgress == 1) return true;
        if (name == "Defendant 2" && investigationProgress == 2) return true;
        if (name == "Defendant 3" && investigationProgress == 3) return true;
        return false;
    }

    private void StartDialogue(string name)
    {
        VD.OnNodeChange += UpdateUI;
        VD.OnEnd += EndDialogue;
        VD.BeginDialogue(name); // Starts the VIDE node tree
        dialoguePanel.SetActive(true);
    }

   void UpdateUI(VD.NodeData data)
    {
        // Clear previous choice buttons
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        if (data.isPlayer) 
        {
            for (int i = 0; i < data.comments.Length; i++)
            {
                int index = i;
                GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = data.comments[i];
                
                // FIX START: This is the part causing your error
                btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                    data.commentIndex = index; // Set the choice
                    VD.Next();                 // Call Next with NO arguments
                });
                // FIX END
            }
        }
        else 
        {
            npcText.text = data.comments[data.commentIndex];
            nameText.text = data.tag;
        }
    }

 void EndDialogue(VD.NodeData data)
    {
        VD.OnNodeChange -= UpdateUI;
        VD.OnEnd -= EndDialogue;
        dialoguePanel.SetActive(false);
        
        investigationProgress++; 
    } // This closes the EndDialogue function
} // THIS IS THE ONE YOU ARE LIKELY MISSING (closes the Class)