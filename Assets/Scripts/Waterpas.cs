using NUnit.Framework;
using RUMBLE.Utilities;
using SpiritLevel.Player;
using System;
using UnityEngine;

public class Waterpas : MonoBehaviour
{
    [SerializeField, Tooltip("The rigidbody on the level")]
    private Rigidbody rigidBody;

    [SerializeField, Tooltip("Enables keyboard input in the edito")]
    private bool enableEditorInput = false;

    [SerializeField, Tooltip("Spawn positions for players")]
    internal Transform spawnPositionPlayer1, spawnPositionPlayer2;

    [SerializeField, Tooltip("Prefab of the bubble player")]
    internal Bubble bubblePrefab;

    /// <summary>
    /// Multiplier used for torque
    /// </summary>
    [SerializeField, Tooltip("Multiplier used for torque")]
    internal float torqueMultiplier;

    /// <summary>
    /// Input queried from update loop
    /// </summary>
    private Vector2 lastInput = Vector2.zero;

    /// <summary>
    /// Called on the first active frame
    /// </summary>
    private void Awake()
    {
#if UNITY_EDITOR
        if (enableEditorInput)
        {
            Bubble newBubble = Instantiate(bubblePrefab, spawnPositionPlayer1.position, spawnPositionPlayer1.rotation);
            return;
        }
#endif

        //Spawn 2 new bubbles in the tube
        for (int i = 0; i < 1; i++)
        {
            //Get spawn point for the player
            Transform spawnPoint = i == 0 ? spawnPositionPlayer1 : spawnPositionPlayer2;

            //Spawn a new bubble
            Bubble newBubble = Instantiate(bubblePrefab, spawnPoint.position, spawnPoint.rotation);
            newBubble.BubbleID = i;
        }
    }

    /// <summary>
    /// Called on each update, takes input for rotation
    /// </summary>
    private void Update()
    {
        //Stop processing input if the game hasn't started yet
        if (Game.Instance?.CurrentGameState != Game.GameState.GAMEPLAY)
            return;

        float horizontalInput = 0;
        float verticalInput = 0;

#if UNITY_EDITOR
        //Set editor keyboard data for testing
        if (enableEditorInput)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
#endif
        //Assign input vector
        lastInput = new Vector2(horizontalInput, verticalInput);
    }

    /// <summary>
    /// Determines input type of the Waterpas
    /// </summary>
    public ControlMode ActiveControlMode = ControlMode.LocalSpace;
    public enum ControlMode
    {
        LocalSpace,
        WorldSpace,
        GyroData

    }

    /// <summary>
    /// Called every fixed frame, rotates the actual level
    /// </summary>
    private void FixedUpdate()
    {
        //Stop processing input if the game hasn't started yet
        if (Game.Instance?.CurrentGameState != Game.GameState.GAMEPLAY)
            return;

        if (ActiveControlMode == ControlMode.LocalSpace)
        {
            //PC+Keyboard controls Local space
            rigidBody.AddTorque(transform.forward * lastInput.x * torqueMultiplier, ForceMode.Impulse);
            rigidBody.AddTorque(transform.right * lastInput.y * torqueMultiplier, ForceMode.Impulse);
        }
        else if (ActiveControlMode == ControlMode.WorldSpace)
        {
            //PC+Keyboard controls world space
            rigidBody.AddTorque(Vector3.forward * lastInput.x * torqueMultiplier, ForceMode.Impulse);
            rigidBody.AddTorque(Vector3.right * lastInput.y * torqueMultiplier, ForceMode.Impulse);
        }
        else if (ActiveControlMode == ControlMode.GyroData)
        {
            UpdateGyroInputDeltas();
        }
    }

    [Tooltip("Multiplier applied to all input deltas")]
    public float InputDeltaMultiplier = 2;

    [Tooltip("Minimum required delta to process a change")]
    public float MinRequiredAngleDelta = 0.1f;

    [Tooltip("Maximum clamped input angle")]
    public float MaxAngle = 2;

    [Tooltip("Multiplier applied to all input deltas")]
    public float Smoothing;

    [Tooltip("The amount of samples to store in the delta history")]
    public int deltaHistory;

    [Tooltip("Final multiplier of the output")]
    public Vector3 outputMultipliersPerAxis;

    /// <summary>
    /// Amount of deltas stored as history
    /// </summary>
    private int storedDeltaCount = 0;

