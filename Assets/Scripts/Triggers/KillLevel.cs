using UnityEngine;

// Tracks the player to see if they fall below a certain Y value. Kills them if so.

public class KillLevel : MonoBehaviour
{
	private GameObject player;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update()
	{
		if (player != null && player.transform.position.y <= transform.position.y)
			player.SendMessage("DoPlayerDeath", DeathType.Fall, SendMessageOptions.DontRequireReceiver);
	}
}
