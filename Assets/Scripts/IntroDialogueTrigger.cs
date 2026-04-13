using UnityEngine;
using System.Collections;
using StarterAssets;

public class AutoStartDialogue : MonoBehaviour
{
    [Header("References")]
    public ConversationManager conversationManager;
    
    [Header("Dialogue Content")]
    public DialogueLine[] introLines; 

    [Header("Settings")]
    public float delayBeforeStart = 0.5f;

    private StarterAssetsInputs playerInputs;
    private bool isRunningIntro = false;

    void Start()
    {
        playerInputs = FindFirstObjectByType<StarterAssetsInputs>();

        if (conversationManager != null)
        {
            StartCoroutine(TriggerIntro());
        }
    }

    IEnumerator TriggerIntro()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        if (introLines != null && introLines.Length > 0)
        {
            isRunningIntro = true;
            conversationManager.StartManualConversation(introLines);
        }
    }

    void Update()
    {
        // If we aren't in the intro, don't do anything
        if (!isRunningIntro || conversationManager == null) return;

        bool isTalking = conversationManager.IsTalking();

        if (isTalking)
        {
            // 1. FREEZE MOVEMENT: Keep player from walking away
            if (playerInputs != null) playerInputs.move = Vector2.zero;

            // 2. LISTEN FOR [F]: This is what was missing!
            if (Input.GetKeyDown(KeyCode.F))
            {
                conversationManager.OnInteractPressed();
            }
        }
        else
        {
            // Dialogue ended, stop the "Babysitter" logic
            isRunningIntro = false;
        }
    }
}