    /// <summary>
    /// Delta list, ordered for frames
    /// </summary>
    public Vector3[] deltaBuffer;

    /// <summary>
    /// Internal rotation counter
    /// </summary>
    private Vector3 internalRotation;

    /// <summary>
    /// Final output multiplier for torque
    /// </summary>
    [Tooltip("Final output multiplier for torque")]
    public float TorqueMultiplier = 0.1f;

    /// <summary>
    /// Update gyro input data
    /// </summary>
    public void UpdateGyroInputDeltas()
    {
        float alpha = PlayerManager.Instance.Players[0].Input.Alpha;
        float beta = PlayerManager.Instance.Players[0].Input.Beta;
        float gamma = PlayerManager.Instance.Players[0].Input.Gamma;

        float alphaDelta = PlayerManager.Instance.Players[0].Input.Alpha - PlayerManager.Instance.Players[0].Input.previousAlphaValue;
        float betaDelta = 0; //We don't do betas
        float gammaDelta = PlayerManager.Instance.Players.Count > 1 ? (PlayerManager.Instance.Players[1].Input.Gamma - PlayerManager.Instance.Players[1].Input.previousGammaValue) : 0;

        float alphaDeltaSign = Mathf.Sign(alphaDelta);
        float betaDeltaSign = Mathf.Sign(betaDelta);
        float gammaDeltaSign = Mathf.Sign(gammaDelta);

        //If the delta is too low, make it 0
        if (Math.Abs(alphaDelta) < MinRequiredAngleDelta)
            alphaDelta = 0;
        if (Math.Abs(betaDelta) < MinRequiredAngleDelta)
            betaDelta = 0;
        if (Math.Abs(gammaDelta) < MinRequiredAngleDelta)
            gammaDelta = 0;

        if (Mathf.Abs(alphaDelta) > MaxAngle)
            alphaDelta = MaxAngle * alphaDeltaSign;
        if (Mathf.Abs(betaDelta) > MaxAngle)
            betaDelta = MaxAngle * betaDeltaSign;
        if (Mathf.Abs(gammaDelta) > MaxAngle)
            gammaDelta = MaxAngle * gammaDeltaSign;

        //Buffer the adjusted delta values
        BufferDelta(new Vector3(alphaDelta, betaDelta, gammaDelta));

        //Sample average delta from the buffer
        Vector3 averageDelta = GetAverageDelta();

        //Lock position on zero, it should never move anyway
        rigidBody.position = Vector3.zero;

        rigidBody.AddTorque(transform.right * TorqueMultiplier * (averageDelta.z * Smoothing * Time.deltaTime * InputDeltaMultiplier), ForceMode.Impulse);
        rigidBody.AddTorque(transform.forward * TorqueMultiplier * (averageDelta.x * Smoothing * Time.deltaTime * InputDeltaMultiplier), ForceMode.Impulse);
    }

    /// <summary>
    /// Returns the average delta movement from all of its history samples
    /// </summary>
    public Vector3 GetAverageDelta()
    {
        Vector3 averageDelta = Vector3.zero;

        for (int i = 0; i < storedDeltaCount; i++)
            averageDelta += deltaBuffer[i];

        return averageDelta / storedDeltaCount;
    }

    /// <summary>
    /// Buffers given input delta into the history
    /// </summary>
    /// <param name="delta"></param>
    public void BufferDelta(Vector3 delta)
    {
        if (deltaBuffer == null || deltaBuffer.Length == 0 || deltaBuffer.Length != deltaHistory)
            ResetBuffer();

        for (int i = deltaBuffer.Length - 1; i > 0; i--)
            deltaBuffer[i] = deltaBuffer[i - 1];

        storedDeltaCount = Mathf.Clamp(storedDeltaCount + 1, 0, deltaBuffer.Length);
        deltaBuffer[0] = delta;

        void ResetBuffer()
        {
            storedDeltaCount = 0;

            //Reset buffer without allocating new memory
            if (deltaBuffer == null || deltaBuffer.Length == 0 || deltaBuffer.Length != deltaHistory)
                deltaBuffer = new Vector3[deltaHistory];
            else
            {
                for (int i = 0; i < deltaHistory; i++)
                    deltaBuffer[i] = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Converts weird ass gyro data to proper rotation
    /// </summary>
    private Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
