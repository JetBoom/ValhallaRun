using UnityEngine;

// Just kills the game when the scene is loaded.
// Doesn't have a use in a web player.

public class KillGame : MonoBehaviour
{
	void Awake()
	{
		Application.Quit();
	}
}
