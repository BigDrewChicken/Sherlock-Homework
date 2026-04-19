using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT BUTTON CLICKED");
    }
    public void OpenSettings()
    {
        Debug.Log("SETTINGS OPENED");
    }
}
