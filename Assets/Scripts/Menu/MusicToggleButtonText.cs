using UnityEngine;
using UnityEngine.UI;

// Button on the main menu for toggling the music on and off.

public class MusicToggleButtonText : MonoBehaviour
{
	public Text textToChange;

	private void RefreshText()
	{
		if (textToChange)
		{
			if (Game.Settings.musicVolume > 0)
				textToChange.text = "Music: ON";
			else
				textToChange.text = "Music: OFF";
		}
	}

	void Awake()
	{
		RefreshText();
	}

	public void Toggle()
	{
		Game.Settings.musicVolume = Game.Settings.musicVolume == 0.5f ? 0.0f : 0.5f;
		Game.Settings.Save();

		RefreshText();
	}

	void Update()
	{
		if (textToChange)
			textToChange.color = Color.Lerp(Color.red, Color.green, Game.Settings.musicVolume * 2f);
	}
}
