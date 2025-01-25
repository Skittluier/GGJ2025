namespace SpiritLevel
{
    using SpiritLevel.Input;
    using System;

    [Serializable]
    public class Player
    {
        public string UUID;
        public PlayerInput Input = new PlayerInput();
    }
}