using UnityEngine;
using UnityEngine.Timers;

public class Obstacle : MonoBehaviour
{
    private Bubble Bubble;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private float startTime;
    [SerializeField]
    private float timeNeeded;
    [SerializeField]
    private Collider obstacleCollider;

    private bool canStartShaking;
    private bool startedTimer;
    private void OnTriggerEnter(Collider collision)
    {
        var hitLayerMask = 1 << collision.gameObject.layer;
        if ((hitLayerMask & mask) == 0)
        {
            if (Bubble == null)
                Debug.LogError("No bubble");

            canStartShaking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var hitLayerMask = 1 << collision.gameObject.layer;
        if ((hitLayerMask & mask) == 0)
        {
            if (Bubble == null)
                Debug.LogError("No bubble PLOX help");
            canStartShaking = false;

        }
    }
    
    public void StartShaking()
    {
        if (canStartShaking && !startedTimer)
        {
            TimerManager.Instance.AddTimer(BreakObject, timeNeeded);
            startTime = Time.time;
        }
    }

    private void BreakObject()
    {
        // is player stil shaking
       // obstacleCollider.enabled = false;
        Debug.Log("Break");
    }
}
