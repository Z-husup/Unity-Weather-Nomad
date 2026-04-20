using System.Collections;
using TMPro;
using UnityEngine;

public class SmartphoneUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform phonePanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;

    [Header("Animation")]
    [SerializeField] private Vector2 hiddenPosition = new Vector2(-320f, 20f);
    [SerializeField] private Vector2 shownPosition = new Vector2(20f, 20f);
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float visibleDuration = 3f;

    private Coroutine currentRoutine;

    private void Start()
    {
        if (phonePanel != null)
        {
            phonePanel.anchoredPosition = hiddenPosition;
        }
    }

    public void ShowMessage(string title, string message)
    {
        if (phonePanel == null || titleText == null || messageText == null)
        {
            Debug.LogWarning("SmartphoneUI is missing references.");
            return;
        }

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowMessageRoutine(title, message));
    }

    private IEnumerator ShowMessageRoutine(string title, string message)
    {
        titleText.text = title;
        messageText.text = message;

        yield return MovePhone(phonePanel.anchoredPosition, shownPosition, moveDuration);
        yield return new WaitForSeconds(visibleDuration);
        yield return MovePhone(phonePanel.anchoredPosition, hiddenPosition, moveDuration);

        currentRoutine = null;
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