using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
            throw new Exception("Multiple players in scene");
        
        instance = this;
    }
    
    public float moveSpeed = 5f;
    
    private Vector3 input;
    private Vector3 skewedInput;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector3(h, 0, v).normalized;

        skewedInput = Quaternion.Euler(0, 45, 0) * input;

        if (input.magnitude > 0.1f)
        {
            transform.Translate(skewedInput * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
