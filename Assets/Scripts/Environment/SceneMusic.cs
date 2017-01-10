using UnityEngine;

public class SceneMusic : MonoBehaviour
{
	public AudioClip clip;
	
	void Awake()
	{
		Invoke("LoadMusic", 0.1f);
	}

	void LoadMusic()
	{
		if (clip)
			Music.ChangeTrack(clip);
	}
}
