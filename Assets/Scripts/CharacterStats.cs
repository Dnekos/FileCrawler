using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CharacterStats : MonoBehaviour
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

	/// <summary>
	/// resolved the effects of an action
	/// </summary>
	/// <param name="action"></param>
	/// <returns>Returns true if the character's health hits 0</returns>
	public virtual void ReceiveBasicAction(CombatAction action)
	{
		ReceiveShield(action.defense);
		TakeDamage(action.damage);
	}

	/// <summary>
	/// they take damage, defense takes priority over health
	/// </summary>
	/// <param name="dam"></param>
	/// <returns>true if damage is taken</returns>
	public virtual bool TakeDamage(int dam)
	{
		int oldHP = Health;
		Defense -= dam;
		if (Defense < 0)
			Health = Mathf.Max(0, Defense + Health);
		Defense = Mathf.Max(0, Defense);
		return oldHP > Health;
	}
	public void ReceiveShield(int def)
	{
		Defense += def;
	}

	public virtual void PerformAction(CombatAction action)
	{

	}
}
