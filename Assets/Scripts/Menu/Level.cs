using UnityEngine;

// Stores info about a particular level such as its scene, par time, collectable count, etc.

public class Level : MonoBehaviour
{
	public string Scene = "";
	public float ParTime = 0.0f;
	public int Collectables = 0;

	[HideInInspector]
	public string Name = "Unnamed";
	[HideInInspector]
	public float BestTime = 0.0f;
	[HideInInspector]
	public int BestCollectables = 0;
	[HideInInspector]
	public int BestComboLost = 0;

	void Start()
	{
		Name = gameObject.name;
	}

	void Awake()
	{
		CheckLevelWasLoaded();
	}

	void OnLevelWasLoaded(int levelid)
	{
		CheckLevelWasLoaded();
	}

	private void CheckLevelWasLoaded()
	{
		if (Application.loadedLevelName == Scene)
			LevelController.Controller.SetCurrentLevel(this);
	}
}
