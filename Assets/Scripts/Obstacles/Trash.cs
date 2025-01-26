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
    private float startTime;
    [SerializeField]
    private float timeNeeded;
    [SerializeField]
    private float ShakeAmount;
    private bool canStartShaking;
    private bool startedTimer;
    public Slider progressBar;
    public float ProgressAddingValue = 0.01f;
    [SerializeField]
    private GameObject shakeIndication;

    [SerializeField]
    private Sprite[] trashSprites;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private void OnTriggerEnter(Collider collision)
    {
        var hitLayerMask = 1 << collision.gameObject.layer;
        if ((hitLayerMask & mask) > 0)
        {
            if (collision.TryGetComponent<Bubble>(out Bubble bubble))
            {

                this.bubble = bubble;
                BubbleRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                BubbleRigidbody.angularVelocity = Vector3.zero;
                BubbleRigidbody.linearVelocity = Vector3.zero;
                bubble.RigidBodyIsSleeping = true;
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
            {
                Debug.LogError("No bubble PLOX help");
            }
            canStartShaking = false;
        }
    }

    public void StartShaking()
    {
        startTime = Time.time;
        progressBar.value = 0;
        progressBar.gameObject.SetActive(true);
        shakeIndication.SetActive(true);
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

                if (progressBar.value > .95f)
                {
                    BreakObject();
                }
            }

            if(progressBar.value > 0 && progressBar.value <= 0.2) { }
            if(progressBar.value > 0.2 && progressBar.value <= 0.4) { }
            if(progressBar.value > 0.4 && progressBar.value <= 0.6) { }
            if(progressBar.value > 0.6 && progressBar.value <= 0.8) { }
            if(progressBar.value > 0.8) { }
        }
    }

    private void HapticFeedbackCall()
    {
        Debug.Log(bubble);
        Debug.Log(bubble.player);
        Debug.Log(bubble.player.UUID);
        UnityMessage<Dictionary<string, string>> message = new UnityMessage<Dictionary<string, string>>()
        {
            type = UnityMessageType.VIBRATION_START,
            data = new Dictionary<string, string>()
                {
                    { "length", "250" },
                }
        };

        string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
        PlayerManager.Instance.SendData(data);
    }

    private void BreakObject()
    {
        progressBar.gameObject.SetActive(false);
        shakeIndication.SetActive(false);
        bubble.RigidBodyIsSleeping = false;
        UnityMessage<Dictionary<string, string>> message = new UnityMessage<Dictionary<string, string>>()
        {
            type = UnityMessageType.VIBRATION_STOP,

        };

        string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
        PlayerManager.Instance.SendData(data);
        BubbleRigidbody.WakeUp();
        gameObject.SetActive(false);

    }
}
