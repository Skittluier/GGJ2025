namespace SpiritLevel
{
    using SpiritLevel.Player;
    using System.Collections;
    using UnityEngine;

    public class CameraHandler : Singleton<CameraHandler>
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private float victoryAnimationLength;


        public void FocusOnPlayer()
        {
            Bubble firstPlayerBubble = FindAnyObjectByType<Bubble>();

            if (!firstPlayerBubble)
                return;

            animator.enabled = false;

            StartCoroutine(CryRoutine());
            IEnumerator CryRoutine()
            {
                Vector3 fromPos = transform.position;
                Vector3 toPos = firstPlayerBubble.transform.position;
                toPos.z -= 3;

                float currVal = 0;

                while (currVal < 1)
                {
                    currVal += Time.deltaTime / victoryAnimationLength;
                    transform.position = Vector3.Lerp(fromPos, toPos, currVal);

                    yield return null;

                    if (currVal >= 1)
                        transform.position = toPos;
                }
            }
        }
    }
}