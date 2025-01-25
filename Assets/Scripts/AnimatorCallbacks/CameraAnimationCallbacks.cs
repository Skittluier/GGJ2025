using SpiritLevel;
using SpiritLevel.Player;
using System.Collections;
using UnityEngine;

public class CameraAnimationCallbacks : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField]
    private AudioClip introSequenceAudioClip;


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
        GlobalAudio.Instance.PlayAudioResource(introSequenceAudioClip);
        Bubble firstPlayerBubble = PlayerManager.Instance?.Players[0]?.Bubble;

        if (!firstPlayerBubble)
            return;

        StartCoroutine(CryRoutine());

        IEnumerator CryRoutine()
        {
            firstPlayerBubble.SetExpression(Bubble.Expression.Crying, 14f);
            yield return new WaitForSeconds(14f);

            firstPlayerBubble.SetExpression(Bubble.Expression.Floating, 2f);
        }
    }
}
