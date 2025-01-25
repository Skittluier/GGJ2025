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
        internal List<PlayerIdentity> Players { get; private set; } = new List<PlayerIdentity>();

        internal delegate void OnPlayerJoinedMethod(PlayerIdentity player);
        internal OnPlayerJoinedMethod OnPlayerJoined;

        internal delegate void OnPlayerLeftMethod(string playerUUID);
        internal OnPlayerLeftMethod OnPlayerLeft;

        internal delegate void OnRoomCodeUpdatedMethod(string roomCode);
        internal OnRoomCodeUpdatedMethod OnRoomCodeUpdated;

        private bool tryingToConnect = false;


        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            webSocket = new WebSocket(webSocketURL);

            webSocket.OnOpen += WebSocket_OnOpen;
            webSocket.OnError += WebSocket_OnError;
            webSocket.OnClose += WebSocket_OnClose;
            webSocket.OnMessage += WebSocket_OnMessage;
        }

        private void Update()
        {
            if (webSocket != null)
            {
                webSocket.DispatchMessageQueue();

                if (webSocket.State != WebSocketState.Open && webSocket.State != WebSocketState.Connecting)
                    ConnectToWebServer();
            }
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

                        Players[i].Input.UpdateOrientationValues(serverMsg.data[j].alpha, serverMsg.data[j].beta, serverMsg.data[j].gamma);
                        Players[i].Input.Accelerometer = new Vector3(serverMsg.data[j].accX, serverMsg.data[j].accY, serverMsg.data[j].accZ);
                        Players[i].Input.Gyroscope = new Vector3(serverMsg.data[j].gyroX, serverMsg.data[j].gyroY, serverMsg.data[j].gyroZ);
                    }
                }
            }
            else if (sMessage.type == ServerMessageType.PLAYER_JOINED || sMessage.type == ServerMessageType.PLAYER_LEFT)
            {
                ServerMessage<PlayerStatusUpdateData> playerStatusUpdateData = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage<PlayerStatusUpdateData>>(result);

                Debug.Log($"[PlayerManager] Player Status Update: {playerStatusUpdateData.type} | ID: {playerStatusUpdateData.data.id} | UUID: {playerStatusUpdateData.data.uuid}");

                if (sMessage.type == ServerMessageType.PLAYER_JOINED)
                {
                    PlayerIdentity player = new PlayerIdentity() { UUID = playerStatusUpdateData.data.uuid, ID = playerStatusUpdateData.data.id };
                    Players.Add(player);

                    OnPlayerJoined?.Invoke(player);
                }
                else if (sMessage.type == ServerMessageType.PLAYER_LEFT && PlayerExists(playerStatusUpdateData.data.uuid, out int inputIndex))
                {
                    Players.RemoveAt(inputIndex);
                    OnPlayerLeft?.Invoke(playerStatusUpdateData.data.uuid);
                }
            }
            else if (sMessage.type == ServerMessageType.PLAYER_READY)
            {
                ServerMessage<PlayerStatusUpdateReadyData> serverMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage<PlayerStatusUpdateReadyData>>(result);

                Debug.Log("[PlayerManager] Player Ready changed. UUID: " + serverMsg.data.uuid);

                for (int i = 0; i < Players.Count; i++)
                {
                    //Check which player sent the message
                    if (Players[i].UUID == serverMsg.data.uuid)
                        Players[i].IsReady = serverMsg.data.ready;
                }
            }
            else if (sMessage.type == ServerMessageType.ROOM_CREATE)
            {
                ServerMessage<string> serverMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage<string>>(result);
                Debug.Log("[PlayerManager] Room Create received. Code: " + serverMsg.data);

                OnRoomCodeUpdated?.Invoke(serverMsg.data);
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

        public void SendData(string data)
        {
            webSocket.SendText(data);
        }

        public void SendUnityMessage(string data)
        {
            webSocket.SendText(data);
        }

        private void WebSocket_OnClose(WebSocketCloseCode closeCode)
        {
            Debug.Log("[PlayerManager] Websocket Closed. Code: " + closeCode);
            Players.Clear();
        }

        private void WebSocket_OnOpen()
        {
            Debug.Log("[PlayerManager] Websocket Open.");
        }

        private void WebSocket_OnError(string errorMsg)
        {
            Debug.Log("[PlayerManager] Websocket Error: " + errorMsg);
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
            Debug.Log("[PlayerManager] Trying to connect...");
            await webSocket.Connect();
            await Task.Delay(1000);
            Debug.Log("[PlayerManager] Done trying to connect.");

            tryingToConnect = false;
        }

        private async void OnApplicationQuit()
        {
            if (webSocket != null)
                await webSocket.Close();
        }
    }
}