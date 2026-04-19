using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public static InteractionPrompt Instance;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // Hide it at the start
    }

    public void SetPrompt(bool state)
    {
        gameObject.SetActive(state);
    }
}