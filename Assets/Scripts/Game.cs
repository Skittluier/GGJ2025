using SpiritLevel;
using SpiritLevel.Networking;
using SpiritLevel.Player;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

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

    private bool isPlayingOutro = false;

    [SerializeField, Tooltip("A reference to the main camera's animator.")]
    private Animator mainCameraAnimator;

    private int amountOfPlayersFinished;

    [Header("Audio")]
    [SerializeField, Tooltip("The voice being played whenever the bubble is free.")]
    private AudioResource bubbleVictoryAudioResource;

    [SerializeField, Tooltip("The voice being played whenever the bubble loses.")]
    private AudioResource bubbleLostAudioResource;

    [SerializeField, Tooltip("Music audio source")]
    private AudioSource musicAudioSource;

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
    internal GameState CurrentGameState = GameState.INTRO;

    private bool lostGame = false;


    /// <summary>
    /// Called from the start, Starts the game right away on scene load
    /// </summary>
    private void Start()
    {
        Instance = this;

        //Set intro state
        UpdateGameState(GameState.INTRO);
    }

    /// <summary>
    /// Starts the countdown sequence for starting the game
    /// </summary>
    public void StartCountdownSequence()
    {
        //Set countdown state
        UpdateGameState(GameState.COUNTDOWN);

        //Set countdown timestamp
        startingTimestamp = countdownTime;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //If the game hasn't started, perform startup logic
        if (CurrentGameState == GameState.COUNTDOWN)
        {
            if (!musicAudioSource.isPlaying)
                musicAudioSource.Play();

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
        else if (CurrentGameState == GameState.GAMEPLAY)
        {
            //Lower the current game time
            endGameTimestamp = Mathf.Clamp(endGameTimestamp - Time.deltaTime, 0, roundTime);

            //Update amount of time left in the game on the in-game UI
            gameTimeText.text = string.Format("You have: {0:0.0} seconds left!", endGameTimestamp);

            //If the gametime is getting below 0, the game round is over and the players have lost
            if (endGameTimestamp <= 0)
                LoseGame();
        }
        //Outro logic
        else if (CurrentGameState == GameState.OUTRO)
        {
            if (musicAudioSource.isPlaying)
                musicAudioSource.Stop();

            if (!isPlayingOutro)
            {
                isPlayingOutro = true;

                mainCameraAnimator.enabled = false;

                GlobalAudio.Instance.PlayAudioResource(lostGame ? bubbleLostAudioResource : bubbleVictoryAudioResource);
                CameraHandler.Instance.FocusOnPlayer();

                Bubble firstPlayerBubble = PlayerManager.Instance?.Players[0]?.Bubble;

                if (!firstPlayerBubble)
                    return;

                StartCoroutine(DoOutroAnimation());
                IEnumerator DoOutroAnimation()
                {
                    if (!lostGame)
                    {
                        firstPlayerBubble.SetExpression(Bubble.Expression.Normal, 4.5f);
                        yield return new WaitForSeconds(4.5f);

                        firstPlayerBubble.ExecuteTwerk();
                    }
                    else
                    {
                        float expressionLength = 2;
                        firstPlayerBubble.SetExpression(Bubble.Expression.Impact, expressionLength);

                        float currValue = 0;
                        Vector3 fromScale = firstPlayerBubble.transform.localScale;
                        Vector3 toScale = Vector3.zero;

                        while (currValue < 1)
                        {
                            currValue += Time.deltaTime / expressionLength;
                            firstPlayerBubble.transform.localScale = Vector3.Lerp(fromScale, toScale, currValue);

                            if (currValue >= 1)
                                firstPlayerBubble.transform.localScale = toScale;

                            yield return null;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    private void StartGame()
    {
        //Indicate that the game has started
        UpdateGameState(GameState.GAMEPLAY);

        //Assign the round time
        endGameTimestamp = roundTime;
    }

    /// <summary>
    /// Registers a player win. When it's equal to the player count, then win the game.
    /// </summary>
    internal void PlayerWin()
    {
        amountOfPlayersFinished++;

        if (amountOfPlayersFinished >= PlayerManager.Instance.Players.Count)
            WinGame();
    }

    /// <summary>
    /// This function executes the winning sequence
    /// </summary>
    [ContextMenu("Win Game")]
    private void WinGame()
    {
        //Indicate that the game has finished
        UpdateGameState(GameState.OUTRO);

        //Send win signal to the Animator
        gameUIAnimator.SetTrigger("Win Game");
    }


    /// <summary>
    /// This function executes the losing sequence
    /// </summary>
    [ContextMenu("Lose Game")]
    private void LoseGame()
    {
        //Indicate that the game has finished
        UpdateGameState(GameState.OUTRO);
        lostGame = true;

        //Send lose signal to the Animator
        gameUIAnimator.SetTrigger("Lose Game");
    }

    private void UpdateGameState(GameState newState)
    {
        UnityMessage<int> unityMessage = new UnityMessage<int>();
        unityMessage.type = UnityMessageType.GAME_STATE_UPDATE;
        unityMessage.data = (int)newState;

        PlayerManager.Instance?.SendData(Newtonsoft.Json.JsonConvert.SerializeObject(unityMessage));
        CurrentGameState = newState;
    }

    /// <summary>
    /// Enum representing the current gameplay state
    /// </summary>
    public enum GameState
    {
        INTRO,
        COUNTDOWN,
        GAMEPLAY,
        OUTRO,
    }
}
