using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR // Everything that is inside this clause only runs when in the Unity Editor. 
using UnityEditor;
#endif

public class SceneLoader_LVL2 : MonoBehaviour
{
    // Scene dragged from the Project window to avoid typing scene names manually
#if UNITY_EDITOR // Scene asset can only be accessed in Unity Editor, so this is essential.
    public SceneAsset scene;
#endif

    // Stores the scene name used by SceneManager
    private string sceneName;

    // Gets the scene name from the assigned SceneAsset
    void Awake()
    {
#if UNITY_EDITOR // The whole purpose of finding the SceneAsset name is to store it in a variable so the function can load the scene based on the scene name 
        sceneName = scene.name;
#endif
    }

    // Loads the assigned scene (used for level buttons)
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

}