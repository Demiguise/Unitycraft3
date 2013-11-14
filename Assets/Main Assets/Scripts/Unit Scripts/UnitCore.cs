using UnityEngine;
using System.Collections;

[RequireComponent( typeof(UnitMovement), typeof(UnitHealthBar), typeof(UnitAudio))]
[RequireComponent( typeof(UnitAbilities), typeof(UnitAnimation), typeof(UnitCombat))]

public class UnitCore : MonoBehaviour {
	
	public bool selected;
	public float health;
	public float healthMax;
	public Vector3 worldToScreen;
	public int factionFlag;
	public Vector3 extents;

		
	// Use this for initialization
	void Start () {
		selected = false;
		health = healthMax;
		this.GetComponent<UnitAudio>().SendMessage("PlayAudio");
	}
	
	// Update is called once per frame
	void Update () {
		extents = this.GetComponent<MeshFilter>().mesh.bounds.extents;
		worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
	}
	
	void ChangeSelectedState (bool state) {
		selected = state;
		this.GetComponent<UnitHealthBar>().SendMessage("ToggleHealthBars", state);
		if (selected) {
			renderer.material.color = Color.red;
			this.GetComponent<UnitAudio>().SendMessage("PlayAudio");
			
		}
		if (!selected) {
			renderer.material.color = Color.green;
		}
	}
	
	void TakeDamage (float damageToTake) {
		Debug.Log("Taking " + damageToTake + " damage.");
		if (health > 0) {
			health -= damageToTake;
		}
		if (health > healthMax) {
			health = healthMax;
		}
		if (health < 0) {
			health = 0;
		}
	}
	
}
