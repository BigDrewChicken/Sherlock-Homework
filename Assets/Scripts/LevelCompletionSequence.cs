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
    public GameObject player; // drag your Player here

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
    public float fastTypeSpeed = 0.01f; // ✅ ADDED (speed when holding F)

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
        // ❌ REMOVED NEED FOR FIRST F (no change here, just won't be used anymore)

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
        levelCompletePanel.SetActive(false);

        Debug.Log("Level sequence started");

        StartCoroutine(GoToFirstStep());

        player.SetActive(false); // ✅ disable player
    }

    private IEnumerator GoToFirstStep()
    {
        yield return null;

        // ✅ AUTO START EXPERIMENT DIALOGUE (no F needed)
        StartExperimentDialogue();
    }

    // EXPERIMENT DIALOGUE (NEW UI)
    private void StartExperimentDialogue()
    {
        state = SequenceState.ExperimentDialogue;

        cameraExperiment.SetActive(true);
        cameraProfessor.SetActive(false);

        experimentDialoguePanel.SetActive(true);
        experimentDialogueText.text = "";

        StartCoroutine(TypeExperimentDialogue());

        Debug.Log("Experiment dialogue shown");
    }

    private IEnumerator TypeExperimentDialogue()
    {
        isTyping = true;
        canPressF = false;

        experimentDialogueText.text = "";

        foreach (char c in experimentDialogue)
        {
            experimentDialogueText.text += c;

            // ✅ HOLD F TO SPEED UP
            float currentSpeed = Input.GetKey(KeyCode.F) ? fastTypeSpeed : typeSpeed;
            yield return new WaitForSeconds(currentSpeed);
        }

        isTyping = false;
        canPressF = true;
    }

    // TEACHER DIALOGUE (UNCHANGED UI)
    private void StartProfessorDialogue()
    {
        state = SequenceState.ProfessorDialogue;

        experimentDialoguePanel.SetActive(false);

        cameraExperiment.SetActive(false);
        cameraProfessor.SetActive(true);

        dialoguePanel.SetActive(true);

        dialogueStep = 0;

        StartCoroutine(ShowDialogueStep());

        Debug.Log("Professor dialogue started");
    }

    private void NextDialogue()
    {
        dialogueStep++;

        if (dialogueStep >= 2)
        {
            state = SequenceState.WaitSecondF;
            Debug.Log("Press F again to finish level");
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

            // ✅ SAME SPEED-UP FOR TEACHER DIALOGUE
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
        levelCompletePanel.SetActive(true);

        Debug.Log("Level Completed!");
    }
}