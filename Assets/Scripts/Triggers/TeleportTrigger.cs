using UnityEngine;

// Player teleporter trigger.

public class TeleportTrigger : MonoBehaviour
{
	// The other teleporter.
	public TeleportTrigger destination;

	// Should we override the camera angles when they teleport?
	public bool setAngles = true;

	// Should we set their velocity to 0 when they teleport?
	public bool clearVelocity = true;

	// This is for making it so the player doesn't instantly get teleported back as soon as they go over to the other one.
	[HideInInspector]
	public bool dontTeleport;

	void OnTriggerEnter(Collider collider)
	{
		GameObject obj = collider.gameObject;
		if (obj.tag == "Player" && destination != null && !dontTeleport)
		{
			destination.dontTeleport = true; // Tell other teleporter to not teleport until the NEXT time they get touched.

			obj.transform.position = destination.transform.position;

			if (destination.clearVelocity)
				obj.GetComponent<Rigidbody>().velocity = Vector3.zero;

			// Tell camera to use the destination's angles.
			if (destination.setAngles)
				obj.transform.rotation = destination.transform.rotation;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		// OK, it's safe to clear this flag.
		if (collider.gameObject.tag == "Player")
			dontTeleport = false;
	}
}
