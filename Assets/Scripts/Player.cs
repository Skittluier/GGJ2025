namespace SpiritLevel
{
    using SpiritLevel.Player;
    using System;

    [Serializable]
    public class Player
    {
        public string UUID;
        public PlayerInput Input = new PlayerInput();
    }
}