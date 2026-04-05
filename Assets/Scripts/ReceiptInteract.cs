using UnityEngine;

public class ReceiptInteract : Interactable
{
    public GameObject receiptCanvas;      
    public GameObject hintCanvas;         
    public GameObject interactButton;     

    void Start()
    {
        
        if (receiptCanvas != null) receiptCanvas.SetActive(false);
        if (hintCanvas != null) hintCanvas.SetActive(false);
    }

    public override void OnInteract()
    {
        
        if (NPCInteract.hasFinishedFirstTalk)
        {
            
            if (receiptCanvas != null) receiptCanvas.SetActive(true);
            if (hintCanvas != null) hintCanvas.SetActive(false);
        }
        else
        {
            
            if (hintCanvas != null) hintCanvas.SetActive(true);
            if (receiptCanvas != null) receiptCanvas.SetActive(false);
        }

        ShowPrompt(false); 
    }

    public override void ShowPrompt(bool show)
    {
        if (interactButton != null) interactButton.SetActive(show);
    }

    public override void ForceEnd()
    {
        
        if (receiptCanvas != null) receiptCanvas.SetActive(false);
        if (hintCanvas != null) hintCanvas.SetActive(false);
    }
}