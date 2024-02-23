using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Text", menuName = "Dialogue Box/Dialogue Text")]
public class DialogueText : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private ResponseOptions[] responses;

    public string[] Dialogue => dialogue; // getter function
    public ResponseOptions[] Responses => responses;
    public bool HasResponses => Responses != null && Responses.Length > 0;
}
