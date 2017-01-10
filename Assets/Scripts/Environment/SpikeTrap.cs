using UnityEngine;

// Effect for the spike traps. Makes the spikes jut in and out of the ground in a wave fashion.

public class SpikeTrap : MonoBehaviour
{
	void Update()
	{
		float i = 0.0f;
		foreach (Transform trans in transform)
		{
			i += 1.0f;

			trans.localScale = new Vector3(0.2f, 0.2f, 1.0f - Mathf.Abs(Mathf.Sin(Time.time * 4.0f + i * 0.5f)) * 0.2f);
		}
	}
}
