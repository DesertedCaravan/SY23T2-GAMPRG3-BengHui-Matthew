using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBoxManager : MonoBehaviour
{
    [Header("Dialogue Box")]
    [SerializeField] private GameObject dialogueBox;

    [Header("Dialogue")]
    [SerializeField] private TextMeshProUGUI textComponent;

    [SerializeField] private InteractableObject currentInteraction = null;
    [SerializeField] private OptionsUIBase menuTextInteraction = null;

    [SerializeField] private DialogueText currentDialogueBox = null;
    // private char[] currentDialogueText = null;
    private string currentDialogueText = null;

    [Header("Response Events")]
    private ResponseEvent[] responseEvents;

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

    // Old Version
    /*
    private readonly Dictionary<HashSet<char>, float> punctuations = new Dictionary<HashSet<char>, float>()
    {
        { new HashSet<char>() {'.' , '!' , '?'}, 0.6f },
        { new HashSet<char>() {',' , ':' , ';'}, 0.3f },
    };
    */

    [Header("Responses")]
    [SerializeField] private RectTransform responseBox; // Response Box Panel
    [SerializeField] private RectTransform responseContainer; // Response Container
    [SerializeField] private RectTransform responseButtonTemplate; // Option Text Template (TMP)

    private List<GameObject> tempResponseButtons = new List<GameObject>();
    private bool wait;
    private bool menuTextActivated;

    // Convert to Singleton
    public static DialogueBoxManager instance = null; // public static means that it can be accessed

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // textComponent.text = string.Empty;

        // StartDialogue();

        dialogueBox.gameObject.SetActive(false);
        responseBox.gameObject.SetActive(false);

        textSpeed = textDefault;
        textDelay = textDelayDefault;
        wait = false;
        menuTextActivated = false;
    }

    // Update is called once per frame
    void Update() // While gameObject.SetActive(false) prevents the Update loop from occurring
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && wait == false)
        {
            if (textComponent.text == currentDialogueBox.Dialogue[textIndex])
            {
                // Move to next text box
                NextLine();
            }
            else
            {
                // Skip the text loading and display the entire text box

                StopAllCoroutines();
                textComponent.text = currentDialogueBox.Dialogue[textIndex];
                CheckForResponses();
            }
        }
    }

    public void StartDialogue(InteractableObject interaction, DialogueText dialogueText)
    {
        wait = false;

        currentInteraction = interaction;
        currentDialogueBox = dialogueText;
        // dialogueText contains string[] dialogue & ResponseOptions[] responses

        PlayerController.instance.StopMovement(); // Keep Player in place

        dialogueBox.gameObject.SetActive(true); // Make Dialogue Box visible

        textIndex = 0; // Starting text box

        StartCoroutine(CO_TypeLine());
    }

    public void StartMenuText(OptionsUIBase ui, DialogueText text)
    {
        wait = false;

        menuTextActivated = true; // activating Dialogue Box in menu, not due to overworld

        menuTextInteraction = ui;
        currentDialogueBox = text;

        dialogueBox.gameObject.SetActive(true); // Make Dialogue Box visible

        textIndex = 0; // Starting text box

        StartCoroutine(CO_TypeLine());
    }

    void NextLine()
    {
        if (textIndex < currentDialogueBox.Dialogue.Length - 1) // if there's more than one text box
        {
            textIndex++; // move to next index

            StartCoroutine(CO_TypeLine());
        }
        else // if there are no more text boxes left
        {
            StartCoroutine(CO_CloseDialogue());
        }
    }

    public void CheckForResponses()
    {
        if (textIndex == currentDialogueBox.Dialogue.Length - 1 && textComponent.text == currentDialogueBox.Dialogue[textIndex])
        {
            if (textIndex == currentDialogueBox.Dialogue.Length - 1 && currentDialogueBox.HasResponses)
            {
                ShowResponses(currentDialogueBox.Responses);
            }
        }
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    public void ShowResponses(ResponseOptions[] options)
    {
        wait = true;

        float responseBoxHeight = 0f;

        for (int i = 0; i < options.Length; i++) // Old Version: foreach (ResponseOptions response in options)
        {
            ResponseOptions response = options[i];
            int responseIndex = i;

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.OptionText;

            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));

            tempResponseButtons.Add(responseButton);

            // sizeDelta is used for RectTransform
            // Adds the height of the template to the height of the box
            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(ResponseOptions response, int responseIndex)
    {
        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseButtons) // remove all response buttons
        {
            Destroy(button);
        }

        tempResponseButtons.Clear();

        if (responseEvents != null && responseIndex <= responseEvents.Length) // check if responseIndex is within the in bounds of the ResponseEvents array
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke(); // check for a responseEvent in the chosen index, then invoke the OnPickedResponse UnityEvent if there is
        }

        responseEvents = null; // reset responseEvents array just in case

        if (response.OptionResult) // only show new dialogue if the response chosen has any dialogue to show
        {
            if (menuTextActivated == true)
            {
                StartMenuText(menuTextInteraction, response.OptionResult);

                menuTextInteraction.CheckResponseEvents(response.OptionResult); // check if the new dialogue text has any response events that are attached to the interacted Game Object
            }
            else
            {
                StartDialogue(currentInteraction, response.OptionResult);

                currentInteraction.CheckResponseEvents(response.OptionResult); // check if the new dialogue text has any response events that are attached to the interacted Game Object
            }
        }
        else
        {
            textDelay = 0;
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
        currentDialogueText = null;

        if (textIndex > 0) // only delay transition after first text box
        {
            yield return new WaitForSeconds(textDelay);
        }

        // Current Typing Effect (higher textSpeed results in a faster text speed)

        currentDialogueText = currentDialogueBox.Dialogue[textIndex];

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

        #region Old Typing Effects
        // Old Version 2
        /*
        // Set Dialogue Text array
        currentDialogueText = currentDialogueBox.Dialogue[textIndex].ToCharArray();

        // Type each character 1 by 1
        for (int i = 0; i < currentDialogueText.Length; i++)
        {
            // check current item has punctuation
            // exclude last item in array
            // exclude next item in array

            bool isLast = i >= currentDialogueText.Length - 1;

            if (IsPunctuation(currentDialogueText[i], out float waitTime) && !isLast && !IsPunctuation(currentDialogueText[i + 1], out _))
            {
                textSpeed = waitTime;
            }
            else
            {
                textSpeed = textDefault;
            }

            textComponent.text += currentDialogueText[i];

            yield return new WaitForSeconds(textSpeed);
        }
        */

        // Old Version 1
        /*
        foreach (char c in currentDialogueBox.Dialogue[textIndex].ToCharArray())
        {
            if (IsPunctuation(c, out float waitTime))
            {
                textSpeed = waitTime;
            }
            else
            {
                textSpeed = textDefault;
            }

            yield return new WaitForSeconds(textSpeed);

            textComponent.text += c;
        }
        */
        #endregion
        
        CheckForResponses();
    }

    IEnumerator CO_CloseDialogue()
    {
        yield return new WaitForSeconds(textDelay);

        textDelay = textDelayDefault;

        if (menuTextActivated == true)
        {
            menuTextInteraction.SetOptionSelected = false; // allow Menu Option to become interactable again

            menuTextInteraction = null;
            currentDialogueBox = null;
            currentDialogueText = null;

            dialogueBox.gameObject.SetActive(false); // Make Dialogue Box not visible

            menuTextActivated = false;
        }
        else
        {
            currentInteraction.GetComponent<InteractableObject>().AllowInteraction(); // Allow player to interact with object again.
            currentInteraction.GetComponent<InteractableObject>().OnEndInteract(); // Runs code after dialogue is completed.

            currentInteraction = null;
            currentDialogueBox = null;
            currentDialogueText = null;

            dialogueBox.gameObject.SetActive(false); // Make Dialogue Box not visible

            PlayerController.instance.StartMovement(); // Allow player to move again
        }
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
