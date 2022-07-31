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

	[SerializeField]
	GameObject DeathPrefab;

	// components
	Animator anim;

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
	}

	public EnemyStats(string name, int health, int energy, int defense = 0, int maxattacks = 1) : base(name,health,energy,defense)
	{
		maxAttacks = maxattacks;
	}
	public override bool TakeDamage(int dam)
	{
		bool tookDamage = base.TakeDamage(dam);
		if (base.TakeDamage(dam))
			anim.SetTrigger("isHit");
		return tookDamage;

	}

	public override void PerformAction(CombatAction action)
	{
		if (action.damage > 0)
			anim.SetTrigger("isAttacking");
	}
	public override void ReceiveBasicAction(CombatAction action)
	{
		base.ReceiveBasicAction(action);
		if (Health <= 0)
		{
			foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
			{
				GameObject deaththrow = Instantiate(DeathPrefab, sr.transform.position, sr.transform.rotation);
				deaththrow.GetComponent<SpriteRenderer>().sprite = sr.sprite;
				deaththrow.transform.localScale = sr.transform.lossyScale;
			}

			SettingsManager.DeleteEnemy(name);

			Destroy(gameObject);
		}
	}
	/// <summary>
	/// Randomly selects and creates files for a number of attacks
	/// </summary>
	/// <returns>Attacks that it will use next turn</returns>
	public CombatAction[] PrepareAttack()
	{
		preppedActions = new CombatAction[Random.Range(1, maxAttacks + 1)];
		for (int i = 0; i < preppedActions.Length; i++)
		{
			preppedActions[i] = loadedActions[Random.Range(0, loadedActions.Count)];
			SettingsManager.CreateWeaponFile(preppedActions[i], "", "Enemy - " + Name + "\\Next Attack");
		}
		return preppedActions;
	}
}
