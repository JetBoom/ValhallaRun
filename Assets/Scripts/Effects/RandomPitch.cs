using UnityEngine;

public class RandomPitch : MonoBehaviour
{
	public float minPitch = 0.9f;
	public float maxPitch = 1.1f;

	void Start ()
	{
		if (GetComponent<AudioSource>() != null)
			GetComponent<AudioSource>().pitch = Random.Range(minPitch, maxPitch);
	}
}
