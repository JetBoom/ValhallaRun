using UnityEngine;
using System.Collections;

public class SettingsLoader : MonoBehaviour
{
	private static bool loaded;

	void Awake()
	{
		if (loaded) return;
		loaded = true;

		Invoke("Load", 0.01f);
	}

	void Load()
	{
		Game.Settings.Load();
	}
}
