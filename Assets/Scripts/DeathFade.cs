using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFade : MonoBehaviour
{
	[SerializeField] float WhiteSpeed = 2, Deathspeed = 1;
	[SerializeField] SpriteRenderer WhiteSprite, EnemySprite;
	
    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine(Fade());
    }
	IEnumerator Fade()
	{
		Color col = WhiteSprite.color;
		col.a = 0;
		while (col.a < 1)
		{
			col.a += Time.deltaTime * WhiteSpeed;
			WhiteSprite.color = col;
			yield return null;
		}
		col.a = 1;
		while (col.a > 0)
		{
			col.a -= Time.deltaTime * Deathspeed;
			WhiteSprite.color = col;
			EnemySprite.color = col;
			yield return null;
		}
		Destroy(gameObject);
	}
}
