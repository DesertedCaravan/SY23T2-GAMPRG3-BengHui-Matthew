using UnityEngine;

[System.Serializable]
public class ResponseOptions
{
    [SerializeField] private string optionText;
    [SerializeField] private DialogueText optionResult;

    public string OptionText => optionText;
    public DialogueText OptionResult => optionResult;
}