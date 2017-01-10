using UnityEngine;

// Tutorial trigger. Displays helpful information on the screen while the player is inside of it.

public class Tutorial : MonoBehaviour
{
	[Multiline]
	public string Text = "";

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			GameObject obj = GameObject.FindGameObjectWithTag("GameUI");
			if (obj)
				obj.GetComponent<GameHUD>().StartTutorial(this);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			GameObject obj = GameObject.FindGameObjectWithTag("GameUI");
			if (obj)
				obj.GetComponent<GameHUD>().EndTutorial(this);
		}
	}
}
