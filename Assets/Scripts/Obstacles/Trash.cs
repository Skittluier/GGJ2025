using UnityEngine;
using UnityEngine.Timers;
using UnityEngine.UI;
using SpiritLevel.Networking;
using SpiritLevel.Player;

public class Trash : Obstacle
{
    private Rigidbody BubbleRigidbody;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private float startTime;
    [SerializeField]
    private float timeNeeded;
    [SerializeField]
    private Collider obstacleCollider;
    [SerializeField]
    private float ShakeAmount;
    private bool canStartShaking;
    private bool startedTimer;
    public Slider progressBar;
    private void OnTriggerEnter(Collider collision)
    {
        var hitLayerMask = 1 << collision.gameObject.layer;
        if ((hitLayerMask & mask) == 1)
        {
            BubbleRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (BubbleRigidbody == null)
                Debug.LogError("No bubble");
            BubbleRigidbody.Sleep();
            canStartShaking = true;
            StartShaking();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        var hitLayerMask = 1 << collision.gameObject.layer;
        if ((hitLayerMask & mask) == 0)
        {
            if (BubbleRigidbody == null)
                Debug.LogError("No bubble PLOX help");
            canStartShaking = false;
        }
    }

    public void StartShaking()
    {
        Debug.Log("StartShaking");
        TimerManager.AddTimer(BreakObject, timeNeeded);
        startTime = Time.time;
        progressBar.value = 0;
        progressBar.gameObject.SetActive(true);
        UnityMessage<object> message = new UnityMessage<object>()
        {
            type = UnityMessageType.VIBRATION_START,
            data = 5000f
        };
        object data = Newtonsoft.Json.JsonConvert.SerializeObject(message);

        //InputManager.Instance.SendHapticFeedback(data);
    }

    public void Update()
    {
        if (canStartShaking)
        {
            if (StillShaking())
            {
                progressBar.value += 0.1f;
            }
        }
    }

    public bool StillShaking()
    {
        // if mobile still shaking
        return true;
    }

    private void BreakObject()
    {
        if (progressBar.value == 100)
        {
            progressBar.gameObject.SetActive(false);
            gameObject.SetActive(false);
            Debug.Log("Break");
        }
    }
}
