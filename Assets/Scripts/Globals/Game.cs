using UnityEngine;

namespace Game
{
	public static class Constants
	{
		public const float transitionBottomY = 72f;
		public const float transitionTopY = 512f;
		public const float fogDensitySkyArea = 0.025f;
		public const float fogDensityGround = 0.005f;
	}

	public static class NetworkSettings
	{
		public const int playerNameMinLength = 3;
		public const int playerNameMaxLength = 40;
	}

	public static class Settings
	{
		private static float m_musicVolume = 0.5f;
		private static float m_lookSensitivity = 1.0f;

		public static float lookSensitivity
		{
			set { m_lookSensitivity = Mathf.Clamp(value, 0.1f, 10.0f); }
			get { return m_lookSensitivity; }
		}
		public static float musicVolume
		{
			set
			{
				value = Mathf.Clamp01(value);

				foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Music"))
					obj.GetComponent<AudioSource>().volume = value;

				m_musicVolume = value;
			}
			get { return m_musicVolume; }
		}

		public static void Save()
		{
			PlayerPrefs.SetFloat("look_sensitivity", lookSensitivity);
			PlayerPrefs.SetFloat("music_volume", musicVolume);

			PlayerPrefs.Save();
		}

		public static void Load()
		{
			m_lookSensitivity = PlayerPrefs.GetFloat("look_sensitivity", m_lookSensitivity);
			m_musicVolume = PlayerPrefs.GetFloat("music_volume", m_musicVolume);
		}
	}
}
