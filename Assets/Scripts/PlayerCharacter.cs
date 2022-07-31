using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterStats
{

	public PlayerCharacter(string name, int health, int energy, int defense = 0) : base(name, health, energy, defense)
	{

	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	IEnumerator DamageShake()
	{
		
		yield return new WaitForSeconds(.1f);

	}
}
