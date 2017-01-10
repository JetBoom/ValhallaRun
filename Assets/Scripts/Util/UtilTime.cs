using UnityEngine;

public static class UtilTime
{
	// Converts seconds in to MM:SS.MS
	public static string toMinutesSecondsMilliseconds(float t)
	{
		int minutes = Mathf.FloorToInt(t / 60.0f);
		t -= minutes * 60;
		int seconds = Mathf.FloorToInt(t);
		t -= seconds;
		int milliseconds = Mathf.RoundToInt(t * 1000.0f);

		return string.Format("{0}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);
	}

	// Converts seconds in to MM:SS
	public static string toMinutesSeconds(float t)
	{
		int minutes = Mathf.FloorToInt(t / 60.0f);
		t -= minutes * 60;
		int seconds = Mathf.FloorToInt(t);

		return string.Format("{0}:{1:D2}", minutes, seconds);
	}
}
