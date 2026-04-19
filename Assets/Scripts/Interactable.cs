using UnityEngine;

// 'abstract' means you can't attach THIS script to an object. 
// Instead, you create new scripts that "inherit" from this one.
public abstract class Interactable : MonoBehaviour
{
    // Keeps track of whether the player is currently standing in the trigger zone.
    protected bool playerDetected = false;

    // 'virtual' allows child classes to override this Update if they need extra logic.
    protected virtual void Update()
    {
        // Check if the player is nearby AND if they just pressed the 'F' key.
        if (playerDetected && Input.GetKeyDown(KeyCode.F))
        {
            OnInteract();
        }
    }

    // --- ABSTRACT METHODS ---
    // These have no code here; any script inheriting from Interactable 
    // MUST define what happens inside these functions.

    public abstract void OnInteract();   // What happens when I press F?
    public abstract void ShowPrompt(bool show); // Show/Hide the "Press F to Interact" UI.
    public abstract void ForceEnd();     // Stop the interaction immediately (e.g., player walks away).

    // Logic for when the player enters the trigger collider (set to 'Is Trigger').
    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if the object entering is specifically named "PlayerCapsule".
        // Note: It's usually safer to use Tags (other.CompareTag("Player")).
        if (other.name == "PlayerCapsule")
        {
            playerDetected = true;
            ShowPrompt(true);
        }
    }

    // Logic for when the player leaves the trigger collider.
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "PlayerCapsule")
        {
            playerDetected = false;
            ShowPrompt(false);
            ForceEnd(); // Ensure menus or dialogues close if the player runs away.
        }
    }
}