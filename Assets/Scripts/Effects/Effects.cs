using UnityEngine;

public static class Effects
{
	public static void Dispatch(string name, Vector3 position, Quaternion rotation)
	{
		PrefabStorage.Instantiate(name, position, rotation);
	}
}
