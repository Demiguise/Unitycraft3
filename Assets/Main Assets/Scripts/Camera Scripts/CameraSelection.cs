using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraSelection : MonoBehaviour {
	
	public bool rayDebugInfo;
	private GameObject[] unitArray;
	public int factionFlag;
	NavNodeManager navManager = new NavNodeManager();
	
	private System.Collections.Generic.List<GameObject> selectedObjectList = new System.Collections.Generic.List<GameObject>();
	private Vector3 originalMousePosition;
	private Vector3 newMousePosition;
	private bool buildMenu;
	
	
	// Use this for initialization
	void Start () {
		unitArray = GameObject.FindGameObjectsWithTag("Unit");
		rayDebugInfo = true;
		buildMenu = false;
		navManager.CreateSpawnNode();
	}
	
	// Update is called once per frame
	void Update () {
		if (!buildMenu) {
			if (Input.GetMouseButtonDown(0)){
				originalMousePosition = Input.mousePosition;
				SelectObjects();
			}
			if (Input.GetMouseButton(0)){
				newMousePosition = Input.mousePosition;
			}
			if (Input.GetMouseButtonUp(0)){
				BoxSelect();
				originalMousePosition = Vector3.zero;
				newMousePosition = Vector3.zero;
			}
			if (Input.GetKeyUp(KeyCode.L)){
				navManager.BeginFloodFill();
				//navManager.FindNodeLinks();
			}
			if ((Input.GetMouseButtonDown(1)) && (selectedObjectList.Count > 0)){
				int[] worldLayer = {9};
				int[] gameLayer = {8,9,10};
			 	GameObject objectHit = CastSelectionRay(gameLayer);
				if (objectHit != null) {
					if (objectHit.tag == "MapGeom") {
						if (selectedObjectList[0].tag == "Unit") {
							foreach (GameObject unit in selectedObjectList){
								unit.GetComponent<UnitMovement>().SendMessage("Move", CastWorldRay(worldLayer));
							}
						}
						if (selectedObjectList[0].tag == "Building") {
							foreach (GameObject building in selectedObjectList) {
								building.SendMessage("SetRallyPont", CastWorldRay(worldLayer));
							}
						}
					}
					if (objectHit.tag == "Unit" || objectHit.tag == "Building") {
						foreach (GameObject unit in selectedObjectList) {
							unit.GetComponent<UnitCombat>().StartSwinging(objectHit);
						}
					}
				}
			}
		}
		if (buildMenu) {
			ShowGhostBuilding();
		}
	}
	
	void OnGUI () {
		if (GUI.Button(new Rect(0,0,75,25), "Regen Nav")) {
			navManager.RegenerateNavMesh();
		}
	}
	
	void ShowGhostBuilding() {
		
	}
	
	void BoxSelect () {
		foreach (GameObject unit in unitArray){
			Vector3 unitScreenPoint = unit.GetComponent<UnitCore>().worldToScreen;
			if(CheckUnitInRange(unitScreenPoint, originalMousePosition, newMousePosition) && CheckFactionFlags(unit)){
				ToggleObjectSelection(unit, true);
			}
		}
	}
	
	private bool CheckFactionFlags (GameObject unit) {
		if (unit.GetComponent<UnitCore>().factionFlag == factionFlag) {
			return true;
		}
		else return false;
	}
	
	bool CheckUnitInRange (Vector3 unitScreenPos, Vector3 firstMousePosition, Vector3 secondMousePosition) {
		Bounds boundingBox = new Bounds(Vector3.zero, Vector3.zero);
		boundingBox.SetMinMax(firstMousePosition, secondMousePosition);
		unitScreenPos.z = 0;
		Vector3 newBounds = Vector3.zero;
		for (int i = 0 ; i <= 2 ; i++ ){
			newBounds[i] = Mathf.Abs(boundingBox.extents[i]);
		}
		boundingBox.extents = newBounds;
		//Debug.Log(boundingBox);
		return boundingBox.Contains(unitScreenPos);
	}
	
	void ToggleObjectSelection (GameObject selectedUnit, bool state) {
		selectedUnit.SendMessage("ChangeSelectedState", state);
		selectedObjectList.Add(selectedUnit);
	}
	
	void DeselectAllObjects () {
		foreach (GameObject Unit in selectedObjectList){
			Unit.SendMessage("ChangeSelectedState", false);
		}
		selectedObjectList.Clear();
	}
	
	Vector3 CastWorldRay (int[] layers) {
		RaycastHit objectHit = DetectRayCollisions(CreateLayerMask(layers), 100);
		if (objectHit.collider.gameObject.tag == "MapGeom"){
			return objectHit.point;
		}
		return new Vector3(0,0,0);
	}
	
	void SelectObjects () {
		int[] unitMask = {8, 10};
		GameObject selectedUnit = CastSelectionRay(unitMask);
		if (selectedUnit != null && CheckFactionFlags(selectedUnit)){
			DeselectAllObjects();
			ToggleObjectSelection(selectedUnit, true);
		}
	}
	
	GameObject CastSelectionRay (int[] layers) {
		RaycastHit objectHit = DetectRayCollisions(CreateLayerMask(layers), 100);
		if (objectHit.collider != null){
			if (objectHit.collider.gameObject.tag == "Unit" || objectHit.collider.gameObject.tag == "Building" || objectHit.collider.gameObject.tag == "MapGeom"){
				return objectHit.collider.gameObject;
			}
		}
		return null;
	}
	
	int CreateLayerMask (int[] layers) {
		int layerMask = 1;
		foreach (int Layer in layers){
			int i = 1 << Layer;
			layerMask = layerMask | i;
		}
		return layerMask;
	}
	
	RaycastHit DetectRayCollisions (int layerMask, float distance) {
		RaycastHit objectHit;
		if (Physics.Raycast(CreateScreenRay(), out objectHit, distance, layerMask)){
			return objectHit;
		}
		else
			return objectHit;
	}
	
	void DrawDebugRay (Ray rayRequest) {
		if (rayDebugInfo){
			Debug.DrawRay(transform.position, (rayRequest.direction) * 20, Color.yellow, 0);
		}
	}
	
	Ray CreateScreenRay () {
		Ray screenRay = camera.ScreenPointToRay(Input.mousePosition);
		DrawDebugRay(screenRay);
		return screenRay;
	}
}
