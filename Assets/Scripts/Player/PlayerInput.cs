namespace SpiritLevel.Player
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PlayerInput
    {
        private const float MINIMUM_SHAKE_MAGNITUDE = 25.0f;

        [SerializeField]
        private float previousAlphaValue;

        [SerializeField]
        private float previousBetaValue;

        [SerializeField]
        private float previousGammaValue;

        [SerializeField]
        internal float Alpha, Beta, Gamma;

        [SerializeField]
        internal Vector3 Accelerometer;

        [SerializeField]
        internal Vector3 Gyroscope;


        /// <summary>
        /// Detects with the help of the alpha, beta and gamma movement whether the player is shaking their device or not.
        /// </summary>
        /// <returns>Is the player shaking?</returns>
        internal bool IsShaking(out float shakeMagnitude)
        {
            shakeMagnitude = 0.0f;

            float possibleMaxShakeMagnitude = shakeMagnitude = Mathf.Abs(Alpha - previousAlphaValue);

            possibleMaxShakeMagnitude = Mathf.Abs(Beta - previousBetaValue);
            if (possibleMaxShakeMagnitude > shakeMagnitude)
                shakeMagnitude = possibleMaxShakeMagnitude;

            possibleMaxShakeMagnitude = Mathf.Abs(Gamma - previousGammaValue);
            if (possibleMaxShakeMagnitude > shakeMagnitude)
                shakeMagnitude = possibleMaxShakeMagnitude;

            return shakeMagnitude >= MINIMUM_SHAKE_MAGNITUDE;
        }

        /// <summary>
        /// Updates the orientation values of the player's device.
        /// </summary>
        internal void UpdateOrientationValues(float alpha, float beta, float gamma)
        {
            // First the history.
            previousAlphaValue = Alpha;
            previousBetaValue = Beta;
            previousGammaValue = Gamma;

            // Then the current values.
            Alpha = alpha;
            Beta = beta;
            Gamma = gamma;
        }
    }
}
