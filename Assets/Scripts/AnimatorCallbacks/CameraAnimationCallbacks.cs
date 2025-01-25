using UnityEngine;

public class CameraAnimationCallbacks : MonoBehaviour
{
    /// <summary>
    /// Callback invoked through animations that the intro animation on the camera is done playing
    /// </summary>
    public void OnIntroCutsceneDone()
    {
        Game.Instance.StartCountdownSequence();
    }
}
