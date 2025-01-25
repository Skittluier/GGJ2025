using NativeWebSocket;
using SpiritLevel.Networking;
using System.Collections;
using UnityEngine;

public class Waterpas : MonoBehaviour
{
    [SerializeField, Tooltip("The rigidbody on the level")]
    private Rigidbody rigidBody;

    [SerializeField, Tooltip("The rotation strength of the level")]
    private float rotationStrength;

    private Vector2 lastInput = Vector2.zero;


    //private void Update()
    //{
    //    float horizontalInput = Input.GetAxis("Horizontal");
    //    float verticalInput = Input.GetAxis("Vertical");

    //    //Assign input vector
    //    lastInput = new Vector2(horizontalInput, verticalInput);
    //}

    //private void FixedUpdate()
    //{
    //    //Get the current rotation
    //    Quaternion currentRotation = rigidBody.rotation;

    //    rigidBody.AddRelativeTorque(new Vector3(lastInput.x * rotationStrength, 0, 0), ForceMode.Impulse);
    //    rigidBody.AddRelativeTorque(new Vector3(0, 0, lastInput.y * rotationStrength), ForceMode.Impulse);
    //}
}
