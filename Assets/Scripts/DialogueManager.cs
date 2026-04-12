using UnityEngine;
using TMPro;
using System.Collections; // IDINAGDAG: Kailangan ito para sa Coroutines (IEnumerator)

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Elements")]
    public GameObject dialogPanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Player Control")]
    public MonoBehaviour cameraLookScript;

    [Header("Typing Settings")]
    public float typingSpeed = 0.04f; // IDINAGDAG: Bilis ng pag-type ng text

    private string[] currentLines;
    private int currentLineIndex = 0;
    private NPC currentNPC;

    private Coroutine typingCoroutine; // IDINAGDAG: Para matigil ang typing kung pipindot agad
    private bool isTyping = false;     // IDINAGDAG: Para malaman kung nagta-type pa

    void Awake() { instance = this; }

    void Start() { dialogPanel.SetActive(false); }

    public void StartDialogue(string npcName, string[] lines, NPC npc)
    {
        currentNPC = npc;
        currentLines = lines;
        currentLineIndex = 0;

        nameText.text = npcName;

        dialogPanel.SetActive(true);

        if (cameraLookScript != null) { cameraLookScript.enabled = false; }

        // NAGBAGO: Imbes na ilabas agad ang text, sisimulan ang typing animation
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
    }

    public void NextLine()
    {
        // BAGONG LOGIC: Kung nagta-type pa tapos pinindot ang 'F', ilabas agad ang buong text (Fast-forward)
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = currentLines[currentLineIndex];
            isTyping = false;
        }
        else // Kung tapos nang mag-type, tsaka lang lilipat sa next line
        {
            currentLineIndex++;
            if (currentLineIndex < currentLines.Length)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
            }
            else
            {
                EndDialogue();
            }
        }
    }

    // BAGONG COROUTINE: Ang tagagawa ng typing effect
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialogPanel.SetActive(false);

        if (cameraLookScript != null) { cameraLookScript.enabled = true; }

        if (currentNPC != null && currentNPC.onDialogueEnd != null)
        {
            currentNPC.onDialogueEnd.Invoke();
        }
    }
}