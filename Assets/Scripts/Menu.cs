using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Scene Indexes")]

    [SerializeField]
    private int mainMenuSceneIndex;

    [SerializeField]
    private int gameplayLevelIndex;

    [SerializeField]
    private GameObject JoinGamePanel;
    [SerializeField]
    private GameObject MainMenuPanel;

    public void EnableJoinScreen()
    {
        MainMenuPanel.SetActive(false);
        JoinGamePanel.SetActive(true);
    }

    public void GoToGameplayScene()
    {
        SceneManager.LoadScene(gameplayLevelIndex);
    }
}
                                                     