using UnityEngine;
using VIDE_Data;

public class WinTrigger_LVL2 : MonoBehaviour
{
    public GameObject winCanvas;
    // Set this to exactly what your Culprit's name is in the Hierarchy
    public string culpritName = "Rob";

    void OnEnable()
    {
        VD.OnEnd += HandleDialogueEnd;
    }

    void OnDisable()
    {
        VD.OnEnd -= HandleDialogueEnd;
    }

    void HandleDialogueEnd(VD.NodeData data)
    {
        // THE SAFETY FILTER:
        // We check the name of the 'nodeData.tag' or the object we are interacting with.
        // If the dialogue that just ended isn't from our Culprit, we stop here.
        if (data.tag != culpritName)
        {
            return;
        }

        // Only if it's the culprit do we show the win screen
        if (winCanvas != null)
        {
            winCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Success: " + culpritName + " confessed. Win screen shown.");
        }
    }
}