using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	
	public int moveSpeed;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float h = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
		float v = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
		transform.Translate(new Vector3(h,0,v), Space.World);
	}
}
