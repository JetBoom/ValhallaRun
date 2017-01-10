using UnityEngine;

public static class UtilVector3
{
	public static Vector3 LerpSnap(Vector3 v1, Vector3 v2, float t, float snapdistance)
	{
		Vector3 lerped = Vector3.Lerp(v1, v2, t);

		if (Vector3.Distance(lerped, v2) <= snapdistance)
			return v2;

		return lerped;
	}
	public static Vector3 LerpSnap(Vector3 v1, Vector3 v2, float t) { return LerpSnap(v1, v2, t, 0.01f); }
}
