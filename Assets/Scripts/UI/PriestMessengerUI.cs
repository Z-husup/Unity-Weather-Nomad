using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PriestMessengerUI : MonoBehaviour
{
    [Header("Phone Panel")]
    [SerializeField] private RectTransform phonePanel;

    [Header("Message List")]
    [SerializeField] private Transform messageContainer;
    [SerializeField] private GameObject messageItemPrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Animation")]
    [SerializeField] private float moveDuration = 0.5f;

    private Coroutine currentRoutine;

    private void Start()
    {
    }

    public void AddMessage(string senderName, string message)
    {
        if (messageContainer == null || messageItemPrefab == null || phonePanel == null)
        {
            Debug.LogWarning("PriestMessengerUI references are missing.");
            return;
        }

        GameObject item = Instantiate(messageItemPrefab, messageContainer);
        MessageItemUI itemUI = item.GetComponent<MessageItemUI>();

        if (itemUI != null)
            itemUI.Setup(senderName, message);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        StartCoroutine(ScrollToBottomNextFrame());

    }
    
    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }


    private IEnumerator MovePhone(Vector2 start, Vector2 end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            phonePanel.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        phonePanel.anchoredPosition = end;
    }
}