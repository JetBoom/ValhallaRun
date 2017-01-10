using UnityEngine;

public class SkyCamera : MonoBehaviour
{
	void Awake()
	{
		if (Camera.main != null)
		{
			GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
			Camera.main.clearFlags = CameraClearFlags.Depth;
		}
	}

	void Update()
	{
		if (Camera.main != null)
		{
			transform.localPosition = Camera.main.transform.position * 0.002f;
			transform.rotation = Camera.main.transform.rotation;
		}
	}

	void OnPreRender()
	{
		RenderSettings.fogDensity *= 2.5f;
	}

	void OnPostRender()
	{
		RenderSettings.fogDensity /= 2.5f;
	}
}
