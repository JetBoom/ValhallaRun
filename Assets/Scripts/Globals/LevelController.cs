using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is spawned once and stores all of the games levels.

public class LevelController : MonoBehaviour
{
	public static LevelController Controller
	{
		get
		{
			return GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>();
		}
	}

	public static Level currentLevel;

	private static List<Level> LevelList;

	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	void Awake()
	{
		LevelList = new List<Level>();

		foreach (Transform child in transform)
		{
			Level level = child.GetComponent<Level>();
			if (level != null)
				LevelList.Add(level);
		}
	}

	public static Level GetLevelByName(string name)
	{
		foreach (Level level in LevelList)
		{
			if (level.Name == name)
				return level;
		}

		return null;
	}

	public void SetCurrentLevel(Level level)
	{
		if (currentLevel == level)
			return;
		
		currentLevel = level;

		GameObject obj = GameObject.FindGameObjectWithTag("GameUI");
		if (obj)
			obj.GetComponent<GameHUD>().LevelStarted(level);
	}
}
