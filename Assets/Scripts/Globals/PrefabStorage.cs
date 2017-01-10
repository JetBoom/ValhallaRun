using UnityEngine;
using System.Collections.Generic;

public static class PrefabStorage
{
	private static Dictionary<int, GameObject> prefabs = new Dictionary<int, GameObject>();
	private static Dictionary<string, int> prefabNames = new Dictionary<string, int>();
	
	public static void Add(GameObject obj)
	{
		if (prefabs.ContainsValue(obj))
			return;

		int id = prefabs.Count + 1;

		prefabs.Add(id, obj);
		prefabNames.Add(obj.name, id);
	}

	public static GameObject Get(string name)
	{
		int id = Translate(name);
		if (id >= 1)
			return Get(id);

		return null;
	}

	public static GameObject Get(int id)
	{
		if (prefabs.ContainsKey(id))
			return prefabs[id];

#if UNITY_EDITOR
			Debug.LogWarning("Prefab " + id + " doesn't exist in PrefabStorage!");
#endif

		return null;
	}

	public static GameObject Instantiate(int id)
	{
		return Instantiate(id, Vector3.zero, Quaternion.identity);
	}

	public static GameObject Instantiate(string name)
	{
		return Instantiate(Translate(name));
	}

	public static GameObject Instantiate(int id, Vector3 position, Quaternion rotation)
	{
		GameObject prefab = Get(id);
		return GameObject.Instantiate(prefab, position, rotation) as GameObject;
	}

	public static GameObject Instantiate(string name, Vector3 position, Quaternion rotation)
	{
		return Instantiate(Translate(name), position, rotation);
	}

	public static GameObject Instantiate(int id, Vector3 position, Quaternion rotation, float lifetime)
	{
		GameObject prefab = Get(id);
		GameObject obj = GameObject.Instantiate(prefab, position, rotation) as GameObject;
		if (obj)
			GameObject.Destroy(obj, lifetime);

		return obj;
	}

	public static GameObject Instantiate(string name, Vector3 position, Quaternion rotation, float lifetime)
	{
		return Instantiate(Translate(name), position, rotation, lifetime);
	}

	private static int Translate(string name)
	{
		if (prefabNames.ContainsKey(name))
			return prefabNames[name];

		return 0;
	}
}
