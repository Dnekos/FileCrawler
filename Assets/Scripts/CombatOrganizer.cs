using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[System.Serializable]
public class CombatAction
{
	public string name;
	public int damage;
	public int energy;
	public int defense;
	public int speed;
	public enum Target
	{
		First,
		Last,
		Self,
		Random,
		All
	}
	public Target targetting;
	[HideInInspector]
	public CharacterStats Owner;

	public CombatAction(string name = "", int damage = 0, int energy = 0, int defense = 0, int speed = 0, Target targetting = Target.First)
	{
		this.name = name;
		this.damage = damage;
		this.energy = energy;
		this.speed = speed;
		this.defense = defense;
		this.targetting = targetting;
	}
}

public class CombatOrganizer : MonoBehaviour
{
	[SerializeField, Header("Stats")]
	CharacterStats Player;
	Movement PlayerMovement;
	int Energy = 10;

	[SerializeField]
	List<EnemyStats> Enemies;

	[SerializeField, Header("Component")] SettingsManager windowguy;
	public void BeginCombat()
	{
		string[] readied = windowguy.GetReadiedFiles();
		windowguy.CleanReadiedActions();

		List<CombatAction> ReadiedActions = Player.loadedActions.Where(a => readied.Select(x => x.Substring(x.LastIndexOf('\\') + 1)).Contains(a.name)).ToList();

		for (int i = 0; i < Enemies.Count; i++)
		{
			ReadiedActions.AddRange(Enemies[i].preppedActions);
			Enemies[i].PrepareAttack();
		}

		StartCoroutine(ActionStuff(ReadiedActions));
	}

	IEnumerator ActionStuff(List<CombatAction> readiedActions)
	{
		// run through actions and give time for animations
		foreach (CombatAction action in readiedActions.OrderByDescending(x => x.speed))
		{
			AttemptAction(action);
			Debug.Log(action.name);
			yield return new WaitForSeconds(1);
		}

		// Sweep for dead enemies
		for (int i = Enemies.Count - 1; i >= 0; i--)
		{
			if (Enemies[i] == null)
				Enemies.RemoveAt(i);
		}

		// end or continue combat
		if (Enemies.Count == 0)
			PlayerMovement.EndCombat();
		else
			windowguy.FocusExplorer();
	}
	void AttemptAction(CombatAction action)
	{
		if (action.Owner != null)
		{
			action.Owner.Energy = Mathf.Max(0, Energy - action.energy);
			if (action.Owner.Energy >= 0)
			{
				action.Owner.PerformAction(action);

				if (action.targetting == CombatAction.Target.Self)
					PerformAction(action.Owner, action);
				else if (action.Owner == Player)
					PlayerTargeting(action);
				else
					PerformAction(Player, action);
			}
			else
				Debug.Log("Not enough energy");
		}
		else
			Debug.Log("ya dead");
	}
	void PerformAction(CharacterStats target, CombatAction action)
	{
		if (target != null)
			target.ReceiveBasicAction(action);
		else
			Debug.Log("action failed");
	}

	void PlayerTargeting(CombatAction action)
	{
		switch (action.targetting)
		{
			case CombatAction.Target.All:
				for (int i = Enemies.Count - 1; i >= 0; i--)
					PerformAction(Enemies[i], action);
				break;
			case CombatAction.Target.First:
				PerformAction(Enemies[0], action);
				break;
			case CombatAction.Target.Last:
				PerformAction(Enemies[Enemies.Count - 1], action);
				break;
			case CombatAction.Target.Random:
				PerformAction(Enemies[Random.Range(0, Enemies.Count)], action);
				break;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		PlayerMovement = FindObjectOfType<Movement>();
		//Player = new CharacterStats("Player", 10, 10);
		CreateStarterEquipment();

		//InitiateEncounter();
	}

	void CreateStarterEquipment()
	{
		string shield = "\\_________________/\n"
					  + "|       | |       |\n"
					  + "|       | |       |\n"
					  + "|       | |       |\n"
					  + "|_______| |_______|\n"
					  + "|_______   _______|\n"
					  + "|       | |       |\n"
					  + "|       | |       |\n"
					  + " \\      | |      / \n"
					  + "  \\     | |     /  \n"
					  + "   \\    | |    /   \n"
					  + "    \\   | |   /    \n"
					  + "     \\  | |  /     \n"
					  + "      \\ | | /      \n"
					  + "       \\| |/       \n"
					  + "        \\_/        ";
		Player.AddAction(new CombatAction(name: "Shield", energy: 3, defense: 4, speed: 5, targetting: CombatAction.Target.Self));
		SettingsManager.CreateWeaponFile(Player.loadedActions[0], shield, "", false);

		string body = "            _                                                 \n"
					+ " _         | |                                                \n"
					+ "| | _______| | ---------------------------------------------\\ \n"
					+ "| -)_______|==[] ============================================>\n"
					+ "|_|        | | ---------------------------------------------/ \n"
					+ "           |_|                                                ";
		Player.AddAction(new CombatAction(name: "Sword", damage: 3, energy: 6, targetting: CombatAction.Target.First));
		SettingsManager.CreateWeaponFile(Player.loadedActions[1], body, "", false);
	}

	public bool StartAFight(EnemyStats[] presentEnemies)
	{
		if (presentEnemies.Length == 0)
			return false;
		Enemies.AddRange(presentEnemies);
		foreach (EnemyStats enemy in Enemies)
		{
			windowguy.CreateEnemyDirectory(enemy.Name);
			enemy.PrepareAttack();

		}
		return true;
	}

	void AddEnemy()
	{
		Enemies[0].AddAction(new CombatAction("Shoddy Spear", 2, 3, 0, 1));
		windowguy.CreateEnemyDirectory(Enemies[0].Name);
		Enemies[0].PrepareAttack();
	}
	void CreateEnemy()
	{
		EnemyStats enemy = new EnemyStats("Goblin", 5, 5, 2,1);
		enemy.AddAction(new CombatAction("Shoddy Spear", 2, 3, 0, 1));
		windowguy.CreateEnemyDirectory(enemy.Name);
		enemy.PrepareAttack();
		Enemies.Add(enemy);
	}
}
