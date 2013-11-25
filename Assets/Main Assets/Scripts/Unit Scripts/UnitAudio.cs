using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]

public class UnitAudio : MonoBehaviour {

    public bool debugInfo;
	public AudioClip[] whatClips;
	public AudioClip[] attackClips;
	public AudioClip[] attackLaunchClips;
	public AudioClip[] readyClips;
	public AudioClip[] yesClips;
    public AudioClip[] deathClips;
    private UnitDebug debugComp;
	
	// Use this for initialization
	void Start () {
        debugComp = this.GetComponent<UnitDebug>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private AudioClip GetRandomClip(AudioClip[] clipLibrary){
		AudioClip returnClip = clipLibrary [(Random.Range (0, clipLibrary.Length))];
		return returnClip;
	}
	
	public void PlayAudio(int libraryFlag) {
		switch (libraryFlag){
		case 1: 
			audio.clip = GetRandomClip(whatClips);
			break;
		case 2:
			audio.clip = GetRandomClip(attackClips);
			break;
		case 3:
			audio.clip = GetRandomClip(attackLaunchClips);
			break;
		case 4:
			audio.clip = GetRandomClip(readyClips);
			break;
		case 5:
			audio.clip = GetRandomClip(yesClips);
			break;
        case 6:
            audio.clip = GetRandomClip(deathClips);
            break;
		}
		audio.Play();
	}

}
