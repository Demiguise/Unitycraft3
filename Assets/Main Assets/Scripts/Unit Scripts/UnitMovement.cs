using UnityEngine;
using System.Collections;


public class UnitMovement : MonoBehaviour {
	
	public System.Collections.Generic.Dictionary<string, float> moveModifiers = new System.Collections.Generic.Dictionary<string, float>();
	
	public Vector3 bufferZone;
	private Vector3 destination;
	public float moveSpeed;
	public float originalMoveSpeed;
	[System.NonSerialized]
	public bool moving;
	
	
	// Use this for initialization
	void Start () {
		moving = false;
		originalMoveSpeed = moveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		moveSpeed = originalMoveSpeed * CalcMoveModifier();
		if (moving){
			if (DestinationCheck()){
				//Debug.Log("I've arrived at my position! (I think)");
				moving = false;
			}
			float step = moveSpeed * Time.deltaTime;
			//Debug.Log(Vector3.MoveTowards(transform.position, requestedLocation, step));
			transform.position = Vector3.MoveTowards(transform.position, destination, step);
	
		}
	}
	
	public void StopMove () {
		moving = false;
	}
	
	void Move (Vector3 moveLocation){
		moving = true;
		destination = moveLocation;
		//Debug.Log("I've been asked to move to: " + moveLocation);
	}
	
	float CalcMoveModifier () {
		float curMod = 1f;
		foreach (float modifier in moveModifiers.Values) {
			curMod *= (1 - modifier);
		}
		return curMod;
	}
	
	bool DestinationCheck(){
		Vector3 vectorToGoal = destination - transform.position;
		if (bufferZone.magnitude > vectorToGoal.magnitude){
			return true;			
		}
		return false;
	}
}
