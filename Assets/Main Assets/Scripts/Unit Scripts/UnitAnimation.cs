using UnityEngine;
using System.Collections;

public class UnitAnimation : MonoBehaviour {

    public bool debugInfo;
	public float timeSpentIdle;
	public float maxIdleTime;
    public int currentAnimHash;
    public int targetAnimHash;
    //public int currentHashCode;
    public string requestAnimName;
    private UnitDebug debugComp;

	// Use this for initialization
	void Start () {
		timeSpentIdle = 0f;
        //currentHashCode = this.GetComponent<Animator>().GetHashCode();
        debugComp = this.GetComponent<UnitDebug>();
        targetAnimHash = 0;
		maxIdleTime = GenerateFidgetTime();
		FadeAnimationState ("Idle");
	}

	// Update is called once per frame
	void Update () {
		if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle")) {
			timeSpentIdle += Time.deltaTime;
		} 
		else {
			timeSpentIdle = 0f;
		}
		if (timeSpentIdle > maxIdleTime){
            FadeAnimationState("Base Layer.Fidget");
			maxIdleTime = GenerateFidgetTime();
			timeSpentIdle = 0f;
		}
        SetCurrentAnimationInfo();
	}

    public bool IsCurrentlyPlaying(string state)
    {
        int stateHash = Animator.StringToHash(state);
        if ((currentAnimHash == stateHash) || (targetAnimHash == stateHash))
        {
            return true;
        }
        return false;
    }

    private void SetCurrentAnimationInfo()
    {
        currentAnimHash = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).nameHash;
        if (currentAnimHash == targetAnimHash)
        {
            targetAnimHash = 0;
        }
    }

    private float GenerateFidgetTime()
    {
		float randomTime = Random.Range (5f, 10f);
		return randomTime;
	}

	public void FadeAnimationState (string state) {
        if (!IsCurrentlyPlaying(state)){
            debugComp.LogIfTrue("UAnim", ("Fading set state to -> " + state), debugInfo);
		    this.GetComponent<Animator>().CrossFade(state, 0.1f);
            requestAnimName = state;
            targetAnimHash = Animator.StringToHash(state);
        }
	}

	public void ForceAnimationState(string state) {
        debugComp.LogIfTrue("UAnim", ("!Forcing set state to -> " + state), debugInfo);
		this.GetComponent<Animator> ().Play (state);
	}
}
