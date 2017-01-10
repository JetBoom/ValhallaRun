using UnityEngine;

public static class ExtensionGameobject
{
	public static Transform GetEyeTransform(this GameObject obj)
	{
		GameObject o = GetGameObjectByNameInChildren(obj, "eyes");
		if (o)
			return o.transform;

		return obj.transform;
	}

	public static Vector3 GetEyeForward(this GameObject obj)
	{
		return GetEyeTransform(obj).forward;
	}

	public static Vector3 GetEyePos(this GameObject obj)
	{
		return GetEyeTransform(obj).position;
	}

	public static bool GetHasActiveCollider(this GameObject obj)
	{
		return obj.GetComponent<Collider>() && obj.GetComponent<Collider>().enabled;
	}

	public static bool GetHasActiveCollider(this Component obj)
	{
		return obj.gameObject.GetHasActiveCollider();
	}

	public static GameObject GetGameObjectByNameInChildren(this GameObject obj, string name)
	{
		foreach (Transform trans in obj.transform)
		{
			if (trans.name == name)
				return trans.gameObject;

			GameObject o = trans.gameObject.GetGameObjectByNameInChildren(name);
			if (o != null)
				return o;
		}

		return null;
	}

	public static T GetComponentByNameInChildren<T>(this GameObject obj, string name) where T : Component
	{
		foreach (Transform trans in obj.transform)
		{
			if (trans.name == name)
				return trans.GetComponent<T>();

			T c = trans.gameObject.GetComponentByNameInChildren<T>(name);
			if (c != null)
				return c;
		}

		return null;
	}
}
