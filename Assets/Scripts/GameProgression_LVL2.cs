using UnityEngine;

public class GameProgression_LVL2 : MonoBehaviour
{
    // Static instance acts as a global reference to find this script easily
    public static GameProgression_LVL2 Instance;

    [Header("Progression Status")]
    public bool ownerDone = false;
    public bool d1Done = false;
    public bool d2Done = false;
    public bool d3Done = false;

    void Awake()
    {
        // Standard Singleton pattern to prevent NullReferenceExceptions
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarkOwnerDone() { ownerDone = true; Debug.Log("<color=green>Progression: Owner Done</color>"); }
    public void MarkD1Done() { d1Done = true; Debug.Log("<color=green>Progression: D1 Done</color>"); }
    public void MarkD2Done() { d2Done = true; Debug.Log("<color=green>Progression: D2 Done</color>"); }
    public void MarkD3Done() { d3Done = true; Debug.Log("<color=green>Progression: D3 Done - CASE CLOSED</color>"); }

    public bool CanTalkTo(string npcName)
    {
        // Debug to see exactly what string is being checked
        Debug.Log("Checking progression for: " + npcName);

        if (npcName.Contains("Owner")) return true;

        // D1 requires Owner to be finished first
        if (npcName.Contains("Defendant 1")) return ownerDone;

        // D2 requires D1 (Winning argument at Node ID 2)
        if (npcName.Contains("Defendant 2")) return d1Done;

        // D3 requires D2 (Winning argument at Node ID 2)
        if (npcName.Contains("Defendant 3")) return d2Done;

        return false;
    }
}