using UnityEngine;

public class NPCInteract : Interactable
{
    public ConversationManager conversationManager;
    
    public static bool hasFinishedFirstTalk = false;

    public override void OnInteract()
    {
        conversationManager.OnInteractPressed();
    }

    public override void ShowPrompt(bool show)
    {
        conversationManager.ShowPrompt(show);
    }

    public override void ForceEnd()
    {
        conversationManager.ForceEnd();
    }

    protected override void Update()
    {
        base.Update(); 

        if (playerDetected && !conversationManager.IsTalking() && !conversationManager.interactButton.gameObject.activeSelf)
        {
            
            if (conversationManager.conversation.Length > 0 && !hasFinishedFirstTalk)
            {
                hasFinishedFirstTalk = true; 
            }
            ShowPrompt(true);
        }
    }
}