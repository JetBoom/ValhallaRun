using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Game;

public class WeatherController : MonoBehaviour
{
	/*private bool inSkyArea = false;

	void Awake()
	{
		SwitchShadows();
	}

	void SwitchShadows()
	{
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("WorldSkyShadowSwitch"))
		{
			MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
			if (renderer)
				renderer.shadowCastingMode = inSkyArea ? ShadowCastingMode.On : ShadowCastingMode.Off;
		}
	}*/

	/*void Update()
	{
		Camera cam = Camera.main;
		if (!cam)
			return;

		Vector3 campos = cam.transform.position;
		float camY = campos.y;

		bool newstate = false;

		if (camY <= Constants.transitionBottomY)
			RenderSettings.fogDensity = Constants.fogDensityGround;
		else if (camY >= Constants.transitionTopY)
		{
			RenderSettings.fogDensity = Constants.fogDensitySkyArea;
			newstate = true;
		}
		else
			RenderSettings.fogDensity = Mathf.Lerp(Constants.fogDensityGround, Constants.fogDensitySkyArea, (camY - Constants.transitionBottomY) / (Constants.transitionTopY - Constants.transitionBottomY));

		if (newstate != inSkyArea)
		{
			inSkyArea = newstate;

			SwitchShadows();
		}
	}*/
}
