namespace SpiritLevel.Player
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PlayerInput
    {
        public float Alpha;
        public float Beta;
        public float Gamma;
        public Vector3 Accelerometer;
        public Vector3 Gyroscope;
    }
}
