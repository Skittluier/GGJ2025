using SpiritLevel;
using SpiritLevel.Networking;
using SpiritLevel.Player;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //Nicely exposed singleton
    public static Game Instance;

    [SerializeField, Tooltip("The max amount of time allowed for the round")]
    private float roundTime = 60f;

    [SerializeField, Tooltip("Countdown time before starting the game")]
    private float countdownTime = 3;

    [SerializeField, Tooltip("Text that shows that the players can start the game")]
    private TextMeshProUGUI startGameText;

    [SerializeField, Tooltip("Animator that controls the in-game UI")]
    private Animator gameUIAnimator;

    private bool isPlayingOutro = false;

    [SerializeField, Tooltip("A reference to the main camera's animator.")]
    private Animator mainCameraAnimator;

    private int amountOfPlayersFinished;

    [Header("Countdown")]
    [SerializeField, Tooltip("Text component to show the countdown before starting the game")]
    private GameObject countdownParentGameObject;

    [SerializeField]
    private GameObject[] countdownObjects;

    [Header("Timer")]
    [SerializeField]
    private GameObject timerGameObject;

    [SerializeField, Tooltip("Text component to show the current time left for the round")]
    private TMP_Text gameTimeText;

    /// <summary>
    /// Timestamp for when the game round is over, is being set when the game start
    /// </summary>
    private float endGameTimestamp = 0f;
    private float startingTimestamp;

    [SerializeField]
    private Image timerFillImage;

    [SerializeField]
    private TMP_Text timerText;

    [Header("Audio")]
    [SerializeField, Tooltip("The voice being played whenever the bubble is free.")]
    private AudioResource bubbleVictoryAudioResource;

    [SerializeField, Tooltip("The voice being played whenever the bubble loses.")]
    private AudioResource bubbleLostAudioResource;

    [SerializeField, Tooltip("Music audio source")]
    private AudioSource musicAudioSource;

    /// <summary>
    /// The current GameState of the game
    /// </summary>
    internal GameState CurrentGameState = GameState.INTRO;

    private bool isCountingDown = false;
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
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //If the game hasn't started, perform startup logic
        if (CurrentGameState == GameState.COUNTDOWN)
        {
            if (!isCountingDown)
            {
                isCountingDown = true;

                if (!musicAudioSource.isPlaying)
                    musicAudioSource.Play();

                StartCoroutine(DoCountdown());
                IEnumerator DoCountdown()
                {
                    SetCountdownNumber(3);
                    yield return new WaitForSeconds(1);
                    SetCountdownNumber(2);
                    yield return new WaitForSeconds(1);
                    SetCountdownNumber(1);
                    yield return new WaitForSeconds(1);
                    SetCountdownNumber(0);

                    //send start game signal to game UI
                    gameUIAnimator.SetTrigger("Start Game");

                    //Start the game already!
                    StartGame();

                    yield return new WaitForSeconds(1);

                    //Disable countdown text and start the game!
                    countdownParentGameObject.gameObject.SetActive(false);
                }

                void SetCountdownNumber(int countdownNumber)
                {
                    for (int i = 0; i < countdownObjects.Length; i++)
                        countdownObjects[i].SetActive(countdownNumber == i);
                }
            }
        }
        //Perform game logic for when the game is running
        else if (CurrentGameState == GameState.GAMEPLAY)
        {
            //Set countdown timestamp and activate everything once.
            if (endGameTimestamp == 0)
            {
                startingTimestamp = Time.time;
                endGameTimestamp = startingTimestamp + roundTime;
                timerGameObject.SetActive(true);
            }

            //Update amount of time left in the game on the in-game UI
            float timePlaying = Time.time - startingTimestamp;

            if (timePlaying > 0)
                timerFillImage.fillAmount = 1 - (timePlaying / roundTime);

            gameTimeText.text = string.Format("{0:0.0}", endGameTimestamp - Time.time);

            //If the gametime is getting below 0, the game round is over and the players have lost
            if (endGameTimestamp - Time.time <= 0)
                LoseGame();
        }
        //Outro logic
        else if (CurrentGameState == GameState.OUTRO)
        {
            timerGameObject.SetActive(false);

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
