using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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

    // Inayos ang duplicate variables
    public UnityEvent triggerBenDialogue;
    public UnityEvent resetBenDialogue; // <-- IDAGDAG ITO PARA SA CORRECT ANSWER

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
        }
    }

    public void OpenQuiz()
    {
        scienceQuestionPanel.SetActive(true);
        blurBackground.SetActive(true);
        isQuizFinished = false;
        wrongAttempts = 0; // Fresh start kapag unang bukas ng beaker

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
        isQuizFinished = true; // I-lock agad para hindi madoble ng spam click!

        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(CorrectRoutine(clickedButton));
    }

    public void ChooseWrongAnswer()
    {
        if (isQuizFinished) return;
        isQuizFinished = true; // I-lock agad para hindi madoble ng spam click!

        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(WrongRoutine(clickedButton));
    }

    private IEnumerator CorrectRoutine(GameObject btn)
    {
        btn.GetComponent<Image>().color = Color.green;
        questionText.text = "Correct! The ice melted. I should check the unlocked cabinet...";

        // SABIHAN SI BEN NA TAMA NA ANG SAGOT, BUMALIK NA SIYA SA NORMAL
        resetBenDialogue.Invoke();

        yield return new WaitForSeconds(2.5f);

        scienceQuestionPanel.SetActive(false);
        blurBackground.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (cabinetDoor != null) cabinetDoor.SetActive(false);
    }

    private IEnumerator WrongRoutine(GameObject btn)
    {
        Image btnImage = btn.GetComponent<Image>();
        btnImage.color = Color.red;

        // Shake effect
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

        // LOGIC PARA SA MALI AT FALLBACK
        wrongAttempts++;

        if (wrongAttempts >= 2)
        {
            scienceQuestionPanel.SetActive(false);
            cluePromptPanel.SetActive(true);
        }
        else
        {
            isQuizFinished = false; // Payagan ulit sumagot dahil 1st attempt palang
        }
    }

    // TATAWAGIN NG "YES" BUTTON
    public void AcceptClue()
    {
        // FIX: Buhayin muna ang panel bago tawagin ang Coroutine para hindi mag-error!
        scienceQuestionPanel.SetActive(true);

        // Saka natin patakbuhin ang routine
        StartCoroutine(GoToBenRoutine());
    }

    private IEnumerator GoToBenRoutine()
    {
        cluePromptPanel.SetActive(false);

        // Palitan ang text para alam ng player ang gagawin
        questionText.text = "Maybe I should go talk to Ben and ask for a clue...";

        // Sabihan si Ben na i-ready ang clue dialogue niya
        triggerBenDialogue.Invoke();

        yield return new WaitForSeconds(2.5f);

        // Isara na ang panel at ibalik ang control sa player
        scienceQuestionPanel.SetActive(false);
        blurBackground.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        wrongAttempts = 0;
        isQuizFinished = false;
    }

    // TATAWAGIN NG "NO" BUTTON
    public void DeclineClue()
    {
        cluePromptPanel.SetActive(false);
        scienceQuestionPanel.SetActive(true); // Ibalik ang quiz

        // GAWING 1 ANG MALI! Para sa susunod na magkamali siya (1+1=2), lilitaw ulit itong panel.
        wrongAttempts = 1;
        isQuizFinished = false;
    }
}