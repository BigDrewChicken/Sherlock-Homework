using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VIDE_Data;
using System.Collections;

public class ConversationManager_LVL2 : MonoBehaviour
{
    [Header("UI Groups")]
    public GameObject dialogueCanvas;
    public GameObject npcDialogueGroup;
    public GameObject choicePanel;

    [Header("Text Content")]
    public TextMeshProUGUI npcText;
    public TextMeshProUGUI nameText;

    [Header("Choices")]
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;

    public bool IsTalking => VD.isActive;
    private float timer = 0;
    private float nextNodeTimer = 0.2f;

    void Start()
    {
        VD.OnNodeChange += UpdateUI;
        VD.OnEnd += OnDialogueEnd;
        dialogueCanvas.SetActive(false);
    }

    public void StartConversation(VIDE_Assign npc)
    {
        if (VD.isActive) return;

        // PROGRESSION REMOVED: You can now talk to any NPC regardless of flags
        VD.EndDialogue();
        VD.BeginDialogue(npc);
        dialogueCanvas.SetActive(true);
    }

    void Update()
    {
        if (!IsTalking) return;
        timer += Time.deltaTime;

        if (!VD.nodeData.isPlayer && timer > nextNodeTimer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                timer = 0;
                VD.Next();
            }
        }
    }

    void UpdateUI(VD.NodeData data)
    {
        timer = 0;
        if (data.isPlayer)
        {
            if (npcDialogueGroup != null) npcDialogueGroup.SetActive(false);
            choicePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            foreach (var btn in choiceButtons) btn.gameObject.SetActive(false);

            for (int i = 0; i < data.comments.Length; i++)
            {
                if (i >= choiceButtons.Length) break;
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = data.comments[i];

                int index = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => SelectChoice(index));
            }
        }
        else
        {
            if (npcDialogueGroup != null) npcDialogueGroup.SetActive(true);
            choicePanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            nameText.text = data.tag;
            int safeIndex = (data.commentIndex < data.comments.Length) ? data.commentIndex : 0;
            npcText.text = data.comments[safeIndex];
        }
    }

    void SelectChoice(int index)
    {
        if (index < VD.nodeData.comments.Length)
        {
            VD.nodeData.commentIndex = index;
            try { VD.Next(); }
            catch { VD.EndDialogue(); }
        }
    }

    void OnDialogueEnd(VD.NodeData data)
    {
        dialogueCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        // LOGIC CLEANUP: Removed progression flags to prevent NullReferenceErrors
        if (VD.assigned != null)
        {
            Debug.Log("Dialogue with " + VD.assigned.gameObject.name + " ended at Node: " + data.nodeID);
        }

        StartCoroutine(ResetVIDEState());
    }

    IEnumerator ResetVIDEState()
    {
        yield return new WaitForEndOfFrame();
        VD.isActive = false;
        VD.EndDialogue();
    }
}