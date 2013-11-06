using UnityEngine;
using System.Collections;

public class UnitCombat : MonoBehaviour {
	
	public System.Collections.Generic.Dictionary<string, int> attackModifiers = new System.Collections.Generic.Dictionary<string, int>();
	public System.Collections.Generic.Dictionary<string, int> defenseModifiers = new System.Collections.Generic.Dictionary<string, int>(); //From things like "-20% damage taken"
	public Spell curOrbEffect = new Spell();
	
	public int strength;
	public int agility;
	public int intelligence;
	public int minDamage;
	public int maxDamage;
	public int armour;
	public bool swinging;
	public float curTimer;
	public float attackdistance;
	public float swingTimer; //Time it takes to swing
	public float damageReduction; //from Armour
	public GameObject targetUnit;
	public float distanceToTarget;
	
	void Start () {
		minDamage = 10;
		maxDamage = 20;
		CalculateAttributeDamage();
		curTimer = swingTimer;
	}
	
	void Update () {
		CalculateArmourValue();
		if (targetUnit != null) {
			CalculateDistanceToTarget();
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
	
	public void CalculateDistanceToTarget () {
		Vector3 VectorToTarget = targetUnit.transform.position - this.transform.position;
		distanceToTarget = VectorToTarget.magnitude;
	}
	
	public void MoveToAttackZone () {
		this.GetComponent<UnitMovement>().SendMessage("Move", targetUnit.transform.position);
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
	
	private void CalculateAttributeDamage () {
		
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
		int attackDamage = Random.Range(minDamage, maxDamage);
		int netDamage = (int)Mathf.Round(attackDamage + CollateAttackMods());
		Debug.Log(this.gameObject + " has attacked with " + netDamage + " damage");
		defendingUnit.GetComponent<UnitCombat>().Defend(netDamage);
	}
	
	public void Defend (int damageToTake) {
		int netDamage = (int)Mathf.Round (damageToTake * CollateDamageReductionMods());
		this.GetComponent<UnitCore>().health -= netDamage;
		Debug.Log(this.gameObject + " has taken " + netDamage + " damage.");
	}
}
