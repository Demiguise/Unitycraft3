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
	private List<NavNode> nodeList = new List<NavNode>();
	
	
	// Use this for initialization
	void Start () {
		moving = false;
		originalMoveSpeed = moveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		moveSpeed = originalMoveSpeed * CalcMoveModifier();
		if (moving){
			if ((DestinationCheck()) && (nodeList.Count == 0)){
				moving = false;
			}
			if ((DestinationCheck()) && (nodeList.Count > 0)){
				nodeList.Remove(nodeList[0]);
				if (nodeList.Count != 0) {
					destination = SetDestination(nodeList[0].nodePosition);
				}
			}
			float step = moveSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, destination, step);
		}
	}
	
	public void StopMove () {
		moving = false;

	}
	
	private Vector3 SetDestination (Vector3 destination) {
		Vector3 modDestination = destination;
		modDestination.y = (this.renderer.bounds.extents.y);
		return modDestination;
	}
	
	private void Move (List<NavNode> initNodeList){
		moving = true;
		nodeList = initNodeList;
		destination = SetDestination(nodeList[0].nodePosition);
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
			Debug.Log("I have reached my destination!");
			return true;			
		}
		return false;
	}
}
