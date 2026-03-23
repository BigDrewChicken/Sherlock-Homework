// Create ExitButton.cs - Attach to your button!
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    void Start()
    {
        // AUTO-FIND Interactable & Connect!
        Interactable inter = FindObjectOfType<Interactable>();
        if (inter != null)
        {
            Button btn = GetComponent<Button>();
            btn.onClick.RemoveAllListeners(); // Clear old
            btn.onClick.AddListener(() => inter.ExitInteractionButton());
            Debug.Log("ExitButton AUTO-CONNECTED to Interactable!");
        }
    }
}