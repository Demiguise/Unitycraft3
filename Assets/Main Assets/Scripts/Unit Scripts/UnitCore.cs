using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof(UnitMovement), typeof(UnitHealthBar), typeof(UnitAudio))]
[RequireComponent( typeof(UnitAbilities), typeof(UnitAnimation), typeof(UnitCombat))]
[RequireComponent( typeof(UnitDebug))]

public class UnitCore : MonoBehaviour {
	
	public bool selected;
    public bool debugInfo;
    public int uniqueID;
	public float health;
	public float healthMax;
	public Vector3 worldToScreen;
	public int factionFlag;
	public Vector3 extents;
	public NavNodeManager navManager;
    private UnitDebug debugComp;
    public float testScale;
		
	// Use this for initialization
	void Start () {
        uniqueID = GenerateUniqueHash();
        debugComp = this.GetComponent<UnitDebug>();
        debugComp.SetUID(uniqueID);
		selected = false;
		health = healthMax;
        this.GetComponent<UnitAudio>().SendMessage("PlayAudio", 4);
	}

    private int GenerateUniqueHash()
    {
        int hash = 0;
        return hash;
    }

	// Update is called once per frame
	void Update () {
		extents = this.GetComponent<BoxCollider> ().bounds.extents;
		worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
        CheckForDeath();
	}

    private void CheckForDeath()
    {
        if (health < 0)
        {
            debugComp.Log("UCore", "Unit has died");
            OnDeath();
        }
    }

    private void OnDeath()
    {
        GameObject unitInstance = Instantiate(Resources.Load("Prefabs/Zergling-DeathAcid"), transform.position, transform.rotation) as GameObject;
        Destroy(this.gameObject);
    }

	public void InitNavManager (NavNodeManager manager){
		navManager = manager;
	}

	public void RequestMovement (Vector3 targetDestination, int typeFlag = 0){
		if (navManager != null){
			switch (typeFlag){
			case 0:
				this.GetComponent<UnitMovement>().SendMessage("MoveToPoint", navManager.FindTraversalMap(transform.position, targetDestination, this.gameObject));
				break;
			case 1:
				this.GetComponent<UnitMovement>().SendMessage("MoveToAttack", navManager.FindTraversalMap(transform.position, targetDestination, this.gameObject));
				break;
			}
		}
		else {
			debugComp.Log("UCore", "No Navmanager attached to unit core!");
		}
	}
	
	void ChangeSelectedState (bool state) {
		selected = state;
		this.GetComponent<UnitHealthBar>().SendMessage("ToggleHealthBars", state);
		if (selected) {
            //
			this.GetComponent<UnitAudio>().SendMessage("PlayAudio", 1);
		}
		if (!selected) {
            //
		}
	}
	
}
