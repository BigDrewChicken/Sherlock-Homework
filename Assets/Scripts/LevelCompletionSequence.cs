using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Cinemachine;

public class LevelCompletionSequence : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera cmExperiment;
    public CinemachineVirtualCamera cmProfessor;

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
    private bool skipTyping = false;

    private int dialogueStep = 0;

    private enum SequenceState
    {
        Idle,
        LookAtExperiment,
        ExperimentDialogue,
        ProfessorDialogue,
        WaitSecondF,
        Completed
    }

    private SequenceState state = SequenceState.Idle;

    // ⭐ ADDED (FIX for scrambling FIRST teacher dialogue)
    private Coroutine experimentCoroutine;
    private Coroutine dialogueCoroutine;
    private bool professorDialogueStarted = false;

    void Update()
    {
        // ⭐ SKIP TYPEWRITER (safe instant complete)
        if (Input.GetKeyDown(KeyCode.F) && isTyping)
        {
            skipTyping = true;

            if (experimentCoroutine != null)
                StopCoroutine(experimentCoroutine);

            if (dialogueCoroutine != null)
                StopCoroutine(dialogueCoroutine);

            if (experimentDialoguePanel.activeSelf)
                experimentDialogueText.text = experimentDialogue;

            if (dialoguePanel.activeSelf)
            {
                string fullText = (dialogueStep == 0) ? teacherDialogue1 : teacherDialogue2;
                dialogueText.text = fullText;
            }

            isTyping = false;
            canPressF = true;
            return;
        }

        if (state == SequenceState.ExperimentDialogue && Input.GetKeyDown(KeyCode.F))
        {
            if (!isTyping && canPressF)
                StartProfessorDialogue();
        }

        if (state == SequenceState.ProfessorDialogue && Input.GetKeyDown(KeyCode.F))
        {
            if (!isTyping && canPressF)
                NextDialogue();
        }

        if (state == SequenceState.WaitSecondF && Input.GetKeyDown(KeyCode.F))
        {
            CompleteLevel();
        }
    }

    public void StartSequence()
    {
        StopAllCoroutines();

        experimentCoroutine = null;
        dialogueCoroutine = null;

        // ⭐ FIX reset lock
        professorDialogueStarted = false;

        state = SequenceState.LookAtExperiment;

        dialoguePanel.SetActive(false);
        experimentDialoguePanel.SetActive(false);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        player.SetActive(false);

        StartCoroutine(GoToFirstStep());
    }

    private IEnumerator GoToFirstStep()
    {
        yield return null;
        StartExperimentDialogue();
    }

    private void StartExperimentDialogue()
    {
        state = SequenceState.ExperimentDialogue;

        SwitchCamera(cmExperiment, cmProfessor);

        StartCoroutine(ExperimentDelay());
    }

    private IEnumerator ExperimentDelay()
    {
        yield return new WaitForSeconds(2f);

        experimentDialoguePanel.SetActive(true);
        experimentDialogueText.text = "";

        experimentCoroutine = StartCoroutine(TypeExperimentDialogue());
    }

    private void StartProfessorDialogue()
    {
        state = SequenceState.ProfessorDialogue;

        experimentDialoguePanel.SetActive(false);

        SwitchCamera(cmProfessor, cmExperiment);

        StartCoroutine(ProfessorDelay());
    }

    private IEnumerator ProfessorDelay()
    {
        yield return new WaitForSeconds(2f);

        // ⭐ FIX: prevents double-start causing scrambled FIRST teacher dialogue
        if (professorDialogueStarted) yield break;
        professorDialogueStarted = true;

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        dialogueStep = 0;

        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueCoroutine = StartCoroutine(ShowDialogueStep());
    }

    private void NextDialogue()
    {
        dialogueStep++;

        if (dialogueStep >= 2)
        {
            state = SequenceState.WaitSecondF;
            return;
        }

        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueCoroutine = StartCoroutine(ShowDialogueStep());
    }

    private IEnumerator TypeExperimentDialogue()
    {
        isTyping = true;
        canPressF = false;
        skipTyping = false;

        experimentDialogueText.text = "";

        foreach (char c in experimentDialogue)
        {
            if (skipTyping) break;

            experimentDialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        experimentDialogueText.text = experimentDialogue;

        isTyping = false;
        canPressF = true;
    }

    private IEnumerator ShowDialogueStep()
    {
        isTyping = true;
        canPressF = false;
        skipTyping = false;

        string textToShow = (dialogueStep == 0) ? teacherDialogue1 : teacherDialogue2;

        dialogueText.text = "";

        foreach (char c in textToShow)
        {
            if (skipTyping) break;

            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        dialogueText.text = textToShow;

        isTyping = false;
        canPressF = true;
    }

    private void CompleteLevel()
    {
        state = SequenceState.Completed;

        dialoguePanel.SetActive(false);
        experimentDialoguePanel.SetActive(false);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Level Completed and Mouse Unlocked!");
    }

    private void SwitchCamera(CinemachineVirtualCamera toActivate, CinemachineVirtualCamera toDeactivate)
    {
        toActivate.Priority = 20;
        toDeactivate.Priority = 10;
    }
}