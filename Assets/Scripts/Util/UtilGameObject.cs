using UnityEngine;

public static class UtilGameObject
{
	public static void NoCollide(Component o1, Component o2, bool nocollide = true)
	{
		if (o1.GetHasActiveCollider() && o2.GetHasActiveCollider())
			Physics.IgnoreCollision(o1.GetComponent<Collider>(), o2.GetComponent<Collider>(), nocollide);
	}
}
