using UnityEngine;

public static class ExtensionVector3
{
	public static Vector3 NearestPointToLine(this Vector3 vOrigin, Vector3 vA, Vector3 vB, bool mustBeOnLine)
	{
		Vector3 vVector1 = vOrigin - vA;
		Vector3 vVector2 = vB - vA;
		vVector2.Normalize();
 
		float d = Vector3.Distance(vA, vB);
		float t = Vector3.Dot(vVector2, vVector1);
 
		if (t <= 0)
			return mustBeOnLine ? Vector3.zero : vA;
 
	    if (t >= d)
	        return mustBeOnLine ? Vector3.zero : vB;
  
		return vA + vVector2 * t;
	}

	public static Vector3 NearestPointToLine(this Vector3 vOrigin, Vector3 vA, Vector3 vB)
	{
		return NearestPointToLine(vOrigin, vA, vB, false);
	}

	public static Vector3 Up(this Vector3 forward)
	{
		Quaternion rot = Quaternion.LookRotation(forward);
		Vector3 ang = rot.eulerAngles;
		ang.x -= 90.0f;

		return Quaternion.Euler(ang) * Vector3.forward;
	}

	public static Vector3 ClampMagnitude(this Vector3 vec, float maxmagnitude)
	{
		if (vec.magnitude > maxmagnitude)
		{
			vec.Normalize();
			vec *= maxmagnitude;

			return vec;
		}

		return vec;
	}

	public static Vector3 ClampMagnitude01(this Vector3 vec)
	{
		return vec.ClampMagnitude(1.0f);
	}

	/*public static Vector3 MajorAxis(this Vector3 forward)
	{
		Vector3 major;

		float x = Mathf.Abs(forward.x);
		float y = Mathf.Abs(forward.y);
		float z = Mathf.Abs(forward.z);
		float max = Mathf.Max(x, y, z);

		if (max == x)
		{
			major = Vector3.right;
			if (major == forward)
				major = Vector3.forward;
		}
		else if (max == z)
		{
			major = Vector3.forward;
			if (major == forward)
				major = Vector3.up;
		}
		else
		{
			major = Vector3.up;
			if (major == forward)
				major = Vector3.right;
		}

		return major;
	}*/
}
