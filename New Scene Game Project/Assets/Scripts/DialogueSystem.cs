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
    public UnityEngine.UI.Image greenImage;
    public UnityEngine.UI.Image blueImage;
    public GameObject playerBox;
    public GameObject redBox;
    public GameObject yellowBox;
    public GameObject greenBox;
    public GameObject blueBox;

    public bool isCutscenePlaying = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

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
            playerBox.gameObject.SetActive(true);
            redImage.gameObject.SetActive(false);
            redBox.gameObject.SetActive(false);
            yellowImage.gameObject.SetActive(false);
            yellowBox.gameObject.SetActive(false);
            greenImage.gameObject.SetActive(false);
            greenBox.gameObject.SetActive(false);
            blueImage.gameObject.SetActive(false);
            blueBox.gameObject.SetActive(false);
        }
        else if (characterName.Equals("red"))
        {
            playerImage.gameObject.SetActive(false);
            playerBox.gameObject.SetActive(false);
            redImage.gameObject.SetActive(true);
            redBox.gameObject.SetActive(true);
            yellowImage.gameObject.SetActive(false);
            yellowBox.gameObject.SetActive(false);
            greenImage.gameObject.SetActive(false);
            greenBox.gameObject.SetActive(false);
            blueImage.gameObject.SetActive(false);
            blueBox.gameObject.SetActive(false);
        }
        else if (characterName.Equals("none"))
        {
            playerImage.gameObject.SetActive(false);
            playerBox.gameObject.SetActive(false);
            redImage.gameObject.SetActive(false);
            redBox.gameObject.SetActive(false);
            yellowImage.gameObject.SetActive(false);
            yellowBox.gameObject.SetActive(false);
            greenImage.gameObject.SetActive(false);
            greenBox.gameObject.SetActive(false);
            blueImage.gameObject.SetActive(false);
            blueBox.gameObject.SetActive(false);
        }
        else if (characterName.Equals("yellow"))
        {
            playerImage.gameObject.SetActive(false);
            playerBox.gameObject.SetActive(false);
            redImage.gameObject.SetActive(false);
            redBox.gameObject.SetActive(false);
            yellowImage.gameObject.SetActive(true);
            yellowBox.gameObject.SetActive(true);
            greenImage.gameObject.SetActive(false);
            greenBox.gameObject.SetActive(false);
            blueImage.gameObject.SetActive(false);
            blueBox.gameObject.SetActive(false);
        }
        else if (characterName.Equals("green"))
        {
            playerImage.gameObject.SetActive(false);
            playerBox.gameObject.SetActive(false);
            redImage.gameObject.SetActive(false);
            redBox.gameObject.SetActive(false);
            yellowImage.gameObject.SetActive(false);
            yellowBox.gameObject.SetActive(false);
            greenImage.gameObject.SetActive(true);
            greenBox.gameObject.SetActive(true);
            blueImage.gameObject.SetActive(false);
            blueBox.gameObject.SetActive(false);
        }
        else if (characterName.Equals("blue"))
        {
            playerImage.gameObject.SetActive(false);
            playerBox.gameObject.SetActive(false);
            redImage.gameObject.SetActive(false);
            redBox.gameObject.SetActive(false);
            yellowImage.gameObject.SetActive(false);
            yellowBox.gameObject.SetActive(false);
            greenImage.gameObject.SetActive(false);
            greenBox.gameObject.SetActive(false);
            blueImage.gameObject.SetActive(true);
            blueBox.gameObject.SetActive(true);
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
        yellowImage.gameObject.SetActive(false);
        greenImage.gameObject.SetActive(false);
        blueImage.gameObject.SetActive(false);
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
        if (isCutscenePlaying)
        {
            yield break;
        }

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
            StartDialogue(new string[] { "la dee dah im just a critter who is NOT evil I would never do that" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "WHAAAAT. I DONT EVEN KNOW HOW TO READ", "BUT THIS MAKES ME VERY ANGRY" }, "red");
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
            StartDialogue(new string[] { "It's okay.", "I will be killing you now." }, "player");
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
            StartDialogue(new string[] { "...", "You killed him." }, "none");
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
            StartDialogue(new string[] { "RAGE acquired, press 'Q' to feel the RAGE come inside of you, making you feel so powerful",
                "so powerful", "powerful RAGE", "yeah", "also it lets you parry btw"}, "none");
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
            StartDialogue(new string[] { "I HATE YOU!!!!!!!!!!!!!!!1!!!!1!!!1!!!!!" }, "player");
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
            StartDialogue(new string[] { "Are you TWELVE." }, "player");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 8)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "Wow, that was so much fun! Thanks fren!!!", "Wait, what are you doing?", "Nononono stopstopstopstop",
            "AAAAAAAAAAAAAAAAAAAA",
            "AAAAAAAAAAAAAAAAAAAAAAAAA",
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGH"}, "yellow");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "And now, you're color is mine!!! YES!", "Is this happiness? I feel so good! So joyous! So excited!",
            "Have they been keeping this all away from me? This color and emotion?",
            "I want their feelings. They're holding it from me. I will show them. I can kill them",
            "IM GONNA TAKE EVERY COLOR IN THE WORLD!!!!!!!!"}, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "DASH acquired, press 'Shift' to do that"}, "none");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 9)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "After a short trip, the colorless finds themself in a field of green" }, "none");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "ommmmmmmmmmmmmmmmmmmmmmmmmmmmm ommmmmmmmmmmmmmmmmmmmm", "ommmmmmmmmmm", "*meditating*", "Heyyyyyy bro" }, "green");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "IM GONNA RIP YOUR HEART OUT AND EAT YOUR ORGANS AAAAAAAAAAA" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "what the freaky" }, "green");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 10)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "Okay bro, stop sliming out my buddies before I have to put you down." }, "green");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "I CANT STOP! IM LIVING! IM FINALLY LIVING!", "DONT TRY TO STOP ME OR ILL KILL YOU HARDER" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Well thats not living. Living is being content in the moment, experiencing things because you can.", "when you're alive, you have love" }, "green");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "STOP WITH YOUR PHILISOPHICAL[sic] BULLS--- IM GONNA MAUL YOU RAHHHH LELELELELELELE" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 11)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "Why are you doing this?" }, "green");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "I was born without color. My life was a void. But one day, I realized. You are all so full of color. Of emotion.",
            "And I want it. I WANT IT!!!"}, "player");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 12)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "oh no im dying" }, "green");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "boooooooom" }, "none");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "What is this? I feel... fine. I feel satiated. I feel... good?", "I should stop here." ,
            "but this feels so good...", "But I have to keep going. What if theres more? More good feelings. Like all of these ones!",
            "I want good feelings! I love good feelings!"}, "player");
            StartDialogue(new string[] { "SLUDGE acquired, press 'E' to splooge",
                "the gunk is supposed to stun enemies but it's lowkey bugged so use your imagination"}, "none");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 13)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "And now, dear player, you have reached the end." }, "none");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Murderer", "MURDERER", "MURDERERRRRRR" }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "I can feel everything.", "I can sense everything.", "I want what you have." }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "FRIEND KILLER.", "BEGONE" }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 13)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "And now, dear player, you have reached the end." }, "none");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Murderer", "MURDERER", "MURDERERRRRRR" }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "I can feel everything.", "I can sense everything.", "I want what you have." }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "FRIEND KILLER.", "BEGONE" }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 14)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "You. I just need you! I NEED YOU" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Empty void. You won't discover what is not already there." }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "But I'm doing it. I'm living. All these years I was a mindless corpse, but now I feel so REAL." }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "One often kills himself in the attempt to be alive" }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "I'm tired of all this lecturing. I WANT TO KILL YOU!" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 15)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "No more." }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "You can feel the beast cry." }, "none");
            while (isTalking)
            {
                yield return null;
            }
            isCutscenePlaying = false;
        }
        else if (cutsceneNumber == 16)
        {
            isCutscenePlaying = true;
            StartDialogue(new string[] { "And now, it is done.", "you killed them all" }, "none");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "YES, I HAVE IT ALL. RAGE. EXCITEMENT. TRANQUILITY. SORROW",
                "What... what is this last one? Why do I feel bad. WHY DO I FEEL REGRET?"}, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "With their dying breath, the being of blue spoke." }, "none");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "Sadness. Empathy. You will feel what you have done.", "You will finally live in the context of the world and the brutality of the experiences you have carried out." }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "The passion of red. The fun of yellow. The peace of green. I... I ended it all.", "WHY DO I CARE.", "WHY DO I FEEL THIS WAY?",
            "I DONT WANT TO FEEL. I DON't WANT TO CARE."}, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "If you had cared from the start, this never would have happened" }, "blue");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "What does that mean? What do I do? WHAT DO I DO?", "COME BACKKKKKK" }, "player");
            while (isTalking)
            {
                yield return null;
            }
            StartDialogue(new string[] { "It was too late. The colors were long gone.", "The feelings and love had forever faded away." }, "none");
            isCutscenePlaying = false;
        }
    }
}