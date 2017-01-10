using UnityEngine;

// Kills the player when the touch me.
// Can have different death types for various effects.

public class KillTrigger : MonoBehaviour
{
	public DeathType deathType;

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
			collider.gameObject.SendMessage("DoPlayerDeath", deathType);
	}
}
