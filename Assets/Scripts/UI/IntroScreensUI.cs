using UnityEngine;

public class IntroScreensUI : MonoBehaviour
{
    [Header("Screens Order")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject inspirationScreen;
    [SerializeField] private GameObject tutorialScreen;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject gameplayUIRoot;

    [Header("Managers")]
    [SerializeField] private GameEndManager gameEndManager;

    [Header("Control")]
    [SerializeField] private bool freezeTimeDuringIntro = true;

    private int currentScreenIndex = 0;
    private GameObject[] screens;

    private void Start()
    {
        screens = new GameObject[]
        {
            startScreen,
            inspirationScreen,
            tutorialScreen
        };

        if (freezeTimeDuringIntro)
            Time.timeScale = 0f;

        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(false);

        ShowOnlyScreen(0);
    }

    public void ShowNextScreen()
    {
        currentScreenIndex++;

        if (currentScreenIndex >= screens.Length)
        {
            StartGame();
            return;
        }

        ShowOnlyScreen(currentScreenIndex);
    }

    public void StartGame()
    {
        HideAllScreens();

        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(true);

        Time.timeScale = 1f;

        if (gameEndManager != null)
            gameEndManager.EnableEndCheck();
    }

    private void ShowOnlyScreen(int index)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if (screens[i] != null)
                screens[i].SetActive(i == index);
        }
    }

    private void HideAllScreens()
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if (screens[i] != null)
                screens[i].SetActive(false);
        }
    }
}