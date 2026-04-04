using UnityEngine;
using TMPro; // Para sa TextMeshPro
using VIDE_Data; // Para sa VIDE System

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogPanel; // Yung panel na i-ha-hide/show
    public TMP_Text nameText;      // Para sa NameText
    public TMP_Text dialogueText;  // Para sa Text (TMP)

    void Start()
    {
        // Siguraduhing nakatago ang UI sa simula
        dialogPanel.SetActive(false);
    }

    // Ito ang tatawagin ng Trigger script natin
    public void StartDialogue(VIDE_Assign npc)
    {
        if (!VD.isActive)
        {
            VD.BeginDialogue(npc); // Simulan ang VIDE
            dialogPanel.SetActive(true); // I-show ang UI
            UpdateUI(); // I-load ang unang text
        }
    }

    // Ito ang tatawagin ng Next Button
    public void NextNode()
    {
        if (VD.isActive)
        {
            VD.Next(); // Pumunta sa susunod na node sa VIDE Editor
            UpdateUI();
        }
    }

    // Dito pinapalitan ang laman ng text sa screen
    void UpdateUI()
    {
        VD.NodeData data = VD.nodeData; // Kunin ang current data mula sa VIDE

        if (data.isEnd) // Kung tapos na ang usapan
        {
            VD.EndDialogue();
            dialogPanel.SetActive(false); // Itago ulit ang UI
            return;
        }

        // Kung Player o NPC ang nagsasalita, ilagay sa text box
        if (data.isPlayer)
        {
            nameText.text = "Detective";
            dialogueText.text = data.comments[data.commentIndex]; // Kunin ang text
        }
        else
        {
            nameText.text = data.tag; // Kunin yung tag na "Ben"
            dialogueText.text = data.comments[data.commentIndex];
        }
    }
}