using UnityEngine;

public class LightFade : MonoBehaviour
{
	public float rate = 4.0f;
	
	void Update()
	{
		GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, 0.0f, Time.deltaTime * rate);
	}
}
