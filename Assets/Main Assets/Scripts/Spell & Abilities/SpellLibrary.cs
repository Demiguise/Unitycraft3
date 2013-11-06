using UnityEngine;
using System.Collections;

public class SpellLibrary : MonoBehaviour {

	
	void Start () {
	}
	

	void Update () {
	}
	
}

public class Spell {
	
	public Texture2D spellIcon;
	protected GameObject parentUnit;
	public float maxTime;
	public float timer;
	public string spellName;
	public bool stackable;
	public int maxStacks;
	public int curStacks;
	
	public void SetIcon (Texture2D iconTexture) {
		spellIcon = iconTexture;
	}
	
	public void AddMoveModifier (GameObject unit, float modPercent) {
		unit.GetComponent<UnitMovement>().moveModifiers.Add(spellName, modPercent);
	}
	public void RemoveMoveModifier (GameObject unit) {
		unit.GetComponent<UnitMovement>().moveModifiers.Remove(spellName);
	}
	
	public void IncrementStacks () {
		if (curStacks < maxStacks) {
			curStacks += 1;
			RemoveEffect();
			AddEffect();
		}
		ResetTimer();
	}
	
	public void ResetTimer () {
		timer = maxTime;
	}
	
	public virtual void Update () {
		timer -= Time.deltaTime;
	}
	
	public virtual void AddEffect () {
	}
	public virtual void RemoveEffect () {
	}
}

public class ChillingTouch : Spell {
	
	private float moveSpeedModifier = 0.1f;
	
	public ChillingTouch (GameObject parentObject) {
		parentUnit = parentObject;
		spellName = "Chilling Touch";
		stackable = false;
		maxStacks = 1;
		curStacks = 1;
		maxTime = 5;
		timer = maxTime;
	}
	
	public override void AddEffect () {
		AddMoveModifier(parentUnit, (moveSpeedModifier * curStacks));
	}
	public override void RemoveEffect () {
		RemoveMoveModifier(parentUnit);
	}
}

public class Haste : Spell {
	private float moveSpeedModifier = -.25f;
	
	public Haste (GameObject parentObject) {
		parentUnit = parentObject;
		spellName = "Haste";
		stackable = true;
		maxStacks = 10;
		curStacks = 1;
		maxTime = 10;
		timer = maxTime;
	}
	
	public override void AddEffect () {
		AddMoveModifier(parentUnit, (moveSpeedModifier * curStacks));
	}
	public override void RemoveEffect () {
		RemoveMoveModifier(parentUnit);
	}
}
