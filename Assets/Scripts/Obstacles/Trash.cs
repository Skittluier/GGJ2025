using UnityEngine;
using UnityEngine.Timers;
using UnityEngine.UI;
using SpiritLevel.Networking;
using SpiritLevel.Player;
using System.Collections.Generic;

public class Trash : Obstacle
{
    private Bubble bubble;
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
    public float ProgressAddingValue = 0.01f;
    private GameObject shakeIndication;
    private void OnTriggerEnter(Collider collision)
    {
        var hitLayerMask = 1 << collision.gameObject.layer;
        if ((hitLayerMask & mask) == 1)
        {
            if (collision.TryGetComponent<Bubble>(out Bubble bubble))
            {
                this.bubble = bubble;
                BubbleRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                BubbleRigidbody.Sleep();
                canStartShaking = true;
                StartShaking();
            }
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
        HapticFeedbackCall();
    }

    public void LateUpdate()
    {
        if (canStartShaking)
        {
            if (bubble.player.Input.IsShaking(out float magnitude))
            {
                magnitude = Mathf.Clamp(magnitude, PlayerInput.MINIMUM_SHAKE_MAGNITUDE, PlayerInput.MAXIMUM_SHAKE_MAGNITUDE);
                progressBar.value += ProgressAddingValue * (magnitude * 0.01f);
            }
        }
    }

    private void HapticFeedbackCall()
    {
        if (bubble.player.Input.IsShaking(out float magnitude))
        {
            UnityMessage<Dictionary<string, float>> message = new UnityMessage<Dictionary<string, float>>()
            {
                type = UnityMessageType.VIBRATION_START,
                data = new Dictionary<string, float>()
            {
                { bubble.player.UUID, 250f }
            }
            };
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            PlayerManager.Instance.SendData(data);

            TimerManager.AddTimer(HapticFeedbackCall, .5f);
        }
    }

    private void BreakObject()
    {
        if (progressBar.value == 100)
        {
            progressBar.gameObject.SetActive(false);
            gameObject.SetActive(false);
            Debug.Log("Break");
            UnityMessage<string> message = new UnityMessage<string>()
            {
                type = UnityMessageType.VIBRATION_STOP,
                data = bubble.player.UUID
            };

            string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            PlayerManager.Instance.SendData(data);
            BubbleRigidbody.WakeUp();

        }
    }
}
