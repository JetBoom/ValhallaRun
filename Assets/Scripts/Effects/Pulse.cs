using UnityEngine;

// Oscilates the scale of an object through a sin wave.

public class Pulse : MonoBehaviour
{
	public float pulseRate = 2.0f;
	public float pulseAmount = 0.2f;

	private Vector3 baseScale;

	void Awake()
	{
		baseScale = transform.localScale;
	}

	void Update()
	{
		transform.localScale = baseScale * (1.0f + Mathf.Abs(Mathf.Sin(Time.time * pulseRate)) * pulseAmount);
	}
}
