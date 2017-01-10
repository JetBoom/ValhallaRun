using UnityEngine;

// Starts the level when the player exits me so the player has time to look around.

public class LevelStartTrigger : MonoBehaviour
{
	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
			collider.gameObject.SendMessage("DoStartLevel");
	}
}
