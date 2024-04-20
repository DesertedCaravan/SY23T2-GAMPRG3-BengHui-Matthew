using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleDialogueBoxManager : MonoBehaviour
{
    [Header("Dialogue Box")]
    [SerializeField] private GameObject dialogueBox;

    [Header("Dialogue")]
    [SerializeField] private TextMeshProUGUI textComponent;
    private string currentDialogueText = null;
    private List<string> currentDialogueBox = null;

    [Header("Text Data")]
    private int textIndex; // Current index in text array

    public float textSpeed;
    private float textDefault = 50f;

    private float textDelay;
    private float textDelayDefault = 0.05f;

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>() {'.', '!', '?' }, 0.6f),
        new Punctuation(new HashSet<char>() {',', ';', ';' }, 0.3f),
    };

    private bool _startBattlePhase;

    [Header("Battle Manager Updater")]
    [SerializeField] private BattleManager battleManager;

    // Start is called before the first frame update
    void Start()
    {
        dialogueBox.gameObject.SetActive(false);

        textSpeed = textDefault;
        textDelay = textDelayDefault;

        _startBattlePhase = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && _startBattlePhase == true)
        {
            if (textComponent.text == currentDialogueBox[textIndex])
            {
                // Move to next text box
                NextLine();
            }
            else
            {
                // Skip the text loading and display the entire text box

                StopAllCoroutines();
                textComponent.text = currentDialogueBox[textIndex];
            }
        }
    }

    public void StartBattlePhase(int slotSide, string activeCritter, string chosenMove, List<bool> moveOutcome, bool result)
    {
        _startBattlePhase = true;

        // Setup currentDialogueBox
        currentDialogueText = null;
        currentDialogueBox = new List<string>();

        textIndex = 0;

        if (slotSide >= 0 && slotSide <= 4) // For PHandler, Ally Critters and EHandler
        {
            currentDialogueBox.Add(activeCritter + " used " + chosenMove + "!");
            // currentDialogueText = activeCritter + " used " + chosenMove + "!";
        }
        else
        {
            currentDialogueBox.Add("Enemy's " + activeCritter + " used " + chosenMove + "!");
            // currentDialogueText = "Enemy's " + activeCritter + " used " + chosenMove + "!";
        }

        
        textIndex++;

        foreach (bool outcome in moveOutcome)
        {
            if (outcome == true)
            {
                currentDialogueBox.Add("Hit!");
            }
            else
            {
                currentDialogueBox.Add("Missed!");
            }
        }
        

        dialogueBox.gameObject.SetActive(true); // Make Dialogue Box visible

        textIndex = 0; // Starting text box

        StartCoroutine(CO_TypeLine());
    }

    void NextLine()
    {
        if (textIndex < currentDialogueBox.Count - 1) // rework if there's more than one text box
        {
            textIndex++; // move to next index

            StartCoroutine(CO_TypeLine());
        }
        else // if there are no more text boxes left
        {
            StartCoroutine(CO_CloseDialogue());
        }
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations) // Punctuation was formerly KeyValuePair<HashSet<char>, float>
        {
            if (punctuationCategory.Punctuations.Contains(character)) // Punctuations was formerly Key
            {
                waitTime = punctuationCategory.WaitTime; // Punctuations was formerly Value
                return true;
            }
        }

        waitTime = textDefault; // default
        return false;
    }

    IEnumerator CO_TypeLine()
    {
        textComponent.text = string.Empty;

        if (textIndex > 0) // only delay transition after first text box
        {
            yield return new WaitForSeconds(textDelay);
        }

        // Current Typing Effect (higher textSpeed results in a faster text speed)

        currentDialogueText = currentDialogueBox[textIndex];

        float t = 0;
        int charIndex = 0;
        textSpeed = textDefault;

        while (charIndex < currentDialogueText.Length)
        {
            int lastCharIndex = charIndex; // Depending on the speed, several characters might be typed at once during the same frame, so this variable acts as a starting point for each frame.

            t += Time.deltaTime * textSpeed;

            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, currentDialogueText.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                // check current item has punctuation
                // exclude last item in array
                // exclude next item in array

                bool isLast = i >= currentDialogueText.Length - 1;

                textComponent.text = currentDialogueText.Substring(0, i + 1); // String is essentially viewing increasing portions of the current dialogue

                if (IsPunctuation(currentDialogueText[i], out float waitTime) && !isLast && !IsPunctuation(currentDialogueText[i + 1], out _))
                {
                    textSpeed = waitTime;
                }
                else
                {
                    textSpeed = textDefault;
                }

                // yield return new WaitForSeconds(textSpeed); // not needed due to textSpeed change affecting the variable t
            }

            yield return null;
        }

        textComponent.text = currentDialogueText; // String views the entire portion of the current dialogue
    }

    IEnumerator CO_CloseDialogue()
    {
        yield return new WaitForSeconds(textDelay);

        textDelay = textDelayDefault;

        currentDialogueText = null;
        currentDialogueBox = new List<string>();

        _startBattlePhase = false;

        dialogueBox.gameObject.SetActive(false); // Make Dialogue Box not visible

        battleManager.ActiveCritterMoveStart(true); // allow for next active critter to make move
    }

    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctutations, float waitTime)
        {
            Punctuations = punctutations;
            WaitTime = waitTime;
        }
    }
}