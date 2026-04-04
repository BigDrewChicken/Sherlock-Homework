using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public ConversationManager conversationManager;
    private bool playerDetected = false;

    void Update()
    {
        if (playerDetected)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                conversationManager.OnInteractPressed();
            }

            if (!conversationManager.IsTalking() && !conversationManager.interactButton.gameObject.activeSelf)
            {
                conversationManager.ShowPrompt(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCapsule")
        {
            playerDetected = true;
            conversationManager.ShowPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "PlayerCapsule")
        {
            playerDetected = false;
            conversationManager.ShowPrompt(false);
            conversationManager.ForceEnd();
        }
    }
}