using NativeWebSocket;
using RUMBLE.Utilities;
using SpiritLevel.Networking;
using SpiritLevel.Player;
using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

    [Header("PID Settings")]

    /// <summary>
    /// Reference to the internal PID controller
    /// </summary>
    [SerializeField]
    internal PIDControllerAngularVelocity angularPIDController;

    /// <summary>
    /// Determines the forcemode used for applying force
    /// </summary>
    [SerializeField, Tooltip("Determines the forcemode used for applying force")]
    internal ForceMode forceMode;

    /// <summary>
    /// Multiplier used for torque
    /// </summary>
    [SerializeField, Tooltip("Multiplier used for torque")]
    internal float torqueMultiplier;

    /// <summary>
    /// Input queried from update loop
    /// </summary>
    private Vector2 lastInput = Vector2.zero;

    private Quaternion initialRotation;

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

        initialRotation = transform.rotation;

        //Attach rigidbody
        angularPIDController.AttachedRigidBody = rigidBody;

        //Loop over all players and spawn their players
        for (int i = 0; i < PlayerManager.Instance.Players.Count; i++)
        {
            //Get spawn point for the player
            Transform spawnPoint = i == 0 ? spawnPositionPlayer1 : spawnPositionPlayer2;

            //Spawn a new bubble
            Bubble newBubble = Instantiate(bubblePrefab, spawnPoint.position, spawnPoint.rotation);

            //Assign the spawned new bubble to the player
            PlayerManager.Instance.Players[i].Bubble = newBubble;

            //Stop at the second player for nows
            if (i == 1)
                return;
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


    public ControlMode ActiveControlMode = ControlMode.LocalSpace;
    public enum ControlMode
    {
        LocalSpace,
        WorldSpace,
        GyroData

    }

    /// <summary>
    /// Clamp value for the rotation delta
    /// </summary>
    public float RotationDeltaClamp = 3;

    /// <summary>
    /// Current beta delta
    /// </summary>
    private float currentBetaDelta = 0f;

    /// <summary>
    /// Current gamma delta
    /// </summary>
    private float currentGammaDelta = 0f;

    /// <summary>
    /// Current gamma delta velocity
    /// </summary>
    private float deltaGammaVelocity;

    /// <summary>
    /// Current beta delta velocity
    /// </summary>
    private float deltaBetaVelocity;

    public float Smooooooooth;
    public float DeltaMultiplier;

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
            Vector3 gyroInput = Vector3.zero;

            //Just pick the input of the first player
            if (PlayerManager.Instance.Players.Count == 1)
            {
                float gammaDelta = Mathf.Clamp(-(PlayerManager.Instance.Players[0].Input.Gamma - PlayerManager.Instance.Players[0].Input.previousGammaValue), -RotationDeltaClamp, RotationDeltaClamp) * DeltaMultiplier;

                float betaDelta = Mathf.Clamp((PlayerManager.Instance.Players[0].Input.Alpha - PlayerManager.Instance.Players[0].Input.previousAlphaValue), -RotationDeltaClamp, RotationDeltaClamp) * DeltaMultiplier;
                
                if (Mathf.Abs(currentGammaDelta) <= 0.05f)
                    currentGammaDelta = 0;

                currentBetaDelta = Mathf.SmoothDamp(currentBetaDelta, betaDelta, ref deltaBetaVelocity, Time.deltaTime * Smooooooooth);
                currentGammaDelta = Mathf.SmoothDamp(currentGammaDelta, gammaDelta, ref deltaGammaVelocity, Time.deltaTime * Smooooooooth);

                //Gyro controls for singleplayer
                gyroInput = new Vector3(currentGammaDelta, 0, currentBetaDelta);
            }
            //If there are more then 2 players, split the input across 2 different players
            else if (PlayerManager.Instance.Players.Count >= 2)
            {
                //Gyro controls for mutliplayer
                gyroInput = new Vector3(-PlayerManager.Instance.Players[0].Input.Gamma, 0, /*-PlayerManager.Instance.Players[1].Input.Beta*/0);

            }
            //Get the desired output
            Quaternion outputRotation = GyroToUnity(Quaternion.Euler(gyroInput));

            //Update error in the angular PID controller
            Vector3 diffAngles = GyroToUnity(Quaternion.Euler(gyroInput)).eulerAngles;

            //CHeck for angle flips to make sure the fastest rotation is taken instead of turning complete circles
            if (diffAngles.x >= 180)
                diffAngles.x = -(360 - diffAngles.x);

            if (diffAngles.y >= 180)
                diffAngles.y = -(360 - diffAngles.y);

            if (diffAngles.z >= 180)
                diffAngles.z = -(360 - diffAngles.z);

            //Update rotation controller error
            angularPIDController.AttachedRigidBody = rigidBody;
            angularPIDController.UpdateError(diffAngles, Time.fixedDeltaTime);

            //Add torque based on error
            rigidBody.AddTorque(transform.forward * diffAngles.x * torqueMultiplier, ForceMode.Impulse);

            //Add torque based on error
            //rigidBody.AddRelativeTorque(angularPIDController.ControlValue * Time.fixedDeltaTime * torqueMultiplier, ForceMode.Impulse);
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
