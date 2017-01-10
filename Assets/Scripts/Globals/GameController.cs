using UnityEngine;

public class GameController : MonoBehaviour
{
	public string[] tags;
	public GameObject[] prefabs;

	private static GameController controller;

	private static bool loadedPersistLevel;

	void Start()
	{
		if (controller == null)
		{
			controller = this;
			DontDestroyOnLoad(gameObject);
		}	
		else
			DestroyImmediate(gameObject);
	}

	void Awake()
	{
		SpawnObjects();

		if (!loadedPersistLevel)
		{
			if (Application.loadedLevelName != "_persist")
				Application.LoadLevelAdditive("_persist");

			loadedPersistLevel = true;
		}
	}

	void OnLevelWasLoaded()
	{
		SpawnObjects();
	}

	private void SpawnObjects()
	{
		for (int i = 0; i < tags.Length; i++)
			if (GameObject.FindGameObjectWithTag(tags[i]) == null)
				Instantiate(prefabs[i]);
	}
}
