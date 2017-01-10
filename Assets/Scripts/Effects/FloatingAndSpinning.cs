using UnityEngine;

// Gives an object the look of floating and spinning. Useful for getting attention.

public class FloatingAndSpinning : MonoBehaviour
{
	public float floatRate = 2.0f;
	public float floatAmount = 0.5f;
	public float spinRate = 90.0f;

	private Vector3 basePos;

	void Awake()
	{
		basePos = transform.localPosition;
	}

	void Update()
	{
		transform.localPosition = basePos + new Vector3(0.0f, Mathf.Sin(Time.time * floatRate) * floatAmount, 0.0f);
		transform.Rotate(0.0f, Time.deltaTime * spinRate, 0.0f);
	}
}
