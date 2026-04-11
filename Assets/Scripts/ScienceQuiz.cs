using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using StarterAssets; // Namespace para sa player scripts

public class ScienceQuiz : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject scienceQuestionPanel;
    public GameObject blurBackground;
    public GameObject cabinetDoor;
    public TextMeshProUGUI questionText;

    [Header("Fallback System")]
    public int wrongAttempts = 0;
    public GameObject cluePromptPanel;

    public UnityEvent triggerBenDialogue;
    public UnityEvent resetBenDialogue;

    [Header("Level Sequence")]
    public LevelCompletionSequence levelSequence;

    [Header("Player Reference")]
    public FirstPersonController firstPersonController;
    // <-- DAGDAG NATIN ITO PARA MAPIGILAN YUNG MOUSE INPUTS
    public StarterAssetsInputs playerInputs;

    public bool isQuizFinished = false;

    void Update()
    {
        if ((scienceQuestionPanel.activeSelf || cluePromptPanel.activeSelf) && !isQuizFinished)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // THE FIX: Pilitin nating i-zero ang camera movement habang nagsasagot
            if (playerInputs != null)
            {
                playerInputs.look = Vector2.zero;
                playerInputs.cursorInputForLook = false; // Sabihan si StarterAssets na wag munang pansinin ang mouse
            }
        }
    }

    public void OpenQuiz()
    {
        scienceQuestionPanel.SetActive(true);
        blurBackground.SetActive(true);
        isQuizFinished = false;
        wrongAttempts = 0;

        if (firstPersonController != null) firstPersonController.enabled = false;

        StartCoroutine(EnableMouseAutomatic());
    }

    private IEnumerator EnableMouseAutomatic()
    {
        yield return null;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    public void ChooseCorrectAnswer()
    {
        if (isQuizFinished) return;
        isQuizFinished = true;

        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(CorrectRoutine(clickedButton));
    }

    public void ChooseWrongAnswer()
    {
        if (isQuizFinished) return;
        isQuizFinished = true;

        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(WrongRoutine(clickedButton));
    }

    private IEnumerator CorrectRoutine(GameObject btn)
    {
        btn.GetComponent<Image>().color = Color.green;
        questionText.text = "Correct! The ice melted. I should check the unlocked cabinet...";

        resetBenDialogue.Invoke();

        yield return new WaitForSeconds(1.5f);

        // ✅ TRIGGER LEVEL SEQUENCE HERE
        if (levelSequence != null)
        {
            levelSequence.StartSequence();
        }

        scienceQuestionPanel.SetActive(false);
        blurBackground.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (firstPersonController != null) firstPersonController.enabled = true;

        if (playerInputs != null) playerInputs.cursorInputForLook = true;

        if (cabinetDoor != null) cabinetDoor.SetActive(false);
    }

    private IEnumerator WrongRoutine(GameObject btn)
    {
        Image btnImage = btn.GetComponent<Image>();
        btnImage.color = Color.red;

        Vector3 origPos = scienceQuestionPanel.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            float x = origPos.x + Random.Range(-10f, 10f);
            float y = origPos.y + Random.Range(-5f, 5f);
            scienceQuestionPanel.transform.localPosition = new Vector3(x, y, origPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        scienceQuestionPanel.transform.localPosition = origPos;

        yield return new WaitForSeconds(0.5f);
        btnImage.color = Color.white;

        wrongAttempts++;

        if (wrongAttempts >= 2)
        {
            scienceQuestionPanel.SetActive(false);
            cluePromptPanel.SetActive(true);
        }
        else
        {
            isQuizFinished = false;
        }
    }

    // TATAWAGIN NG "YES" BUTTON
    public void AcceptClue()
    {
        // 1. Isara AGAD ang lahat ng UI panels
        cluePromptPanel.SetActive(false);
        scienceQuestionPanel.SetActive(false);
        blurBackground.SetActive(false);

        // 2. Ibalik ang control sa player (Camera at Mouse) agad-agad
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (firstPersonController != null) firstPersonController.enabled = true;
        if (playerInputs != null) playerInputs.cursorInputForLook = true;

        // 3. Sabihan si Ben na i-ready ang clue dialogue niya
        triggerBenDialogue.Invoke();

        // 4. I-reset ang quiz status para ready ulit pagbalik
        wrongAttempts = 0;
        isQuizFinished = false;
    }
    private IEnumerator GoToBenRoutine()
    {
        cluePromptPanel.SetActive(false);
        questionText.text = "Maybe I should go talk to Ben and ask for a clue...";

        triggerBenDialogue.Invoke();

        yield return new WaitForSeconds(2.5f);

        scienceQuestionPanel.SetActive(false);
        blurBackground.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (firstPersonController != null) firstPersonController.enabled = true;

        // I-BALIK ANG MOUSE CONTROL SA PLAYER
        if (playerInputs != null) playerInputs.cursorInputForLook = true;

        wrongAttempts = 0;
        isQuizFinished = false;
    }

    public void DeclineClue()
    {
        cluePromptPanel.SetActive(false);
        scienceQuestionPanel.SetActive(true);
        wrongAttempts = 1;
        isQuizFinished = false;
    }
}