using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    //Nicely exposed singleton
    public static Game Instance;

    [SerializeField, Tooltip("The max amount of time allowed for the round")]
    private float roundTime = 60f;

    [SerializeField, Tooltip("Countdown time before starting the game")]
    private float countdownTime = 3;

    [SerializeField, Tooltip("Text component to show the countdown before starting the game")]
    private TextMeshProUGUI countdownText;

    [SerializeField, Tooltip("Text component to show the current time left for the round")]
    private TextMeshProUGUI gameTimeText;

    [SerializeField, Tooltip("Text that shows that the players can start the game")]
    private TextMeshProUGUI startGameText;

    [SerializeField, Tooltip("Animator that controls the in-game UI")]
    private Animator gameUIAnimator;

    /// <summary>
    /// Timestamp for starting the game
    /// </summary>
    private float startingTimestamp = 0f;

    /// <summary>
    /// Timestamp for when the game round is over, is being set when the game start
    /// </summary>
    private float endGameTimestamp = 0f;

    /// <summary>
    /// The current GameState of the game
    /// </summary>
    internal GameState CurrentGameState = GameState.Intro;

    /// <summary>
    /// Called from the start, Starts the game right away on scene load
    /// </summary>
    private void Start()
    {
        Instance = this;

        //Set intro state
        CurrentGameState = GameState.Intro;
        startingTimestamp = countdownTime;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //If the game hasn't started, perform startup logic
        if (CurrentGameState == GameState.Intro)
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
            else if (startingTimestamp <= 0)
            {
                //send start game signal to game UI
                gameUIAnimator.SetTrigger("Start Game");

                //Disable countdown text and start the game!
                countdownText.gameObject.SetActive(false);

                //Start the game already!
                StartGame();
            }
        }
        //Perform game logic for when the game is running
        else if (CurrentGameState == GameState.Gameplay)
        {
            //Lower the current game time
            endGameTimestamp = Mathf.Clamp(endGameTimestamp - Time.deltaTime, 0, roundTime);

            //Update amount of time left in the game on the in-game UI
            gameTimeText.text = string.Format("You have: {0:0.0} seconds left!",endGameTimestamp);

            //If the gametime is getting below 0, the game round is over and the players have lost
            if(endGameTimestamp <= 0)
                LoseGame();
        }
        //Outro logic
        else if (CurrentGameState == GameState.Outro)
        {

        }
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    private void StartGame()
    {
        //Indicate that the game has started
        CurrentGameState = GameState.Gameplay;

        //Assign the round time
        endGameTimestamp = roundTime;
    }

    /// <summary>
    /// This function executes the winning sequence
    /// </summary>
    public void WinGame()
    {
        //Indicate that the game has finished
        CurrentGameState = GameState.Outro;

        //Send win signal to the Animator
        gameUIAnimator.SetTrigger("Win Game");
    }


    /// <summary>
    /// This function executes the losing sequence
    /// </summary>
    private void LoseGame()
    {
        //Indicate that the game has finished
        CurrentGameState = GameState.Outro;
    
        //Send lose signal to the Animator
        gameUIAnimator.SetTrigger("Lose Game");
    }

    /// <summary>
    /// Enum representing the current gameplay state
    /// </summary>
    public enum GameState
    {
        Intro,
        Gameplay,
        Outro,
    }
}
