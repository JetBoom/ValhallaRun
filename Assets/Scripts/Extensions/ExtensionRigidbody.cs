using UnityEngine;

// Just some extension methods because I don't like setting the ForceMode every time.

public static class ExtensionRigidbody
{
	public static void AddVelocity(this Rigidbody body, Vector3 velocity)
	{
		body.AddForce(velocity, ForceMode.VelocityChange);
	}

	public static void AddVelocity(this Rigidbody body, float x, float y, float z)
	{
		body.AddVelocity(new Vector3(x, y, z));
	}
}
