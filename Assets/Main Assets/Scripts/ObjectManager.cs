using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjectManager : MonoBehaviour {
	
	private System.Collections.Generic.List<GameObject> selectedUnitList;
	
	// Use this for initialization
	void Start () {
		selectedUnitList = new System.Collections.Generic.List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void AddObject(GameObject objectToAdd) {
		selectedUnitList.Add(objectToAdd);
	}
	
	public void RemoveObject(GameObject objectToRemove) {
		if(selectedUnitList.Remove(objectToRemove)){
			Debug.Log("Object was successfully removed!");
		}
		else{
			Debug.LogWarning("Object was not removed!!");
		}
	}
	
	
}
