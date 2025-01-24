using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField,Tooltip("The rigidbody of the bubble")]
    private new Rigidbody rigidbody;

    [SerializeField,Tooltip("Amount of upwards forces is applied to the bubble each frame")]
    private float upwardsVelocity;

    
    private void FixedUpdate()
    {
        //Add force to the bubble
        rigidbody.AddForce(Vector3.up);
    }
}
