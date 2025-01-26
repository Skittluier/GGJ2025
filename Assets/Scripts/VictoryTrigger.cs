namespace SpiritLevel
{
    using UnityEngine;

    public class VictoryTrigger : MonoBehaviour
    {
        [SerializeField]
        private LayerMask playerLayers;

        private bool isTriggered = false;


        private void OnTriggerEnter(Collider other)
        {
            if (!isTriggered && Utilities.Contains(playerLayers, other.gameObject.layer))
            {
                isTriggered = true;

                if (other.GetComponent<Bubble>() is Bubble bubble)
                    bubble.Finish();

                Game.Instance.PlayerWin();
            }
        }
    }
}