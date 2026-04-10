using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool playerDetected = false;

    protected virtual void Update()
    {
        if (playerDetected && Input.GetKeyDown(KeyCode.F))
        {
            OnInteract();
        }
    }

    public abstract void OnInteract(); 
    public abstract void ShowPrompt(bool show);
    public abstract void ForceEnd();

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCapsule")
        {
            playerDetected = true;
            ShowPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "PlayerCapsule")
        {
            playerDetected = false;
            ShowPrompt(false);
            ForceEnd();
        }
    }
}