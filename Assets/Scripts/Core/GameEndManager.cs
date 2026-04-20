using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject finishScreen;

    [Header("Control")]
    [SerializeField] private bool freezeTimeOnFinish = true;

    private bool gameFinished;
    private bool canCheckForEnd;

    private void OnEnable()
    {
        PlanetRegion.OnAnyRegionStateChanged += CheckForGameEnd;
    }

    private void OnDisable()
    {
        PlanetRegion.OnAnyRegionStateChanged -= CheckForGameEnd;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (finishScreen != null)
            finishScreen.SetActive(false);

        gameFinished = false;
        canCheckForEnd = false;
    }

    public void EnableEndCheck()
    {
        canCheckForEnd = true;
        CheckForGameEnd();
    }

    private void CheckForGameEnd()
    {
        if (!canCheckForEnd)
            return;

        if (gameFinished)
            return;

        PlanetRegion[] regions = FindObjectsByType<PlanetRegion>(FindObjectsSortMode.None);

        if (regions.Length == 0)
            return;

        for (int i = 0; i < regions.Length; i++)
        {
            if (regions[i].IsInDanger)
                return;
        }

        FinishGame();
    }

    private void FinishGame()
    {
        gameFinished = true;

        if (finishScreen != null)
            finishScreen.SetActive(true);

        if (freezeTimeOnFinish)
            Time.timeScale = 0f;

        Debug.Log("All regions are stable. Game finished.");
    }
}