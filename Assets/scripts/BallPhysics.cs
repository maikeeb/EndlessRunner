using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour {

    public float speed = 20;
    public Rigidbody rb;
    public Transform MainCamera;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        MainCamera = GameObject.Find("Main Camera").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector3 forward = MainCamera.forward;
        Vector3 right = MainCamera.right;
        if (Input.GetKey(KeyCode.UpArrow)){
            rb.AddForce(forward * speed);
        }
        if (Input.GetKey(KeyCode.DownArrow)){
            rb.AddForce(-forward * speed);
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            rb.AddForce(right * speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            rb.AddForce(-right * speed);
        }

	}
}
