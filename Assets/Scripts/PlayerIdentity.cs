namespace SpiritLevel
{
    using SpiritLevel.Player;
    using System;

    [Serializable]
    public class PlayerIdentity
    {
        public int PlayerIndex;
        public string UUID;
        public PlayerInput Input = new PlayerInput();
    }
}