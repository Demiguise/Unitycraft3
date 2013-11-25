using UnityEngine;
using System.Collections;

public class UnitCombat : MonoBehaviour {
	
	public System.Collections.Generic.Dictionary<string, int> attackModifiers = new System.Collections.Generic.Dictionary<string, int>();
	public System.Collections.Generic.Dictionary<string, int> defenseModifiers = new System.Collections.Generic.Dictionary<string, int>(); //From things like "-20% damage taken"
	public Spell curOrbEffect = new Spell();

    public bool debugInfo;
	public int[] damageRange;
	public int armour;
	public bool swinging;
	public float curTimer;
	public float attackdistance;
	public float swingTimer; //Time it takes to swing
	public float damageReduction; //from Armour
	public GameObject targetUnit;
	public float distanceToTarget;
    private UnitDebug debugComp;

	void Start () {
		damageRange = new int[2];
		damageRange [0] = 10;
		damageRange [1] = 20;
		curTimer = swingTimer;
        debugComp = this.GetComponent<UnitDebug>();
	}
	
	void Update () {
		CalculateArmourValue();
		if (targetUnit != null) {
			distanceToTarget = CalculateDistanceToTarget();
			if (distanceToTarget <= attackdistance) {
				this.GetComponent<UnitMovement>().SendMessage("StopMove");
				if (swinging && curTimer > 0) {
					curTimer -= Time.deltaTime;
				}
				if (curTimer < 0) {
					Attack(targetUnit);
					ResetSwingTimer();
				}
			}
			else MoveToAttackZone();
		}
	}
	
	public float CalculateDistanceToTarget () {
		Vector3 VectorToTarget = targetUnit.transform.position - this.transform.position;
		float targetDistance = VectorToTarget.magnitude;
		return targetDistance;
	}
	
	public void MoveToAttackZone () {
		this.GetComponent<UnitCore> ().RequestMovement (targetUnit.transform.position, 1);
	}
	
	public void StartSwinging (GameObject reqTargetUnit) {
		swinging = true;
		targetUnit = reqTargetUnit;
	}
	
	private void CalculateArmourValue () {
		damageReduction = ((float)armour / 100);
	}
	
	public void StopSwinging () {
		swinging = false;
		targetUnit = null;
		ResetSwingTimer();
	}
	
	private void ResetSwingTimer () {
		curTimer = swingTimer;
	}

	
	private int CollateAttackMods () {
		int fullMods = 0;
		foreach (int mod in attackModifiers.Values) {
			fullMods += mod;
		}
		return fullMods;
	}
	 	
	private float CollateDamageReductionMods () {
		float curMod = (1f * damageReduction);
		foreach (float modifier in defenseModifiers.Values) {
			curMod *= modifier;
			Debug.Log(curMod);
		}
		return (1 - curMod);
	}	
	
	public void Attack (GameObject defendingUnit) {
        this.GetComponent<UnitAnimation>().ForceAnimationState("Base Layer.Attack");
		int attackDamage = Random.Range(damageRange[0], damageRange[1]);
		int netDamage = (int)Mathf.Round(attackDamage + CollateAttackMods());
        debugComp.LogIfTrue("UCombat", ("Attacked with " + netDamage + " damage"), debugInfo);
		defendingUnit.GetComponent<UnitCombat>().Defend(netDamage);
	}

	public void Defend (int damageToTake) {
		int netDamage = (int)Mathf.Round (damageToTake * CollateDamageReductionMods());
        float newHealth = this.GetComponent<UnitCore>().health - netDamage;
		this.GetComponent<UnitCore>().health = newHealth;
        debugComp.LogIfTrue("UCombat", ("Taken (" + netDamage + ") damage. (" + newHealth + ") health remaining"), debugInfo);
	}
}
