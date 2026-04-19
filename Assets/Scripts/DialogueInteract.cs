using UnityEngine;

public class DialogueInteract : MonoBehaviour
{
    public ConversationManager manager;
    public string thisNPCName; 

    void Update()
    {
        // Check if player is pressing F
        if (Input.GetKeyDown(KeyCode.F)) 
        {
            // Optional: Add a distance check here if needed
            if (manager != null) {
                manager.TryStartConversation(thisNPCName);
            } else {
                Debug.LogError("Manager is missing on " + gameObject.name);
            }
        }
    }
}