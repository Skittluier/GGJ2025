using NativeWebSocket;
using SpiritLevel.Networking;
using System.Collections;
using UnityEngine;

public class Waterpas : MonoBehaviour
{
    private WebSocket webSocket;

    [SerializeField, Tooltip("The websocket URL for the controls.")]
    private string webSocketURL;

    [SerializeField, Tooltip("The rigidbody on the level")]
    private Rigidbody rigidBody;

    [SerializeField, Tooltip("The rotation strength of the level")]
    private float rotationStrength;

    private Vector2 lastInput = Vector2.zero;


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
                Debug.Log($"Alpha: {serverMsg.data[i].alpha} | Beta: {serverMsg.data[i].beta} | Gamma: {serverMsg.data[i].gamma}");
        }
        else if (sMessage.type == ServerMessageType.PLAYER_JOINED)
        {
            ServerMessage<PlayerJoinedData> playerJoinedData = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage<PlayerJoinedData>>(result);
            Debug.Log($"Player Joined: {playerJoinedData.data.uuid}");
        }
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

    //private void Update()
    //{
    //    float horizontalInput = Input.GetAxis("Horizontal");
    //    float verticalInput = Input.GetAxis("Vertical");

    //    //Assign input vector
    //    lastInput = new Vector2(horizontalInput, verticalInput);
    //}

    //private void FixedUpdate()
    //{
    //    //Get the current rotation
    //    Quaternion currentRotation = rigidBody.rotation;

    //    rigidBody.AddRelativeTorque(new Vector3(lastInput.x * rotationStrength, 0, 0), ForceMode.Impulse);
    //    rigidBody.AddRelativeTorque(new Vector3(0, 0, lastInput.y * rotationStrength), ForceMode.Impulse);
    //}
}
