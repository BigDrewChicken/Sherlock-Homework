using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Elements")]
    public GameObject dialogPanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Player Control")]
    public MonoBehaviour cameraLookScript;

    private string[] currentLines;
    private int currentLineIndex = 0;
    private NPC currentNPC; // Bagong memorya para matandaan kung sino ang kausap

    void Awake() { instance = this; }

    void Start() { dialogPanel.SetActive(false); }

    // --- NAGBAGO DITO: Idinagdag natin ang "NPC npc" ---
    public void StartDialogue(string npcName, string[] lines, NPC npc)
    {
        currentNPC = npc; // Tandaan kung sino ang kausap natin
        currentLines = lines;
        currentLineIndex = 0;

        nameText.text = npcName;
        dialogueText.text = currentLines[currentLineIndex];

        dialogPanel.SetActive(true);

        if (cameraLookScript != null) { cameraLookScript.enabled = false; }
    }

    public void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < currentLines.Length)
        {
            dialogueText.text = currentLines[currentLineIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogPanel.SetActive(false);

        if (cameraLookScript != null) { cameraLookScript.enabled = true; }

        // --- NAGBAGO DITO ---
        // Kapag tapos na ang dialogue, i-trigger natin yung "On Dialogue End" ng NPC na iyon!
        if (currentNPC != null && currentNPC.onDialogueEnd != null)
        {
            currentNPC.onDialogueEnd.Invoke();
        }
    }
}