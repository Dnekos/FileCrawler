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

	CharacterStats[] Enemies;

	[SerializeField]
	List<CombatAction> loadedActions;
	CombatAction[] ReadiedActions;

	[SerializeField, Header("Component")] SettingsManager windowguy;
	public void BeginCombat()
	{
		string[] readied = windowguy.GetReadiedFiles();
		windowguy.CleanReadiedActions();

		ReadiedActions = loadedActions.Where(a => readied.Select(x => x.Substring(x.LastIndexOf('\\') + 1)).Contains(a.name)).ToArray();//.OrderBy(x=>x.speed).ToArray();
		Debug.Log(ReadiedActions.Length);
		foreach(CombatAction action in ReadiedActions)
		{
			PerformAction(action);
			Debug.Log(action.name) ;
		}
	}

	void PerformAction(CombatAction action)
	{
		Energy = Mathf.Max(0, Energy - action.energy);
		if (Energy >= 0)
			PlayerHealth -= action.damage;
	}

    // Start is called before the first frame update
    void Start()
    {
		Player = new CharacterStats("Player", 10, 10);
		loadedActions = new List<CombatAction>();
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
		loadedActions.Add(new CombatAction(name: "Shield", energy: 3, defense: 4, speed: 5, targetting: CombatAction.Target.Self));
		windowguy.CreateWeaponFile(loadedActions[0], shield, "", false);

		string body = "            _                                                 \n"
					+ " _         | |                                                \n"
					+ "| | _______| | ---------------------------------------------\\ \n"
					+ "| -)_______|==[] ============================================>\n"
					+ "|_|        | | ---------------------------------------------/ \n"
					+ "           |_|                                                ";
		loadedActions.Add(new CombatAction(name: "Sword", damage: 3, energy: 6, targetting: CombatAction.Target.First));
		windowguy.CreateWeaponFile(loadedActions[1], body, "", false);
	}
	void InitiateEncounter()
	{
		CreateEnemy();
	}
	void CreateEnemy()
	{
		Enemies = new CharacterStats[] { new CharacterStats("Goblin", 5, 5, 2) };
	}
}
