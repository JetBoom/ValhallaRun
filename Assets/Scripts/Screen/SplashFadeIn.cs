using UnityEngine;

// Splash screen controller.

public class SplashFadeIn : MonoBehaviour
{
	enum STATE {FADEIN, PAUSED, FADEOUT};

	public float FadeTime = 1.5f;
	public float PauseTime = 3.0f;
	public bool Skippable = true;

	private float lerp;
	private float pausedTime;
	private STATE state;

	void Start()
	{
		pausedTime = 0.0f;
		state = STATE.FADEIN;
	}

	void Update()
	{
		if (Skippable && state != STATE.FADEOUT && Input.anyKeyDown)
		{
			state = STATE.FADEOUT;
			lerp = 1.0f;
		}

		if (state == STATE.FADEIN)
		{
			lerp = Mathf.Min(1, lerp + Time.deltaTime / FadeTime);
			if (lerp == 1.0f)
				state = STATE.PAUSED;
		}
		else if (state == STATE.PAUSED)
		{
			lerp = 1.0f;
			pausedTime += Time.deltaTime;
			if (pausedTime >= PauseTime)
				state = STATE.FADEOUT;
		}
		else if (state == STATE.FADEOUT)
		{
			lerp = Mathf.Max(0.0f, lerp - Time.deltaTime / FadeTime);
			if (lerp == 0.0f)
			{
				Application.LoadLevel("main_menu");
				return;
			}
		}
	}

	void OnGUI()
	{
		UtilGUI.DrawRectangle(new Rect(0, 0, Screen.width, Screen.height), Color.black, 1.0f - lerp);
	}
}
