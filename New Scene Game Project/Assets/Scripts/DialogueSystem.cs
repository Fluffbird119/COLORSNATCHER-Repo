using System.Collections;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    public GameObject dialoguePanel;
    public TMPro.TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentLine = 0;

    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private bool canAdvance = false;

    public bool isTalking = false;

    public bool dialogueCooldown = false;

    public UnityEngine.UI.Image playerImage;
    public UnityEngine.UI.Image redImage;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
        StartCoroutine(StartCutscene(1));
    }

    void Update()
    {
        if (isTalking && canAdvance && Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
                SkipTyping();
            else
                NextLine();
        }
    }

    public void StartDialogue(string[] lines, string characterName)
    {
        currentLines = lines;
        currentLine = 0;
        isTalking = true;
        canAdvance = false;
        if (characterName.Equals("player"))
        {
            playerImage.gameObject.SetActive(true);
            redImage.gameObject.SetActive(false);
        }
        else if (characterName.Equals("red"))
        {
            playerImage.gameObject.SetActive(false);
            redImage.gameObject.SetActive(true);
        }
        dialoguePanel.SetActive(true);
        ShowLine(currentLines[currentLine]);
        StartCoroutine(EnableAdvance());
    }

    private IEnumerator EnableAdvance()
    {
        yield return new WaitForSeconds(0.1f);
        canAdvance = true;
    }

    private void ShowLine(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private void NextLine()
    {
        currentLine++;
        if (currentLine < currentLines.Length)
        {
            ShowLine(currentLines[currentLine]);
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.04f);
        }
        isTyping = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        dialogueText.text = currentLines[currentLine];
        isTyping = false;
    }

    private void EndDialogue()
    {
        isTalking = false;
        canAdvance = false;
        dialoguePanel.SetActive(false);
        StartCoroutine(DialogueCooldown());
    }

    private IEnumerator DialogueCooldown()
    {
        dialogueCooldown = true;
        yield return new WaitForSeconds(0.2f);
        dialogueCooldown = false;
    }

    private IEnumerator StartCutscene(int cutsceneNumer)
    {
        if (cutsceneNumer == 1)
        {
            StartDialogue(new string[] { "It has been so long. so so so so so long",
                " but i have found it",
                "the red land",
                "the land of red",
                "a land of redness"}, "player");
            while(isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "GRRR WHO ARE YOU" }, "red");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "im gonna kill u" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "WHAAAAT. I DONT EVEN KNOW WHO YOU ARE", "BUT THIS MAKES ME VERY ANGRY" }, "red");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "whats an angry" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "DIEEEEEEEEEE",  "btw use left click button to attack and WASD/arrows to move around" }, "red");
            while (isTalking)
            {
                yield return null;
            }
        }
    }
}