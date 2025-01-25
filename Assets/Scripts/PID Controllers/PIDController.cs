namespace RUMBLE.Utilities
{
    using System;
    using UnityEngine;

    /// <summary>
    /// This class serves as basis for creating a PID controller
    /// </summary>
    /// <typeparam name="T">The Type of value to control using this PID controller</typeparam>
    [System.Serializable]
    public abstract class PIDController<T>
    {
        [SerializeField, Tooltip("The current active PID control modes"),EnumButtons]
        protected PIDControlModes activeControlModes = (PIDControlModes)0;
        /// <summary>
        /// The current active PID control modes of this Controller
        /// </summary>
        public PIDControlModes ActiveControlerModes
        {
            get
            {
                return activeControlModes;
            }
            set
            {
                activeControlModes = value;
            }
        }

        //The previous error value that was used
        protected T previousError;

        protected T controlValue;
        /// <summary>
        /// The current control value (CV) of this PID controller
        /// </summary>
        public T ControlValue => controlValue;

        protected T integral;
        /// <summary>
        /// The current integral of all errors processed in this PID controller
        /// </summary>
        public T Integral => integral;

        /// <summary>
        /// Updates the ControlValue to account for given error
        /// </summary>
        /// <param name="error">The error to account for</param>
        /// <param name="dt">The deltaTime step of the error</param>
        public virtual void UpdateError(T error, float dt)
        {
            //Add to integral
            UpdateIntegral(error, dt);

            //Set last error
            previousError = error;

            //Create new control value
            T newControlValue = default;

            //Apply P, I and D control modes
            if ((activeControlModes & PIDControlModes.Proportional) != 0)
                ProportionalControl(ref newControlValue, error, dt);

            if ((activeControlModes & PIDControlModes.Integral) != 0)
                IntegralControl(ref newControlValue, error, dt);

            if ((activeControlModes & PIDControlModes.Deravative) != 0)
                DerivativeControl(ref newControlValue, error, dt);

            //Set final control value
            controlValue = newControlValue;
        }

        /// <summary>
        /// Updates the current internal used integral value based on given error and deltaTime
        /// </summary>
        /// <param name="error">The error to add to the integral</param>
        /// <param name="dt">The deltaTime step of the error</param>
        protected abstract void UpdateIntegral(T error, float dt);

        /// <summary>
        /// Applies proportional control to the controlValue based on given error and deltaTime
        /// </summary>
        /// <param name="error">The error to solve</param>
        /// <param name="dt">Deltatime step of the error</param>
        protected abstract void ProportionalControl(ref T controlValue, T error, float dt);

        /// <summary>
        /// Applies integral control to the controlValue based on given error and deltaTime
        /// </summary>
        /// <param name="error">The error to solve</param>
        /// <param name="dt">Deltatime step of the error</param>
        protected abstract void IntegralControl(ref T controlValue, T error, float dt);

        /// <summary>
        /// Applies derivative control to the controlValue based on given error and deltaTime
        /// </summary>
        /// <param name="error">The error to solve</param>
        /// <param name="dt">Deltatime step of the error</param>
        protected abstract void DerivativeControl(ref T controlValue, T error, float dt);

        /// <summary>
        /// Resets the values of this PID controller
        /// </summary>
        [ContextMenu("Reset PID controller")]
        public virtual void Reset()
        {
            //Reset all base PID controller values
            controlValue = default;
            integral = default;
            previousError = default;
        }
    }

    /// <summary>
    /// Enum flag for controller the P,I and D control modes
    /// </summary>
    [Flags]
    public enum PIDControlModes
    {
        Proportional = 1 << 1,
        Integral = 1 << 2,
        Deravative = 1 << 3
    }
}