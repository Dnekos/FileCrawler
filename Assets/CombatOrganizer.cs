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
	int Energy = 10;
	[SerializeField]
	int PlayerHealth = 10;

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
			Enemies[i].PrepareAttack(windowguy);
		}
		
		foreach(CombatAction action in ReadiedActions.OrderBy(x => x.speed))
		{
			PerformAction(action);
			Debug.Log(action.name);
		}

	}

	void PerformAction(CombatAction action)
	{
		Energy = Mathf.Max(0, Energy - action.energy);
		if (Energy >= 0)
		{
			if (action.targetting == CombatAction.Target.Self)
				action.Owner.ReceiveBasicAction(action);
			else if (action.Owner == Player)
				PlayerTargeting(action);
			else
				Player.ReceiveBasicAction(action);

		}
	}
	void PlayerTargeting(CombatAction action)
	{
		switch (action.targetting)
		{
			case CombatAction.Target.All:
				for (int i = 0; i < Enemies.Count; i++)
					Enemies[i].ReceiveBasicAction(action);
				break;
			case CombatAction.Target.First:
				Enemies[0].ReceiveBasicAction(action);
				break;
			case CombatAction.Target.Last:
				Enemies[Enemies.Count - 1].ReceiveBasicAction(action);
				break;
			case CombatAction.Target.Random:
				Enemies[Random.Range(0,Enemies.Count)].ReceiveBasicAction(action);
				break;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		Player = new CharacterStats("Player", 10, 10);
		CreateStarterEquipment();

		InitiateEncounter();
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
		windowguy.CreateWeaponFile(Player.loadedActions[0], shield, "", false);

		string body = "            _                                                 \n"
					+ " _         | |                                                \n"
					+ "| | _______| | ---------------------------------------------\\ \n"
					+ "| -)_______|==[] ============================================>\n"
					+ "|_|        | | ---------------------------------------------/ \n"
					+ "           |_|                                                ";
		Player.AddAction(new CombatAction(name: "Sword", damage: 3, energy: 6, targetting: CombatAction.Target.First));
		windowguy.CreateWeaponFile(Player.loadedActions[1], body, "", false);
	}
	void InitiateEncounter()
	{
		Enemies = new List<EnemyStats>();
		CreateEnemy();

	}
	void CreateEnemy()
	{
		EnemyStats enemy = new EnemyStats("Goblin", 5, 5, 2,1);
		enemy.AddAction(new CombatAction("Shoddy Spear", 2, 3, 0, 1));
		windowguy.CreateEnemyDirectory(enemy.Name);
		enemy.PrepareAttack(windowguy);
		Enemies.Add(enemy);
	}
}
