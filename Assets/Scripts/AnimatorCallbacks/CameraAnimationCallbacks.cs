using SpiritLevel.Player;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraAnimationCallbacks : MonoBehaviour
{
    /// <summary>
    /// Callback invoked through animations that the intro animation on the camera is done playing
    /// </summary>
    public void OnIntroCutsceneDone()
    {
        Game.Instance?.StartCountdownSequence();
    }

    /// <summary>
    /// Makes the buble that is being focussed on in the intro cry baby cry
    /// </summary>
    public void MakeBubbleCry()
    {
        Bubble firstPlayerBubble = PlayerManager.Instance?.Players[0]?.Bubble;

        if (!firstPlayerBubble)
            return;

        StartCoroutine(CryRoutine());

        IEnumerator CryRoutine()
        {
            firstPlayerBubble.SetExpression(Bubble.Expression.Crying, 0.5f);
            yield return new WaitForSeconds(0.75f);

            firstPlayerBubble.SetExpression(Bubble.Expression.Crying, 0.5f);
            yield return new WaitForSeconds(0.75f);

            firstPlayerBubble.SetExpression(Bubble.Expression.Crying, 0.5f);
        }
    }
}
