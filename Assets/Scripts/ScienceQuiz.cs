using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class ScienceQuiz : MonoBehaviour
{
    // Idagdag itong dalawang bagong variables:
    [Header("UI Objects")]
    public GameObject scienceQuestionPanel;
    public GameObject blurBackground;
    public GameObject cabinetDoor;
    public TextMeshProUGUI questionText;

    // Isama ito para malaman natin kapag natapos na sumagot
    public bool isQuizFinished = false;

    // Gawing simple muna yung function para sa tamang sagot (Button A)
    public void ChooseCorrectAnswer()
    {
        if (isQuizFinished) return;
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(CorrectRoutine(clickedButton));
    }

    // Gawing simple muna yung function para sa maling sagot (Buttons B, C, D)
    public void ChooseWrongAnswer()
    {
        if (isQuizFinished) return;
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(WrongRoutine(clickedButton));
    }

    // Ito ang mangyayari kapag nagbukas ang panel
    // Pwedeng tawagin ito mula sa InteractableBeaker script
    public void OpenQuiz()
    {
        scienceQuestionPanel.SetActive(true);
        blurBackground.SetActive(true);
        isQuizFinished = false;

        // Tawagin natin ang Coroutine para sigurado ang paglabas ng mouse
        StartCoroutine(EnableMouseAutomatic());
    }

    private IEnumerator EnableMouseAutomatic()
    {
        // Maghintay ng isang frame para matapos yung "F" keypress event
        yield return null;

        // Force the cursor to show up and stay unlocked
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // I-unselect ang kahit anong dating naka-highlight para fresh ang click
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("Mouse should be visible now!");
    }

    private IEnumerator CorrectRoutine(GameObject btn)
    {
        isQuizFinished = true;
        btn.GetComponent<Image>().color = Color.green;
        questionText.text = "Correct! The ice melted. I should check the unlocked cabinet...";

        yield return new WaitForSeconds(2.5f);

        // Itago ulit ang mga UI elements
        scienceQuestionPanel.SetActive(false);
        blurBackground.SetActive(false); // Itago ang blur

        // Itago ulit ang mouse cursor para sa paglalakad
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (cabinetDoor != null)
        {
            cabinetDoor.SetActive(false); // Bubukas yung cabinet
        }
    }

    private IEnumerator WrongRoutine(GameObject btn)
    {
        isQuizFinished = true;
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
        btnImage.color = Color.white; // Babalik sa puti yung button

        isQuizFinished = false; // Pwede ulit sumagot
    }

    void Update()
    {
        // Hangga't active ang panel at hindi pa tapos ang quiz, 
        // laging pilitin na lumabas ang mouse
        if (scienceQuestionPanel.activeSelf && !isQuizFinished)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}