using UnityEngine;

public class Music : MonoBehaviour
{
	private static Music controller;

	private AudioClip targetClip;
	private float targetTime;

	private const float changeTime = 2.0f;

	void Start()
	{
		controller = this;

		DontDestroyOnLoad(gameObject);

		GetComponent<AudioSource>().volume = Game.Settings.musicVolume;
	}

	void Awake()
	{
	}

	void Update()
	{
		/*if (Time.timeScale >= 0.01f)
			audio.pitch = Time.timeScale;
		else
			audio.pitch = 1.0f;*/

		if (targetClip != null)
		{
			float diff = Mathf.Abs(Time.time - targetTime);
			GetComponent<AudioSource>().volume = Mathf.Clamp01(diff / changeTime) * Game.Settings.musicVolume;

			if (Time.time >= targetTime && targetClip != GetComponent<AudioSource>().clip)
			{
				GetComponent<AudioSource>().clip = targetClip;
				GetComponent<AudioSource>().Play();
			}

			if (diff > changeTime)
			{
				targetClip = null;
				targetTime = 0.0f;
			}
		}
	}

	public static void ChangeTrack(AudioClip clip)
	{
		if (!controller || controller.gameObject.GetComponent<AudioSource>().clip == clip)
			return;

		controller.targetTime = Time.time + changeTime;
		controller.targetClip = clip;
	}
}
