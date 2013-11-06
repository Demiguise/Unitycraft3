using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]

public class UnitAudio : MonoBehaviour {
	
	public AudioClip[] audioClips;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void PlayAudio() {
		audio.clip = audioClips[(Random.Range(0, audioClips.Length))];
		audio.Play();
	}
}
