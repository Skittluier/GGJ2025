namespace SpiritLevel.Networking
{
    public enum ServerMessageType
    {
        PLAYER_JOINED, PLAYER_INPUT
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
}