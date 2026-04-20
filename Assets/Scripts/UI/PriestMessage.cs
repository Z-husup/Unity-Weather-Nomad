[System.Serializable]
public class PriestMessage
{
    public string senderName;
    public string text;

    public PriestMessage(string senderName, string text)
    {
        this.senderName = senderName;
        this.text = text;
    }
}