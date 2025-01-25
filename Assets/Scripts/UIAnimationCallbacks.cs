using UnityEngine;
using UnityEngine.SceneManagement;

public class UIAnimationCallbacks : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
