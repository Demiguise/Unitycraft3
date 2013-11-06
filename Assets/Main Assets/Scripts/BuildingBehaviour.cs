using UnityEngine;
using System.Collections;

[RequireComponent (typeof (HealthBar))]

public class BuildingBehaviour : MonoBehaviour {
	
	public GameObject unitType;
	public float constructionTimer;
	private float internalTimer;
	private bool creatingUnit;
	private bool selected;
	private bool rallyPointSet;

	// Use this for initialization
	void Start () {
		creatingUnit = false;
		rallyPointSet = false;
		selected = false;
		if (!unitType) {
			Debug.LogWarning(gameObject + " Requires a unit type!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (selected) {
			if (Input.GetKeyDown(KeyCode.J)) {
				BeginConstructionTimer();
			}
		}
		IncrementConstructionTimer();
	}
	
	
	void ChangeSelectedState (bool state) {
		selected = state;
		//Debug.Log (this.renderer.bounds.size);
		//Debug.Log (this.renderer.bounds.extents);
		if (selected){
			renderer.material.color = Color.red;
			this.GetComponent<HealthBar>().SendMessage("ToggleHealthBars", true);
		}
		if (!selected){
			renderer.material.color = Color.yellow;
			this.GetComponent<HealthBar>().SendMessage("ToggleHealthBars", false);
		}
	}
	
	void SetRallyPont (Vector3 rallyLocation) {
		transform.FindChild("rallyPoint").position = rallyLocation;
		rallyPointSet = true;
		Debug.Log("Rally point set!");
	}
	
	void CreateUnit () {
		Transform outputLoc = transform.FindChild("outputLoc");
		Vector3 unitCreationPosition = new Vector3(outputLoc.position.x, 3, outputLoc.position.z);
		GameObject unitInstance = Instantiate(unitType, unitCreationPosition, transform.rotation) as GameObject;
		if (rallyPointSet) {
			unitInstance.SendMessage("Move", transform.FindChild("rallyPoint").transform.position);
		}
	}
	
	void BeginConstructionTimer () {
		if (internalTimer == 0f) {
			creatingUnit = true;
			Debug.Log ("Unit in Production!");
		}
	}
	
	void IncrementConstructionTimer () {
		if (creatingUnit) {
			internalTimer += Time.deltaTime;
			CheckConstructionTimer();
		}
	}
	
	void CheckConstructionTimer () {
		if (internalTimer >= constructionTimer) {
			creatingUnit = false;
			internalTimer = 0f;
			Debug.Log ("Unit Completed!");
			CreateUnit();
		}
	}
}
