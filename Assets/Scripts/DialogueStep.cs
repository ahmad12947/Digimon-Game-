[System.Serializable]
public class DialogueStep
{
    public string speaker;
    public string text;
    public DialogueType type;
    public string[] choices;
}

public enum DialogueType { Text, Choice, NameInput }
