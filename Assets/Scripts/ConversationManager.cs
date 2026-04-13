using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VIDE_Data;

public class ConversationManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI npcText;
    public TextMeshProUGUI nameText;

    // This property fixes the 'IsTalking' errors in your other script
    public bool IsTalking => VD.isActive;

    void Start()
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);

        // Fixes the 'matches delegate' errors by using the correct signature
        VD.OnNodeChange += UpdateUI;
        VD.OnEnd += OnDialogueEnd;
    }

    void OnDestroy()
    {
        VD.OnNodeChange -= UpdateUI;
        VD.OnEnd -= OnDialogueEnd;
    }

    public void StartConversation(VIDE_Assign npc)
    {
        if (npc == null) return;
        VD.BeginDialogue(npc);
        if (dialogueCanvas != null) dialogueCanvas.SetActive(true);
    }

    public void HandleInteraction()
    {
        if (!VD.isActive) return;
        VD.Next();
    }

    // This signature matches what VIDE expects (VD.NodeData)
    void UpdateUI(VD.NodeData data)
    {
        if (npcText != null && nameText != null)
        {
            npcText.text = data.comments[data.commentIndex];
            nameText.text = data.tag;
        }
    }

    void OnDialogueEnd(VD.NodeData data)
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }

    public void ForceEnd()
    {
        VD.EndDialogue();
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
    }
}