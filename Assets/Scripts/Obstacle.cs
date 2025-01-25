using UnityEngine;
using UnityEngine.Timers;

public class Obstacle : MonoBehaviour
{
    internal TimerManager TimerManager;

    public void Awake()
    {
        TimerManager = TimerManager.Instance;
    }

}
