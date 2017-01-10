using UnityEngine;

public class Floating : MonoBehaviour
{
	public float floatRate = 2.0f;
	public float floatAmount = 0.5f;

	private Vector3 basePos;

	void Awake()
	{
		basePos = transform.localPosition;
	}

	void Update()
	{
		transform.localPosition = basePos + new Vector3(0.0f, Mathf.Sin(Time.time * floatRate) * floatAmount, 0.0f);
	}
}
