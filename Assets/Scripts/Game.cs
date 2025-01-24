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
    /// Called from the start, Starts the game right away on scene load
    /// </summary>
    private void Start()
    {
        startingTimestamp = countdownTime;
    }

    private void Update()
    {
        //If a starting timestamp is present, apply game cooldown
        if (startingTimestamp > 0)
        {
            startingTimestamp -= Time.deltaTime;

            //Apply text to countdown text
            countdownText.gameObject.SetActive(true);
            countdownText.text = string.Format("{0:D0}", startingTimestamp);

        }
    }

    private void StartGame()
    {

    }
}
