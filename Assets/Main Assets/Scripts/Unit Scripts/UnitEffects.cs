using UnityEngine;
using System.Collections;
using System.Linq;

public class UnitEffects : MonoBehaviour {
	
	public System.Collections.Generic.List<Spell> effectList = new System.Collections.Generic.List<Spell>();
	public bool debugInformation;

	// Use this for initialization
	void Start () {
		debugInformation = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.K)){
			AddEffect(1);
		}
		if (Input.GetKeyDown(KeyCode.H)) {
			AddEffect(2);
		}
		UpdateEffects();
	}
	
	private void UpdateEffects () {
		System.Collections.Generic.List<Spell> toBeRemoved = new System.Collections.Generic.List<Spell>();
		foreach (Spell effect in effectList) {
			effect.Update();
			if (effect.timer <= 0) {
				if (debugInformation) Debug.Log("Timer run out for: " + effect);
				effect.RemoveEffect();
				toBeRemoved.Add(effect);
			}
		}
		if (toBeRemoved.Count > 0) {
			foreach (Spell effect in toBeRemoved) {
				effectList.Remove(effect);
			}
			toBeRemoved.Clear();
			if (debugInformation) Debug.Log("Effect list count = " + effectList.Count);
		}
	}
	
	public void AddEffect (int spellID) {
		switch (spellID) {
			case 1:
				if (debugInformation) Debug.Log("Attempting to add Chilling Touch");
				ActuallyAddTheEffect(new ChillingTouch(this.transform.gameObject));
				if (debugInformation) Debug.Log("Effect list count = " + effectList.Count);
				break;
			case 2:
				if (debugInformation) Debug.Log("Attempting to add Haste");
				ActuallyAddTheEffect(new Haste(this.transform.gameObject));
				if (debugInformation) Debug.Log("effect list count = " + effectList.Count);
				break;

		}
	}
	
	private void ActuallyAddTheEffect (Spell reqSpellType) {
		Spell spellQuery = CheckForDuplicates(reqSpellType.spellName);
		if (spellQuery != null) {
			if (spellQuery.stackable) spellQuery.IncrementStacks();
			else spellQuery.ResetTimer();
		}
		else {
			effectList.Add(reqSpellType);
			reqSpellType.AddEffect();
		}
	}
	
	private Spell CheckForDuplicates (string reqSpellName) {
		System.Collections.Generic.IEnumerable<Spell> spellQuery = effectList.Where(spell => spell.spellName == reqSpellName);
		if (spellQuery.Count() > 0) return spellQuery.First();
		return null;
	}
	
}

