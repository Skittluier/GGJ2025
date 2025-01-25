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
        if (PlayerManager.Instance.Players.Count > 0)
        {
            bool allPlayersReady = true;

            //Loop over all players and check if ready
            for (int i = 0; i < PlayerManager.Instance.Players.Count; i++)
            {
                if (!PlayerManager.Instance.Players[i].IsReady)
                {
                    allPlayersReady = false;
                    break;
                }
            }

            // All players ready!
            if (allPlayersReady)
                SceneManager.LoadScene(gameplayLevelIndex);
        }
    }

    public void GoToGameplayScene()
    {
        SceneManager.LoadScene(gameplayLevelIndex);
    }
}
                                                     