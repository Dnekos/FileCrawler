using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
	[Header("Directions"), SerializeField]
	public DungeonRoom[] ConnectedRooms;

	[Header("Whats inside?"), SerializeField]
	CombatAction[] Loot;
	[SerializeField]
	public EnemyStats[] PresentEnemies;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
	public void OnRoomEnter()
	{
		if (PresentEnemies != null)
		{

		}
		else
		{
			CreateChest();
		}
	}
	void CreateChest()
	{
		if (Loot.Length > 0)
		{
			SettingsManager.CreateDirectory("Treasure Chest");
			foreach (CombatAction wep in Loot)
				SettingsManager.CreateWeaponFile(wep, "", "\\Treasure Chest");
		}

	}
	public void LeaveRoom()
	{
		SettingsManager.DeleteDirectory("Treasure Chest");
	}

}
