using UnityEngine;
using System.Collections;

[RequireComponent (typeof (HealthBar))]

public class UnitBehaviour : MonoBehaviour {
	
	public bool selected;
	private bool moving;
	public float moveSpeed;
	private Vector3 requestedLocation;
	private Vector3 movementBufferZone;
	public Vector3 worldToScreen;
	public AudioClip[] audioClips;
	public System.Collections.Generic.List<string> keys = new System.Collections.Generic.List<string>();
	public System.Collections.Generic.List<string> values = new System.Collections.Generic.List<string>();
	private System.Collections.Generic.Dictionary<string, string> abilityDictionary = new System.Collections.Generic.Dictionary<string, string>();
	
	// Use this for initialization
	void Start () {
		selected = false;
		movementBufferZone = new Vector3(0.5f,0,0.5f);
		PlayAudio();
	}
	
	void Awake () {
		//Sewing dictionary together from keys and values.
		if (keys.Count != values.Count) {
			Debug.LogError("Keys and Values count do not match!");
			Debug.Break();
		}
		for (int i = 0 ; i < keys.Count ; i++ ) {
			abilityDictionary.Add(keys[i], values[i]);
		}
	}
	// Update is called once per frame
	void Update () {
		if (moving){
			if (DestinationCheck()){
				//Debug.Log("I've arrived at my position! (I think)");
				moving = false;
			}
			float step = moveSpeed * Time.deltaTime;
			//Debug.Log(Vector3.MoveTowards(transform.position, requestedLocation, step));
			transform.position = Vector3.MoveTowards(transform.position, requestedLocation, step);
		}
		worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
	}
	
	void ActivateAbility(KeyCode keyPressed) {
		Debug.Log("Asked to activate ability with code: " + keyPressed);
		
	}
	
	void ChangeSelectedState (bool state) {
		selected = state;
		if (selected){
			ChangeColor(Color.red);
			//Debug.Log("I'm at: " + transform.position);
			this.GetComponent<HealthBar>().SendMessage("ToggleHealthBars", true);
			PlayAudio();
		}
		if (!selected){
			ChangeColor(Color.green);
			this.GetComponent<HealthBar>().SendMessage("ToggleHealthBars", false);
			
		}
	}
	
	void ChangeColor (Color color) {
		renderer.material.color = color;
	}
	
	void Move (Vector3 moveLocation){
		moving = true;
		requestedLocation = moveLocation;
		Debug.Log("I've been asked to move to: " + moveLocation);
	}
	
	bool DestinationCheck(){
		Vector3 vectorToGoal = requestedLocation - transform.position;
		if ((movementBufferZone).magnitude > vectorToGoal.magnitude){
			return true;			
		}
		return false;
	}

	void PlayAudio() {
		audio.clip = audioClips[(Random.Range(0, audioClips.Length))];
		audio.Play();
	}
}