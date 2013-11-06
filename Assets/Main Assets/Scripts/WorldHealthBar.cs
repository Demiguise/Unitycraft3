using UnityEngine;
using System.Collections;

public class WorldHealthBar : MonoBehaviour {
	
	public float health;
	public float healthMax = 100f;
	public bool selected;
	private Vector3 initHealthScale;
	private Transform healthBar;
	private Transform backgroundBar;
	public bool findingScale;
	public float modifier;
	private Vector3 something;
	
	// Use this for initialization
	void Start () {
		modifier = 1.2f;
		findingScale = false;
		health = healthMax;
		healthBar = transform.GetChild(0);
		backgroundBar = transform.GetChild(1);
		initHealthScale = healthBar.localScale;
		selected = false;
		ToggleHealthBars(false);
		something = new Vector3(0, -1, 0);
		transform.Rotate (0, (transform.parent.rotation.eulerAngles.y * -1),0,Space.Self);
	}
	
	// Update is called once per frame
	void Update () {
		if (findingScale) {
			FindExtents();
		}
		
	}
	
	void ToggleHealthBars(bool state) {
		selected = state;
		for (int i = 0 ; i < transform.childCount ; i++ ) {
			transform.GetChild(i).renderer.enabled = state;
		}
	}
	
	void TakeDamage (float damageTaken) {
		health -= damageTaken;
		if (CheckHealth()) {
			Vector3 newHealthScale = initHealthScale;
			newHealthScale.x = newHealthScale.x * (health / healthMax);
			healthBar.localScale = newHealthScale;
		}
	}
	
	bool CheckHealth () { 
		if (health < 0) {
			Debug.Log ("I'm Dead!");
			return false;
		}
		if (health > healthMax) {
			return false;
		}
		return true;
	}
	
	void FindExtents () {
		Vector3 newScale = backgroundBar.localScale;
		newScale.x += 0.1f;
		backgroundBar.localScale = newScale;
		if (backgroundBar.renderer.bounds.extents.x > (transform.parent.renderer.bounds.extents.x * modifier)) {
			Debug.Log (backgroundBar.renderer.bounds.extents + "And" + transform.parent.renderer.bounds.extents);
			Debug.Log (newScale + "Is a close approximation of the scale needed");
			findingScale = false;
			return;
		}
	}
}
