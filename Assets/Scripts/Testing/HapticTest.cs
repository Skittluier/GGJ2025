using SpiritLevel.Networking;
using SpiritLevel.Player;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

public class HapticTest : MonoBehaviour
{
    public void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            HapticTestMessage();
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            HapticEndTestMessage();
        }

    }

    [ContextMenu("Start")]
    public void HapticTestMessage()
    {
        UnityMessage<Dictionary<string, string>> message = new UnityMessage<Dictionary<string, string>>()
        {
            type = UnityMessageType.VIBRATION_START,
            data = new Dictionary<string, string>()
            {
                { "uuid",PlayerManager.Instance.Players[0].UUID},
                { "length","5000"}
            }
        };
        string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
        PlayerManager.Instance.SendData(data);
    }
    [ContextMenu("End")]
    public void HapticEndTestMessage()
    {
        UnityMessage<string> message = new UnityMessage<string>()
        {
            type = UnityMessageType.VIBRATION_STOP,
            data = PlayerManager.Instance.Players[0].UUID
        };

        string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
        PlayerManager.Instance.SendData(data);
    }
}
