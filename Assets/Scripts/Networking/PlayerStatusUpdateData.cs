namespace SpiritLevel.Networking
{
    public class PlayerStatusUpdateData
    {
        public int id;
        public string uuid;
    }

    public class PlayerStatusUpdateReadyData
    {
        public int id;
        public string uuid;
        public bool ready;
    }
}
