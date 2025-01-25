using NativeWebSocket;
using RUMBLE.Utilities;
using SpiritLevel.Networking;
using SpiritLevel.Player;
using System.Collections;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Waterpas : MonoBehaviour
{
    [SerializeField, Tooltip("The rigidbody on the level")]
    private Rigidbody rigidBody;

    [SerializeField, Tooltip("The rotation strength of the level")]
    private float rotationStrength;

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
        //if (Game.Instance?.CurrentGameState != Game.GameState.Gameplay)
        //    return;

        float horizontalInput = 0;
        float verticalInput = 0;

#if UNITY_EDITOR
        if (!enableEditorInput)
        {
#endif
            //Just pick the input of the first player
            //if (PlayerManager.Instance.Players.Count == 1)
            //{
            horizontalInput = PlayerManager.Instance.Players[0].Input.Alpha;
            verticalInput = PlayerManager.Instance.Players[0].Input.Gamma;
            //}
            ////If there are more then 2 players, split the input across 2 different players
            //else if (PlayerManager.Instance.Players.Count >= 2)
            //{
            //    //Pick alpha value from first player
            //    horizontalInput = PlayerManager.Instance.Players[0].Input.Alpha;

            //    //Pick gamma value from player
            //    verticalInput = PlayerManager.Instance.Players[1].Input.Gamma;
            //}
#if UNITY_EDITOR
        }
#endif

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
    /// Called every fixed frame, rotates the actual level
    /// </summary>
    private void FixedUpdate()
    {
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
            //Gyro controls

        }
    }
}
