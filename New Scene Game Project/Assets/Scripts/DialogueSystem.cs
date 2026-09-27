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
    public UnityEngine.UI.Image yellowImage;

    public bool isCutscenePlaying = false;

    void Awake()
    {
        instance = this;
    }

    /*
    void Start()
    {
        dialoguePanel.SetActive(false);
        playerImage.gameObject.SetActive(false);
        redImage.gameObject.SetActive(false);
    }
    */
    

    void Update()
    {
        if (isTalking && canAdvance && Input.GetKeyDown(KeyCode.Space))
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
            yellowImage.gameObject.SetActive(false);
        }
        else if (characterName.Equals("red"))
        {
            playerImage.gameObject.SetActive(false);
            redImage.gameObject.SetActive(true);
            yellowImage.gameObject.SetActive(false);
        }
        else if (characterName.Equals("none"))
        {
            playerImage.gameObject.SetActive(false);
            redImage.gameObject.SetActive(false);
            yellowImage.gameObject.SetActive(false);
        }
        else if (characterName.Equals("yellow"))
        {
            playerImage.gameObject.SetActive(false);
            redImage.gameObject.SetActive(false);
            yellowImage.gameObject.SetActive(true);
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
        playerImage.gameObject.SetActive(false);
        redImage.gameObject.SetActive(false);
        StartCoroutine(DialogueCooldown());
    }

    private IEnumerator DialogueCooldown()
    {
        dialogueCooldown = true;
        yield return new WaitForSeconds(0.2f);
        dialogueCooldown = false;
    }

    public IEnumerator StartCutscene(int cutsceneNumber)
    {
        if (cutsceneNumber == 1)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "It has been so long. so so so so so long",
                " but i have found it",
                "the red land",
                "the land of red",
                "a land of redness"}, "player");
            while (isTalking)
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
            StartDialogue(new string[] { "DIEEEEEEEEEE", "btw use left click button to attack and WASD/arrows to move around" }, "red");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 2)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "GRR, you taking down all these enemies makes me SUPER ANGRY", "I'm boiling with rage... im steaming out of my ears", "you could even say",
            "im SMOKIN HOTTTTTTTTTTTTTTTTT"}, "red");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 3)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "Why do you do this? Why? I send more of my men out and you just kill them ALL!" }, "red");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "You have something I need, lord of red.",
            "There are problems with me. Problems that must be solved.",
            "I can not live with myself. I have no purpose, no meaning.",
            "And your world is so beautiful. So crimson. So Vibrant.",
            "I live with nothing. And here I was. No color. No soul. No beauty.",
            "I want your color. I wan't your red. I want the passion you feel and the anger you hold."}, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "On my beautiful chiseled body and my mighty shrimp head",
            "YOU WILL DIEEEEEEEEE!!!!!!!!!",
            "and other dramatic war cries"}, "red");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 4)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "I...I lost... I actually LOST???", "ITS IMPOSSIBLE!!!", 
                "I CANT LOSE TO SOMEONE LIKE YOU. SOMEBODY WITH NO PURPOSE, NO COLOR, NO FEELINGS, NO THOUGHTS, YOU'RE COMPLETELY FLAVORLESS!" }, "red");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] {"It's okay.", "I will be killing you now."}, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "I thought with all of my might that no mere gray little b---- could f---ing kill me.",
                "That my pure anger could be channeled into fire and fight.",
                "Yet how could someone with no feelings possibly muster up the will to do this?",}, "red");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "..." }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Hah, I believe I see it now.",
                "You are nothing. You had no purpose, no meaning, no will. And that absence became the significance. You had nothing but a will to be something.",
                "And that blind obsession due to a lack of anything else gave you the meaning and purpose to kill me",
                "I assume you're going to kill the other color lords and take their feelings too?",
                "Well good f___ing luck."}, "red");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "...","You killed him." }, "none");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] {"What... is this feeling? Is this it? The red anger of rage?",
                "I can feel it now! The anger! The will! The motivation! The pure malice to kill everything! The adrenaline to achieve my goals!",
                "I'm going to take every color. One kill at a time" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 5)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "some time passes" }, "yellow");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "HIIII IM YELLOW AND I LIKE TO PARTAYYYYY" }, "yellow");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "YELLLOWWWWWW", "I'M GONNA KILL YOU", "I WILL CHASE YOU TO THE ENDS OF THE EARTHHHHH" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Yoooo whats goody gang that sounds fun!" }, "yellow");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Shut UP.", "You may not know me, but I just killed the entity of red, and I'll kill you too!!!" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Killing! Woah, thats crazy! You're getting me riled up!!!" }, "yellow");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] {"This is NOT a GAME.", "For all my life, I was TRAPPED in an emotionless body.", "But now, I can THINK. And I can FEEL.",
                "And I have been filled with hate. HATE.", "Hate for every disgusting colored being that holds their emotions", "And I want them ALL!!!"}, "player");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 6)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "Wow! You are really good at this!" }, "yellow");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "I HATE YOU!!!!!!!!!!!!!!!1!!!!1!!!1!!!!!"}, "player");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 7)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "Today I ate some cookies!" }, "yellow");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] {"Are you TWELVE."}, "player");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
    }
}