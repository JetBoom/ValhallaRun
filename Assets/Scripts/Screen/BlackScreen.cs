using UnityEngine;

// Draws a black screen and slowly fades out. Self-destructs when fade out is done.

public class BlackScreen : MonoBehaviour
{
	public float fadePerSecond = 0.5f;

	private float lerp;

	void Start()
	{
		lerp = 1.0f;
	}

	void Update()
	{
		lerp -= Time.unscaledDeltaTime * fadePerSecond;

		if (lerp <= 0.0f)
			Destroy(this);
	}

	void OnGUI()
	{
		UtilGUI.DrawRectangle(new Rect(0, 0, Screen.width, Screen.height), Color.black, Mathf.Pow(lerp, 0.7f));
	}
}
