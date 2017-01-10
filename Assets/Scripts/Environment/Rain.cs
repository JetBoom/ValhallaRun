using UnityEngine;

public class Rain : MonoBehaviour
{
	void Update()
	{
		if (Camera.main != null)
			transform.position = Camera.main.transform.position + Vector3.up * 60.0f;
	}
}
