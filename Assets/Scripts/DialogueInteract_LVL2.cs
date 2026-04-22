using UnityEngine;
using VIDE_Data;

public class DialogueInteract_LVL2 : MonoBehaviour
{
    public string npcName;
    public VIDE_Assign assignedDialogue;

    void Start()
    {
        if (assignedDialogue == null) assignedDialogue = GetComponentInParent<VIDE_Assign>();
    }
}