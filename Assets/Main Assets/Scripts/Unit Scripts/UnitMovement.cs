using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitMovement : MonoBehaviour {
	
	public System.Collections.Generic.Dictionary<string, float> moveModifiers = new System.Collections.Generic.Dictionary<string, float>();
	
	public Vector3 bufferZone;
	private Vector3 destination;
	public float moveSpeed;
	public float originalMoveSpeed;
	[System.NonSerialized]
	public bool moving;

	private List<Vector3> VectorList = new List<Vector3>();
	
	
	// Use this for initialization
	void Start () {
		moving = false;
		originalMoveSpeed = moveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		moveSpeed = originalMoveSpeed * CalcMoveModifier();
		ChangeDirection();
		if (moving){
			if ((DestinationCheck()) && (VectorList.Count == 0)){
				moving = false;
			}
			if ((DestinationCheck()) && (VectorList.Count > 0)){
				VectorList.Remove(VectorList[0]);
				if (VectorList.Count != 0) {
					destination = SetDestination(VectorList[0]);
				}
			}
			float step = moveSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(this.transform.position, destination, step);
		}
	}
	
	public void StopMove () {
		moving = false;

	}

	private void ChangeDirection() {
		float step = (moveSpeed * 0.5f) * Time.deltaTime;
		Vector3 directionVector = destination - transform.position;
		Vector3 newLookDirection = Vector3.RotateTowards(this.transform.forward, directionVector, step, 0.0f);
		//Debug.DrawRay(transform.position, (directionVector.normalized * 5),Color.white);
		transform.rotation = Quaternion.LookRotation(newLookDirection);
	}
	
	private Vector3 SetDestination (Vector3 destination) {
		Vector3 modDestination = destination;
		modDestination.y = (this.renderer.bounds.extents.y);
		return modDestination;
	}
	
	private void Move (List<Vector3> initVectorList){
		moving = true;
		VectorList = initVectorList;
		destination = SetDestination(VectorList[0]);
		Debug.Log("Moving to " + destination);
	}
	
	private float CalcMoveModifier () {
		float curMod = 1f;
		foreach (float modifier in moveModifiers.Values) {
			curMod *= (1 - modifier);
		}
		return curMod;
	}
	
	private bool DestinationCheck(){
		Vector3 vectorToGoal = destination - transform.position;
		if (bufferZone.magnitude > vectorToGoal.magnitude){
			//Debug.Log("I have reached my destination!");
			return true;			
		}
		return false;
	}
}
