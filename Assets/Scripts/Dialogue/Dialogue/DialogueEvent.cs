using UnityEngine;
using System;

public class DialogueEvent : MonoBehaviour
{
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private ResponseEvent[] events;

    public DialogueText DialogueText => dialogueText;
    public ResponseEvent[] Events => events;

    public void OnValidate()
    {
        if (dialogueText == null) // check if dialogueText is empty
        {
            return;
        }

        if (dialogueText.Responses == null) // check if dialogueText has responses
        {
            return;
        }

        if (events != null && events.Length == dialogueText.Responses.Length) // check if there are events
        {
            return;
        }

        if (events == null)
        {
            events = new ResponseEvent[dialogueText.Responses.Length]; // if there is no ResponseEvent array, make a new one with a length equal to the number of responses in DialogueText
        }
        else
        {
            Array.Resize(ref events, dialogueText.Responses.Length); // Resize ResponseEvent array
        }

        for (int i = 0; i < dialogueText.Responses.Length; i++) // for each possible response in dialogueText
        {
            ResponseOptions responseOptions = dialogueText.Responses[i]; // Create new ResponseOptions

            if (events[i] != null)
            {
                events[i].name = responseOptions.OptionText; // new ResponseOptions is given the same OptionText name
            }

            events[i] = new ResponseEvent() {name = responseOptions.OptionText}; // new ResponseOptions is given a ResponseEvent (latter also gets the same name)

        }
    }
}
