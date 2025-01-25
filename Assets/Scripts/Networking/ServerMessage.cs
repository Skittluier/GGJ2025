namespace SpiritLevel.Networking
{
    public enum ServerMessageType
    {
        PLAYER_JOINED, PLAYER_LEFT, PLAYER_INPUT
    }

    public enum UnityMessageType
    {
        VIBRATION_START, VIBRATION_STOP 
    }

    public class ServerMessage<T>
    {
        public ServerMessageType type;
        public T data;
    }

    public class ServerMessage
    {
        public ServerMessageType type;
    }

    public class UnityMessage<T>
    {
        public UnityMessageType type;
        public T data;
    }
}