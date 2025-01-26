using SpiritLevel;
using UnityEngine;
using UnityEngine.Audio;

public class Bubble : MonoBehaviour
{
    [Tooltip("The assigned Player Index for this bubble")]
    public PlayerIdentity player;

    [SerializeField, Tooltip("The rigidbody of the bubble")]
    private new Rigidbody rigidbody;

    [SerializeField, Tooltip("Amount of upwards forces is applied to the bubble each frame")]
    private float upwardsVelocity;

    [SerializeField, Tooltip("Root object of the visuals of the Bubble")]
    private Transform VisualsRoot;

    [SerializeField, Tooltip("References to the Renderers on the bubble")]
    private MeshRenderer rootRenderer, expressionRenderer;

    [SerializeField, Tooltip("All the possible face configurations for the bubble")]
    private ExpressionSet[] expressionSprites;

    [SerializeField, Tooltip("This is for visuals which is EVERYTHING except for the twerk animation.")]
    private GameObject generalVisualsGameObject;

    [SerializeField, Tooltip("The game object with the twerk animation.")]
    private GameObject twerkGameObject;

    [SerializeField]
    private AudioSource voiceAudioSource, environmentAudioSource;

    [Header("Audio")]
    [SerializeField, Tooltip("Minimum velocity needed for the impact sound to be invoked.")]
    private float minImpactMagnitude;

    [SerializeField]
    private AudioResource impactVoiceAudioResource, impactAudioResource;


    /// <summary>
    /// The current expression of the bubble
    /// </summary>
    private Expression currentExpression = Expression.Normal;

    /// <summary>
    /// Amount of time before returning to the default expression
    /// </summary>
    private float expressionTimer = 0f;

    /// <summary>
    /// Called every physics update
    /// </summary>
    private void FixedUpdate()
    {
        //Add force to the bubble
        rigidbody.AddForce(Vector3.up);
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //Update billboarding every frame
        UpdateBillboarding();

        //Update bubble position in the world based on its player index
        Shader.SetGlobalVector(string.Format("_BubblePosition{0}", player.ID), transform.position);

        if (expressionTimer > 0)
            expressionTimer = Mathf.Clamp(expressionTimer - Time.deltaTime, 0, 100);
        else if (expressionTimer <= 0 && currentExpression != Expression.Normal)
            SetExpression(Expression.Normal, 0f);
    }

    /// <summary>
    /// Updates billboarding
    /// </summary>
    private void UpdateBillboarding()
    {
        Camera mainCam = Camera.main;

        if (mainCam == null)
            return;

        VisualsRoot.transform.forward = (VisualsRoot.transform.position - mainCam.transform.position);
    }

    /// <summary>
    /// Sets the current expression on the bubble
    /// </summary>
    public void SetExpression(Expression expression, float time)
    {
        ExpressionSet? expressionSet = null;

        for (int i = 0; i < expressionSprites.Length; i++)
        {
            if (expressionSprites[i].Expression == expression)
            {
                expressionSet = expressionSprites[i];
                break;
            }
        }

        //If an expression has been found, apply its
        if (expressionSet.HasValue)
        {
            //Set expression
            expressionRenderer.material.SetTexture("_Texture2D", expressionSet.Value.ExpressionTexture);

            //Set expression timer
            expressionTimer = time;
        }
    }

    /// <summary>
    /// Executes the twerk animation on the player.
    /// </summary>
    public void ExecuteTwerk()
    {
        generalVisualsGameObject.SetActive(false);
        twerkGameObject.SetActive(true);
    }

    /// <summary>
    /// For the impact sounds.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (Game.Instance != null && Game.Instance.CurrentGameState == Game.GameState.GAMEPLAY && collision.impulse.magnitude >= minImpactMagnitude && !voiceAudioSource.isPlaying)
        {
            voiceAudioSource.resource = impactVoiceAudioResource;
            voiceAudioSource.Play();

            environmentAudioSource.resource = impactAudioResource;
            environmentAudioSource.Play();
        }
    }

    /// <summary>
    /// Makes the rigidbody kinematic.
    /// </summary>
    internal void Finish()
    {
        rigidbody.isKinematic = true;
    }

    /// <summary>
    /// Struct for holding 
    /// </summary>
    [System.Serializable]
    public struct ExpressionSet
    {
        [Tooltip("The Expression this belongs to")]
        public Expression Expression;

        [Tooltip("Sprite for the expression of the player")]
        public Texture2D ExpressionTexture;
    }

    /// <summary>
    /// Enum set for representing different expressions
    /// </summary>
    public enum Expression
    {
        Normal,
        Crying,
        Floating,
        Impact,
        Blink,
        Spinning,
    }
}
