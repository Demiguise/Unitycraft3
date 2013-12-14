using UnityEngine;
using System.Collections;

public class NavNodeDebuggingHelper : MonoBehaviour {

    public float castRayLength;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(transform.position, (transform.forward.normalized * castRayLength), Color.black, 0);
	}
}
