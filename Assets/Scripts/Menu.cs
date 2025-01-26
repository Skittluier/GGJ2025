namespace SpiritLevel
{
    using SpiritLevel.Player;
    using System.Collections;
    using TMPro;
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
        private TMP_Text scanToJoinText;
        private string defaultScanToJoinText;

#if UNITY_EDITOR
        [SerializeField]
        private bool EDITOR_enableOnePlayerMode;
#endif


        private IEnumerator Start()
        {
            defaultScanToJoinText = scanToJoinText.text;

            while (PlayerManager.Instance == null)
                yield return null;

            PlayerManager.Instance.OnRoomCodeUpdated += UpdateScanToJoinText;
        }

        private void OnDestroy()
        {
            PlayerManager.Instance.OnRoomCodeUpdated -= UpdateScanToJoinText;
        }

        private void UpdateScanToJoinText(string roomCode)
        {
            scanToJoinText.text = string.Format(defaultScanToJoinText, roomCode);
        }

        public void Update()
        {
#if UNITY_EDITOR
            if (EDITOR_enableOnePlayerMode && PlayerManager.Instance.Players.Count > 0)
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
                {
                    SceneManager.LoadScene(gameplayLevelIndex);
                    return;
                }
            }
#endif

            if (PlayerManager.Instance.Players.Count > 1)
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
}