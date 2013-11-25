using UnityEngine;
using System.Collections;

public class UnitHealthBar : MonoBehaviour {
	
	public float heightOffset;
	public float widthOffset;
	public float width;
	private float health;
	private float healthMax;
	public Texture healthTexture;
	public Texture backgroundTexture;
	private Vector3 curScreenPos;
	private bool selected;
	
	// Use this for initialization
	void Start () {
		healthMax = this.GetComponent<UnitCore>().healthMax;
		health = this.GetComponent<UnitCore>().health;
		selected = false;
	}
	
	void OnGUI () {
		if (selected) {
			GUI.BeginGroup(new Rect(curScreenPos.x - widthOffset, ((Screen.height - curScreenPos.y) - heightOffset), width, 8));
				GUI.DrawTexture(new Rect(0,0,(transform.localScale.x * width),8), backgroundTexture);
				GUI.DrawTexture(new Rect(1,1,((width - 2) * (health / healthMax)),6), healthTexture);
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
