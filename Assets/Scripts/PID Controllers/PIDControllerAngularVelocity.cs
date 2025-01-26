namespace RUMBLE.Utilities
{
    using UnityEngine;

    /// <summary>
    /// Implementation of a PIDController for Quaternions
    /// </summary>
    [System.Serializable]
    public class PIDControllerAngularVelocity : PIDController<Vector3>
    {
        [SerializeField, Tooltip("The proportional gain: K.p")]
        protected Vector3 proportionalGain = 0.2f * Vector3.one;
        internal Vector3 ProportionalGain
        {
            get
            {
                return proportionalGain;
            }
            set
            {
                proportionalGain = value;
            }
        }

        [SerializeField, Tooltip("The integral gain: K.i")]
        protected Vector3 integralGain = 0.05f * Vector3.one;
        internal Vector3 IntegralGain
        {
            get
            {
                return integralGain;
            }
            set
            {
                integralGain = value;
            }
        }

        [SerializeField, Tooltip("The derivative gain: K.d")]
        protected Vector3 derivativeGain = 1f * Vector3.one;
        internal Vector3 DerivativeGain
        {
            get
            {
                return derivativeGain;
            }
            set
            {
                derivativeGain = value;
            }
        }

        protected Rigidbody rigidBody;
        /// <summary>
        /// The RigidBody attached to this PID Controller
        /// </summary>
        public Rigidbody AttachedRigidBody
        {
            get
            {
                return rigidBody;
            }
            set
            {
                rigidBody = value;
            }
        }

        /// <summary>
        /// Updates the current internal used integral value based on given error and deltaTime
        /// </summary>
        /// <param name="error">The error to add to the integral</param>
        /// <param name="dt">The deltaTime step of the error</param>
        protected override void UpdateIntegral(Vector3 error, float dt)
        {
            integral += error * dt;
        }

        /// <summary>
        /// Applies proportional control to the controlValue based on given error and deltaTime
        /// </summary>
        /// <param name="error">The error to solve</param>
        /// <param name="dt">Deltatime step of the error</param>
        protected override void ProportionalControl(ref Vector3 controlValue, Vector3 error, float dt)
        {
            controlValue += Vector3.Scale(proportionalGain, error);
        }

        /// <summary>
        /// Applies integral control to the controlValue based on given error and deltaTime
        /// </summary>
        /// <param name="error">The error to solve</param>
        /// <param name="dt">Deltatime step of the error</param>
        protected override void IntegralControl(ref Vector3 controlValue, Vector3 error, float dt)
        {
            controlValue += Vector3.Scale(integralGain, integral);
        }

        /// <summary>
        /// Applies derivative control to the controlValue based on given error and deltaTime
        /// </summary>
        /// <param name="error">The error to solve</param>
        /// <param name="dt">Deltatime step of the error</param>
        protected override void DerivativeControl(ref Vector3 controlValue, Vector3 error, float dt)
        {
            //Calculate the derivative of the previous error
            Vector3 derivative = (error - previousError) / dt;

            //Transform angular velocity towards local velocity
            Vector3 ang = rigidBody.transform.worldToLocalMatrix.MultiplyVector(rigidBody.angularVelocity);

            //Adjust control value based on derivative gain
            controlValue -= Vector3.Scale(derivativeGain, derivative + ang);
        }
    }
}
