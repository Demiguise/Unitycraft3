using UnityEngine;
using System.Collections;

public class UnitHealthBar : MonoBehaviour {
	
	public float heightOffset;
	private float widthOffset;
	private float width;
	private float health;
	private float healthMax;
	public Texture healthTexture;
	public Texture backgroundTexture;
	private Vector3 curScreenPos;
	private bool selected;
	
	// Use this for initialization
	void Start () {
		width = (this.renderer.bounds.size.x) * 13f;
		widthOffset = (this.renderer.bounds.extents.x) * 13f;
		healthMax = this.GetComponent<UnitCore>().healthMax;
		health = this.GetComponent<UnitCore>().health;
		selected = false;
	}
	
	void OnGUI () {
		if (selected) {
			GUI.BeginGroup(new Rect(curScreenPos.x - widthOffset, ((Screen.height - curScreenPos.y) - heightOffset), width, 12));
				GUI.DrawTexture(new Rect(0,0,(transform.localScale.x * width),12), backgroundTexture);
				GUI.DrawTexture(new Rect(1,1,((width - 2) * (health / healthMax)),10), healthTexture);
			GUI.EndGroup();
		}
	}
	// Update is called once per frame
	void Update () {
		curScreenPos = Camera.main.WorldToScreenPoint(transform.position);
		health = this.GetComponent<UnitCore>().health;
	}
	
	void ToggleHealthBars (bool state) {
		selected = state;
	}
}
