using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelCompletionSequence : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject cameraExperiment;
    public GameObject cameraProfessor;

    [Header("Player")]
    public GameObject player;

    [Header("UI - Teacher Dialogue")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("UI - Experiment Dialogue")]
    public GameObject experimentDialoguePanel;
    public TextMeshProUGUI experimentDialogueText;

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel;

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.1f;
    public float fastTypeSpeed = 0.01f;

    [Header("Experiment Dialogue")]
    [TextArea(3, 10)]
    public string experimentDialogue = "The experiment setup looks strange... something is missing.";

    [Header("Teacher Dialogues")]
    [TextArea(3, 10)]
    public string teacherDialogue1 = "Good job, detective.";

    [TextArea(3, 10)]
    public string teacherDialogue2 = "You solved the mystery of the missing experiment.";

    private bool isTyping = false;
    private bool canPressF = false;

    private int dialogueStep = 0;

    private enum SequenceState
    {
        Idle,
        LookAtExperiment,
        WaitFirstF,
        ExperimentDialogue,
        ProfessorDialogue,
        WaitSecondF,
        Completed
    }

    private SequenceState state = SequenceState.Idle;

    void Update()
    {
        if (state == SequenceState.ExperimentDialogue && Input.GetKeyDown(KeyCode.F))
        {
            if (!isTyping && canPressF)
            {
                StartProfessorDialogue();
            }
        }

        if (state == SequenceState.ProfessorDialogue && Input.GetKeyDown(KeyCode.F))
        {
            if (!isTyping && canPressF)
            {
                NextDialogue();
            }
        }

        if (state == SequenceState.WaitSecondF && Input.GetKeyDown(KeyCode.F))
        {
            CompleteLevel();
        }
    }

    public void StartSequence()
    {
        state = SequenceState.LookAtExperiment;

        cameraExperiment.SetActive(true);
        cameraProfessor.SetActive(false);

        dialoguePanel.SetActive(false);
        experimentDialoguePanel.SetActive(false);

        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        StartCoroutine(GoToFirstStep());

        player.SetActive(false); // Naka-freeze na ang player dito
    }

    private IEnumerator GoToFirstStep()
    {
        yield return null;
        StartExperimentDialogue();
    }

    private void StartExperimentDialogue()
    {
        state = SequenceState.ExperimentDialogue;

        cameraExperiment.SetActive(true);
        cameraProfessor.SetActive(false);

        experimentDialoguePanel.SetActive(true);
        experimentDialogueText.text = "";

        StartCoroutine(TypeExperimentDialogue());
    }

    private IEnumerator TypeExperimentDialogue()
    {
        isTyping = true;
        canPressF = false;

        experimentDialogueText.text = "";

        foreach (char c in experimentDialogue)
        {
            experimentDialogueText.text += c;
            float currentSpeed = Input.GetKey(KeyCode.F) ? fastTypeSpeed : typeSpeed;
            yield return new WaitForSeconds(currentSpeed);
        }

        isTyping = false;
        canPressF = true;
    }

    private void StartProfessorDialogue()
    {
        state = SequenceState.ProfessorDialogue;

        experimentDialoguePanel.SetActive(false);

        cameraExperiment.SetActive(false);
        cameraProfessor.SetActive(true);

        dialoguePanel.SetActive(true);

        dialogueStep = 0;

        StartCoroutine(ShowDialogueStep());
    }

    private void NextDialogue()
    {
        dialogueStep++;

        if (dialogueStep >= 2)
        {
            state = SequenceState.WaitSecondF;
            return;
        }

        StartCoroutine(ShowDialogueStep());
    }

    private IEnumerator ShowDialogueStep()
    {
        canPressF = false;
        isTyping = true;

        string textToShow = "";

        if (dialogueStep == 0)
            textToShow = teacherDialogue1;
        else if (dialogueStep == 1)
            textToShow = teacherDialogue2;

        dialogueText.text = "";

        foreach (char c in textToShow)
        {
            dialogueText.text += c;
            float currentSpeed = Input.GetKey(KeyCode.F) ? fastTypeSpeed : typeSpeed;
            yield return new WaitForSeconds(currentSpeed);
        }

        isTyping = false;
        canPressF = true;
    }

    private void CompleteLevel()
    {
        state = SequenceState.Completed;

        dialoguePanel.SetActive(false);
        experimentDialoguePanel.SetActive(false);

        // ✅ 1. ILABAS ANG "YOU WON" CANVAS
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);

        // ✅ 2. ILABAS ANG MOUSE POINTER (Utos ng Leader mo)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Level Completed and Mouse Unlocked!");
    }
}