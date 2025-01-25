using UnityEngine;
using UnityEngine.Timers;
using UnityEngine.UI;
using SpiritLevel.Networking;
using SpiritLevel.Player;
using System.Collections.Generic;
using UnityEditor.VersionControl;

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
        UnityMessage<Dictionary<string, float>> message = new UnityMessage<Dictionary<string, float>>()
        {
            type = UnityMessageType.VIBRATION_START,
            data = new Dictionary<string, float>()
            {
                { bubble.player.UUID, 5000f }
            }
        };
        string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
        PlayerManager.Instance.SendData(data);
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
            UnityMessage<string> message = new UnityMessage<string>()
            {
                type = UnityMessageType.VIBRATION_STOP,
                data = bubble.player.UUID
            };

            string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            PlayerManager.Instance.SendData(data);

        }
    }
}
