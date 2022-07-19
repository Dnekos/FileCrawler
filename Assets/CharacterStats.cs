using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CharacterStats
{
	public string Name;
	public int Health;
	public int Energy;
	public int Defense;

	[SerializeField]
	public List<CombatAction> loadedActions;


	public CharacterStats(string name, int health, int energy, int defense = 0)
	{
		Name = name;
		Health = health;
		Energy = energy;
		Defense = defense;
		loadedActions = new List<CombatAction>();
	}

	public void AddAction(CombatAction action)
	{
		action.Owner = this;
		loadedActions.Add(action);
	}

	public void ReceiveBasicAction(CombatAction action)
	{
		ReceiveShield(action.defense);
		TakeDamage(action.damage);
	}

	public void TakeDamage(int dam)
	{
		Defense -= dam;
		if (Defense < 0)
			Health = Mathf.Max(0, Defense + Health);
		Defense = Mathf.Max(0, Defense);
	}
	public void ReceiveShield(int def)
	{
		Defense += def;
	}
}
