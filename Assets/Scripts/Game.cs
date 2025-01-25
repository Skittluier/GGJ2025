using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField, Tooltip("The max amount of time allowed for the round")]
    private float roundTime = 60f;

    [SerializeField, Tooltip("Countdown time before starting the game")]
    private float countdownTime = 3;

    [SerializeField, Tooltip("Text component to show the countdown before starting the game")]
    private TextMeshProUGUI countdownText;

    /// <summary>
    /// Timestamp for starting the game
    /// </summary>
    private float startingTimestamp = 0f;

    /// <summary>
    /// Boolean to show if the game has started
    /// </summary>
    private bool gameStarted = false;

    /// <summary>
    /// Called from the start, Starts the game right away on scene load
    /// </summary>
    private void Start()
    {
        gameStarted = false;
        startingTimestamp = countdownTime;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //If a starting timestamp is present, apply game cooldown
        if (startingTimestamp > 0)
        {
            //Sustract time yo
            startingTimestamp = Mathf.Clamp(startingTimestamp - Time.deltaTime, 0, countdownTime);

            //Apply text to countdown text
            countdownText.gameObject.SetActive(true);
            countdownText.text = string.Format("Starting game in: {0:0.0} seconds!", startingTimestamp);
        }
        else if (startingTimestamp <= 0 && !gameStarted)
        {
            //Disable countdown text and start the game!
            countdownText.gameObject.SetActive(false);

            //Start the game already!
            StartGame();
        }
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    private void StartGame()
    {
        //Indicate that the game has started
        gameStarted = true;
    }
}
