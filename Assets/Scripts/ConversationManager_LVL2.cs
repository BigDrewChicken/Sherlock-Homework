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

    private float timer = 0f;
    private float nextNodeTimer = 0.2f;

    // ================= LIFECYCLE FIX =================

    void OnEnable()
    {
        VD.OnNodeChange += UpdateUI;
        VD.OnEnd += OnDialogueEnd;
    }

    void OnDisable()
    {
        VD.OnNodeChange -= UpdateUI;
        VD.OnEnd -= OnDialogueEnd;
    }

    void Start()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);
    }

    // ================= START CONVERSATION =================

    public void StartConversation(VIDE_Assign npc)
    {
        if (VD.isActive) return;

        VD.EndDialogue();
        VD.BeginDialogue(npc);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);
    }

    // ================= UPDATE =================

    void Update()
    {
        if (!IsTalking) return;

        timer += Time.deltaTime;

        if (!VD.nodeData.isPlayer && timer > nextNodeTimer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                timer = 0f;
                VD.Next();
            }
        }
    }

    // ================= UI UPDATE =================

    void UpdateUI(VD.NodeData data)
    {
        if (dialogueCanvas == null) return;

        timer = 0f;

        if (data.isPlayer)
        {
            if (npcDialogueGroup != null)
                npcDialogueGroup.SetActive(false);

            if (choicePanel != null)
                choicePanel.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            foreach (var btn in choiceButtons)
            {
                if (btn != null)
                    btn.gameObject.SetActive(false);
            }

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
            if (npcDialogueGroup != null)
                npcDialogueGroup.SetActive(true);

            if (choicePanel != null)
                choicePanel.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            nameText.text = data.tag;

            int safeIndex =
                (data.commentIndex < data.comments.Length) ? data.commentIndex : 0;

            npcText.text = data.comments[safeIndex];
        }
    }

    // ================= CHOICES =================

    void SelectChoice(int index)
    {
        if (index < VD.nodeData.comments.Length)
        {
            VD.nodeData.commentIndex = index;

            try
            {
                VD.Next();
            }
            catch
            {
                VD.EndDialogue();
            }
        }
    }

    // ================= END =================

    void OnDialogueEnd(VD.NodeData data)
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(ResetState());
    }

    IEnumerator ResetState()
    {
        yield return new WaitForEndOfFrame();

        VD.isActive = false;
        VD.EndDialogue();
    }
}