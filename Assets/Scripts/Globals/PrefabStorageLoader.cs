using UnityEngine;

public class PrefabStorageLoader : MonoBehaviour
{
	public GameObject[] prefabsToLoad;

	void Start()
	{
		foreach (GameObject prefab in prefabsToLoad)
			PrefabStorage.Add(prefab);
	}
}
