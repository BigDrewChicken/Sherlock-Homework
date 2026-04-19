using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using StarterAssets;

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
    public StarterAssetsInputs playerInputs;

    [Header("Player & Interaction Fix")]
    public GameObject interactPromptUI;
    public Collider beakerCollider;

    public bool isQuizFinished = false;

    void Update()
    {
        if (scienceQuestionPanel.activeSelf || cluePromptPanel.activeSelf)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (playerInputs != null)
            {
                playerInputs.look = Vector2.zero;
                playerInputs.cursorInputForLook = false;
            }
        }
    }

    void LateUpdate()
    {
        if (scienceQuestionPanel.activeSelf || cluePromptPanel.activeSelf)
        {
            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
                interactPromptUI.transform.localScale = Vector3.zero;
            }
        }
        else
        {
            if (interactPromptUI != null)
            {
                interactPromptUI.transform.localScale = Vector3.one;
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
        if (beakerCollider != null) beakerCollider.enabled = false;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

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

        GameObject clickedButton = null;
        if (EventSystem.current != null)
        {
            clickedButton = EventSystem.current.currentSelectedGameObject;
        }
        StartCoroutine(CorrectRoutine(clickedButton));
    }

    private IEnumerator CorrectRoutine(GameObject btn)
    {
        if (btn != null)
        {
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null) btnImage.color = Color.green;
        }

        questionText.text = "Correct! The ice melted. I should check the unlocked cabinet...";

        resetBenDialogue.Invoke();

        yield return new WaitForSeconds(1.5f);

        scienceQuestionPanel.SetActive(false);
        blurBackground.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (cabinetDoor != null) cabinetDoor.SetActive(false);

        // ✅ FIX 1: PERMANENTLY i-disable ang Beaker para hindi na ito sumabat sa 'F' key ng cutscene
        if (beakerCollider != null)
        {
            beakerCollider.enabled = false;
            beakerCollider.gameObject.tag = "Untagged";
        }

        // ✅ FIX 2: Kung may cutscene na susunod, WAG munang ibalik ang control sa player!
        if (levelSequence != null)
        {
            levelSequence.StartSequence();
        }
        else
        {
            // Ibabalik lang ang player movement kung walang cutscene na naka-assign
            if (firstPersonController != null) firstPersonController.enabled = true;
            if (playerInputs != null) playerInputs.cursorInputForLook = true;
        }
    }

    public void ChooseWrongAnswer()
    {
        if (isQuizFinished) return;
        isQuizFinished = true;

        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(WrongRoutine(clickedButton));
    }

    private IEnumerator WrongRoutine(GameObject btn)
    {
        Image btnImage = btn.GetComponent<Image>();
        if (btnImage != null) btnImage.color = Color.red;

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
        if (btnImage != null) btnImage.color = Color.white;

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

    public void AcceptClue()
    {
        cluePromptPanel.SetActive(false);
        scienceQuestionPanel.SetActive(false);
        blurBackground.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (firstPersonController != null) firstPersonController.enabled = true;
        if (playerInputs != null) playerInputs.cursorInputForLook = true;

        if (beakerCollider != null) beakerCollider.enabled = true;

        triggerBenDialogue.Invoke();

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