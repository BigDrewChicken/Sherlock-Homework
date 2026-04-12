using UnityEngine;
using TMPro;
using System.Collections;

public class StartDialogue : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject startButton;

    [Header("Player (First Person)")]
    public GameObject player;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] lines;

    public float typingSpeed = 0.05f;

    private int index = 0;
    private bool isTyping = false;

    void Start()
    {
        StartDialogueSequence();
    }

    public void StartDialogueSequence()
    {
        // Show UI
        dialoguePanel.SetActive(true);
        startButton.SetActive(false);

        // Unlock cursor for UI clicking
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player movement (first person)
        if (player != null)
            player.SetActive(false);

        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("No dialogue lines assigned!");
            yield break;
        }

        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in lines[index])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        startButton.SetActive(true);
    }

    public void NextLine()
    {
        if (isTyping) return;

        index++;

        if (lines == null || index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        startButton.SetActive(false);
        StartCoroutine(TypeLine());
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        // Lock cursor back for FPS control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Enable player movement again
        if (player != null)
            player.SetActive(true);

        Debug.Log("Game Started!");
    }
}