namespace SpiritLevel.Player
{
    using NativeWebSocket;
    using SpiritLevel.Networking;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Collections;
    using UnityEngine;

    public class PlayerManager : Singleton<PlayerManager>
    {
        private WebSocket webSocket;

        [SerializeField, Tooltip("The websocket URL for the controls.")]
        private string webSocketURL;

        [field: SerializeField, ReadOnly]
        internal List<Player> Players { get; private set; } = new List<Player>();

        internal delegate void OnPlayerJoinedMethod(Player player);
        internal OnPlayerJoinedMethod OnPlayerJoined;

        internal delegate void OnPlayerLeftMethod(string playerUUID);
        internal OnPlayerLeftMethod OnPlayerLeft;

        private bool tryingToConnect = false;


        private void Start()
        {
            webSocket = new WebSocket(webSocketURL);

            webSocket.OnOpen += WebSocket_OnOpen;
            webSocket.OnError += WebSocket_OnError;
            webSocket.OnClose += WebSocket_OnClose;
            webSocket.OnMessage += WebSocket_OnMessage;
        }

        private void Update()
        {
            if (webSocket != null)
                webSocket.DispatchMessageQueue();

            if (webSocket.State != WebSocketState.Open && webSocket.State != WebSocketState.Connecting)
                ConnectToWebServer();
        }

        private void WebSocket_OnMessage(byte[] data)
        {
            string result = System.Text.Encoding.UTF8.GetString(data);
            ServerMessage sMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage>(result);

            if (sMessage.type == ServerMessageType.PLAYER_INPUT)
            {
                ServerMessage<InputData[]> serverMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage<InputData[]>>(result);

                // Processing all player inputs.
                for (int i = 0; i < Players.Count; i++)
                {
                    for (int j = 0; j < serverMsg.data.Length; j++)
                    {
                        if (!Players[i].UUID.Equals(serverMsg.data[j].uuid))
                            continue;

                        Players[i].Input.Alpha = serverMsg.data[j].alpha;
                        Players[i].Input.Beta = serverMsg.data[j].beta;
                        Players[i].Input.Gamma = serverMsg.data[j].gamma;
                    }
                }
            }
            else if (sMessage.type == ServerMessageType.PLAYER_JOINED || sMessage.type == ServerMessageType.PLAYER_LEFT)
            {
                ServerMessage<PlayerStatusUpdateData> playerStatusUpdateData = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage<PlayerStatusUpdateData>>(result);

                Debug.Log($"[InputManager] Player Status Update: {playerStatusUpdateData.type} | UUID: {playerStatusUpdateData.data.uuid}");

                if (sMessage.type == ServerMessageType.PLAYER_JOINED)
                {
                    Player player = new Player() { UUID = playerStatusUpdateData.data.uuid };
                    Players.Add(player);

                    OnPlayerJoined?.Invoke(player);
                }
                else if (sMessage.type == ServerMessageType.PLAYER_LEFT && PlayerExists(playerStatusUpdateData.data.uuid, out int inputIndex))
                {
                    Players.RemoveAt(inputIndex);
                    OnPlayerLeft?.Invoke(playerStatusUpdateData.data.uuid);
                }
            }
        }

        private bool PlayerExists(string uuid, out int inputIndex)
        {
            inputIndex = -1;

            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].UUID.Equals(uuid))
                {
                    inputIndex = i;
                    return true;
                }
            }

            return false;
        }
        
        public void SendHapticFeedback<T>(string uuid, byte[] data )
        {

        }

        private void WebSocket_OnClose(WebSocketCloseCode closeCode)
        {
            Debug.Log("[InputManager] Websocket Closed. Code: " + closeCode);
        }

        private void WebSocket_OnOpen()
        {
            Debug.Log("[InputManager] Websocket Open.");
        }

        private void WebSocket_OnError(string errorMsg)
        {
            Debug.Log("[InputManager] Websocket Error: " + errorMsg);
        }

        private void ConnectToWebServer()
        {
            if (!tryingToConnect)
            {
                tryingToConnect = true;
                Task.Run(ConnectToWebSocket);
            }
        }

        private async void ConnectToWebSocket()
        {
            Debug.Log("[InputManager] Trying to connect...");
            await webSocket.Connect();
            Debug.Log("[InputManager] Done trying to connect.");

            tryingToConnect = false;
        }

        private async void OnApplicationQuit()
        {
            await webSocket.Close();
        }
    }
}