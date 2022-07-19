using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats : CharacterStats
{
	[Header("Enemy Specific"), SerializeField]
	int maxAttacks = 1;
	[HideInInspector]
	public CombatAction[] preppedActions;

	public EnemyStats(string name, int health, int energy, int defense = 0, int maxattacks = 1) : base(name,health,energy,defense)
	{
		maxAttacks = maxattacks;
	}


	public CombatAction[] PrepareAttack(SettingsManager fileOpener)
	{
		preppedActions = new CombatAction[Random.Range(1, maxAttacks + 1)];
		for (int i = 0; i < preppedActions.Length; i++)
		{
			preppedActions[i] = loadedActions[Random.Range(0, loadedActions.Count)];
			fileOpener.CreateWeaponFile(preppedActions[i], "", "Enemy - " + Name + "\\Next Attack");
		}
		return preppedActions;
	}
}
