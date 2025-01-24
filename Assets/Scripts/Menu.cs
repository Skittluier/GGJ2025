using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Scene Indexes")]

    [SerializeField]
    private int mainMenuSceneIndex;

    [SerializeField]
    private int gameplayLevelIndex;


    public void GoToGameplayScene()
    {
        SceneManager.LoadScene(gameplayLevelIndex);
    }
}
                                                     