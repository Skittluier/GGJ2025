using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Shake : MonoBehaviour
{
    private Vector2 startingPos;
    [SerializeField]
    private float xAmount;
    [SerializeField]
    private float yAmount;

    [SerializeField]
    private float speed;

    private float[] amounts;

    void Awake()
    {
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector2(
            startingPos.x + (Mathf.Sin(Time.time * speed) * xAmount), 
            startingPos.y + (Mathf.Cos(Time.time * speed) * yAmount));

    }
}