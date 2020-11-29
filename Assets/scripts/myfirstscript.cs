using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myfirstscript : MonoBehaviour {

    // whenever an object is created for the first time
    void Awake() {
        Debug.Log("awake");
    }

	// Use this for initialization
	void Start() {
        Debug.Log("start");
	}
	
	// Update is called once per frame
	void Update() {
        Debug.Log("update");
	}

    // whenever the object is turned on
    void OnEnable() {
        Debug.Log("Enable");
    }

    // whenever the object is turned off
    void OnDisable() {
        Debug.Log("Disable");
    }

    // whenever the physics engine checks the physics
    void FixedUpdate() {
        Debug.Log("FixedUpdate");
    }

    // whenever every other update is finished

    void LateUpdate() {
        Debug.Log("LateUpdate");
    }
}
