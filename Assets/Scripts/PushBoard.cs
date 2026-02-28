using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBoard : MonoBehaviour
{
    public float speed = 1f;
    public float distance = 2f;
    private Rigidbody rb;
    private Vector3 startPos;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance);
        Vector3 targetPos = startPos + transform.forward * offset;
        rb.MovePosition(targetPos);
    }
}
