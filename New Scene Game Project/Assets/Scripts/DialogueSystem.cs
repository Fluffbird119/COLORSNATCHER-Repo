using System.Collections;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    public GameObject dialoguePanel;
    public TMPro.TextMeshProUGUI dialogueText;
    public UnityEngine.UI.Image characterImage;

    private string[] currentLines;
    private int currentLine = 0;

    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private bool canAdvance = false;

    public bool isTalking = false;

    public bool dialogueCooldown = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
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

    public void StartDialogue(string[] lines, string charName, string charTitle, Sprite charImage)
    {
        currentLines = lines;
        currentLine = 0;
        isTalking = true;
        canAdvance = false;
        characterImage.sprite = charImage;
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
}