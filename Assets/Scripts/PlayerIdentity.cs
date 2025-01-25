namespace SpiritLevel
{
    using SpiritLevel.Player;
    using System;

    [Serializable]
    public class PlayerIdentity
    {
        public int ID;
        public string UUID;
        public PlayerInput Input = new PlayerInput();
        public Bubble Bubble;
    }
}