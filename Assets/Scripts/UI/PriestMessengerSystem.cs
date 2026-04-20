using UnityEngine;

public class PriestMessengerSystem : MonoBehaviour
{
    public static PriestMessengerSystem Instance { get; private set; }

    [SerializeField] private PriestMessengerUI messengerUI;

    private void Start()
    {
        SendMessage("Mentor Bro", "@Newbie Hey bro. This is our group chat where we will write some requests for weather. People are counting on us :)");
        SendMessage("Mentor Bro", "Keep checking out new messages. New requests will come time to time so, be on reach. Good Luck!");
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SendMessage(string senderName, string message)
    {
        if (messengerUI == null)
        {
            Debug.LogWarning("PriestMessengerSystem: messengerUI is not assigned.");
            return;
        }

        messengerUI.AddMessage(senderName, message);
    }
}