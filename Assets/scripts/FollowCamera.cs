using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public GameObject Player;
    public float zdistance;
    public float ydistance;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if(Input.GetKey(KeyCode.A)){
            transform.Rotate(Vector3.up,Time.deltaTime * 180);
        }

          if(Input.GetKey(KeyCode.D)){
            transform.Rotate(Vector3.up,Time.deltaTime * -180);
        }

        transform.position = Player.transform.position - transform.forward * zdistance;
        transform.Translate(Vector3.up * ydistance);

	}
}
