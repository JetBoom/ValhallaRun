using UnityEngine;

public class Spinning : MonoBehaviour
{
	public float spinRate = 90.0f;

	private Vector3 basePos;

	void Update()
	{
		transform.Rotate(0.0f, Time.deltaTime * spinRate, 0.0f);
	}
}
