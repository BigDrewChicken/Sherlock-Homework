using UnityEngine;

public class HardResetOnLoad : MonoBehaviour
{
    void Awake()
    {
        // 🔥 Reset ALL cross-level static state here
        ReceiptInteract.isReceiptSolved = false;

        NPCInteract.hasFinishedFirstTalk = false;
        NPCInteractLVL3.hasFinishedFirstTalk = false;

        // If you added more static flags later, reset them here too
    }
}