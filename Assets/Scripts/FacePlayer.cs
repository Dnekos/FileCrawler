using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
	Transform Player;
    // Start is called before the first frame update
    void Start()
    {
		Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
		transform.LookAt(Player);
    }
}
