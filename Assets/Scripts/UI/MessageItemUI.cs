using TMPro;
using UnityEngine;

public class MessageItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text senderText;
    [SerializeField] private TMP_Text messageText;

    public void Setup(string senderName, string message)
    {
        if (senderText != null)
            senderText.text = senderName;

        if (messageText != null)
            messageText.text = message;
    }
}