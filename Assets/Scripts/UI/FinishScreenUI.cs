using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScreenUI : MonoBehaviour
{
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}