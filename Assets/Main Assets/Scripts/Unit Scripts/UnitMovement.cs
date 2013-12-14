using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitMovement : MonoBehaviour {
	
	public System.Collections.Generic.Dictionary<string, float> moveModifiers = new System.Collections.Generic.Dictionary<string, float>();

    public bool debugInfo;
	public Vector3 bufferZone;
	private Vector3 destination;
	public float moveSpeed;
	public float originalMoveSpeed;
	public bool moving;
    private UnitDebug debugComp;
	private List<Vector3> VectorList = new List<Vector3>();
	
	
	// Use this for initialization
	void Start () {
		moving = false;
		originalMoveSpeed = moveSpeed;
		destination = this.transform.forward;
        debugComp = this.GetComponent<UnitDebug>();
	}
	
	// Update is called once per frame
	void Update () {
		moveSpeed = originalMoveSpeed * CalcMoveModifier();
        ChangeDirection();
		if (moving){
            
            this.GetComponent<UnitAnimation>().FadeAnimationState("Base Layer.Walk");
			if ((DestinationCheck()) && (VectorList.Count == 0)){
                StopMove();
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
        if (moving == true)
        {
            this.GetComponent<UnitAnimation>().FadeAnimationState("Base Layer.Idle");
            moving = false;
        }
        else if (this.GetComponent<UnitAnimation>().IsCurrentlyPlaying("Base Layer.Walk"))
        {
            this.GetComponent<UnitAnimation>().FadeAnimationState("Base Layer.Idle");
        }
	}

	private void ChangeDirection() {
		float step = (moveSpeed * 0.5f) * Time.deltaTime;
		Vector3 directionVector = destination - transform.position;
		Vector3 newLookDirection = Vector3.RotateTowards(this.transform.forward, directionVector, step, 0.1f);
        debugComp.LogIfTrue("UMove", ("Current look direction -> " + newLookDirection + "| Destination current set to ->" + destination), debugInfo);
		transform.rotation = Quaternion.LookRotation(newLookDirection);
	}
	
	private Vector3 SetDestination (Vector3 destination) {
		Vector3 modDestination = destination;
		modDestination.y = 0.5f;
		return modDestination;
	}

	private void MoveToAttack(List<Vector3> VectorList){
		Move (VectorList);
	}

	private void MoveToPoint(List<Vector3> VectorList){
		this.GetComponent<UnitCombat>().StopSwinging();
		Move (VectorList);
	}

	private void Move (List<Vector3> initVectorList){
		if (initVectorList != null) {
			moving = true;
            this.GetComponent<UnitAnimation>().FadeAnimationState("Base Layer.Walk");
			this.GetComponent<UnitAudio>().PlayAudio(5);
			VectorList = initVectorList;
			destination = SetDestination (VectorList [0]);
            debugComp.Log("UMove", ("Moving to " + destination));
		}
        foreach (Vector3 movePoint in initVectorList)
        {
            Debug.Log(movePoint);
        }
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
            debugComp.LogIfTrue("UMove", "I have reached my destination!", debugInfo);
			return true;			
		}
		return false;
	}
}
