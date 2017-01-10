using UnityEngine;

// Simple trigger tells the player controller when the win the level.

public class GoalTrigger : MonoBehaviour
{
	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
			collider.gameObject.SendMessage("OnTouchGoal", gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
