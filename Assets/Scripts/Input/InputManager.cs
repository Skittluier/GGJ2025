namespace SpiritLevel.Input
{
    using NativeWebSocket;
    using SpiritLevel.Networking;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Windows;

    public class InputManager : MonoBehaviour
    {
        private WebSocket webSocket;

        [SerializeField, Tooltip("The websocket URL for the controls.")]
        private string webSocketURL;

        private List<PlayerInput> inputs = new List<PlayerInput>();


        private async void Start()
        {
            webSocket = new WebSocket(webSocketURL);

            webSocket.OnOpen += WebSocket_OnOpen;
            webSocket.OnError += WebSocket_OnError;
            webSocket.OnClose += WebSocket_OnClose;
            webSocket.OnMessage += WebSocket_OnMessage;

            await webSocket.Connect();
        }

        private void Update()
        {
            if (webSocket != null)
                webSocket.DispatchMessageQueue();
        }

        private void WebSocket_OnMessage(byte[] data)
        {
            string result = System.Text.Encoding.UTF8.GetString(data);
            ServerMessage sMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage>(result);

            if (sMessage.type == ServerMessageType.PLAYER_INPUT)
            {
                ServerMessage<InputData[]> serverMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage<InputData[]>>(result);

                for (int i = 0; i < serverMsg.data.Length; i++)
                {
                    Debug.Log($"[InputManager] Alpha: {serverMsg.data[i].alpha} | Beta: {serverMsg.data[i].beta} | Gamma: {serverMsg.data[i].gamma}");
                }
            }
            else if (sMessage.type == ServerMessageType.PLAYER_JOINED || sMessage.type == ServerMessageType.PLAYER_LEFT)
            {
                ServerMessage<PlayerStatusUpdateData> playerStatusUpdateData = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage<PlayerStatusUpdateData>>(result);

                Debug.Log($"[InputManager] Player Status Update: {playerStatusUpdateData.type} | UUID: {playerStatusUpdateData.data.uuid}");

                if (sMessage.type == ServerMessageType.PLAYER_JOINED)
                    inputs.Add(new PlayerInput() { UUID = playerStatusUpdateData.data.uuid });
                else if (sMessage.type == ServerMessageType.PLAYER_LEFT && PlayerExists(playerStatusUpdateData.data.uuid, out int inputIndex))
                    inputs.RemoveAt(inputIndex);
            }
        }

        private bool PlayerExists(string uuid, out int inputIndex)
        {
            inputIndex = -1;

            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i].UUID.Equals(uuid))
                {
                    inputIndex = i;
                    return true;
                }
            }

            return false;
        }

        private void WebSocket_OnClose(WebSocketCloseCode closeCode)
        {
            Debug.Log("Websocket Closed. Code: " + closeCode);
        }

        private void WebSocket_OnOpen()
        {
            Debug.Log("Websocket Open.");
        }

        private void WebSocket_OnError(string errorMsg)
        {
            Debug.Log("Websocket Error: " + errorMsg);
        }

        private async void OnApplicationQuit()
        {
            await webSocket.Close();
        }
    }
}