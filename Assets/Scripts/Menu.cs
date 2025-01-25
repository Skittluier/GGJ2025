using SpiritLevel.Player;
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


    public void Update()
    {
        bool allPlayersReady = false;

        //Loop over all players and check if ready
        for (int i = 0; i < PlayerManager.Instance.Players.Count; i++)
        {
            
        }
    }

    public void GoToGameplayScene()
    {
        SceneManager.LoadScene(gameplayLevelIndex);
    }
}
                                                     