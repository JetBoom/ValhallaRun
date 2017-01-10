using UnityEngine;

// Destroys this game object in <time> seconds.

public class KillOnAwake : MonoBehaviour
{
	public float time = 1.0f;

	void Awake()
	{
		if (time <= 0.0f)
			Destroy(gameObject);
		else
			Destroy(gameObject, time);
	}
}
