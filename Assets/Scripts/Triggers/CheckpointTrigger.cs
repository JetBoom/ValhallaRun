using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The checkpoints the player can use. Makes life a lot easier for players that aren't masters at the game.

public class CheckpointTrigger : MonoBehaviour
{
	// Store these to change their colors.
	public GameObject raido; // as in the Nordic rune "raido"
	public GameObject ring;
	public Light ambientLight;

	public Material onMaterial;
	public Material offMaterial;

	[HideInInspector]
	public bool activated
	{
		get { return m_activated; }
		set
		{
			if (value)
				TurnOn();
			else
				TurnOff();
		}
	}
	private bool m_activated;

	// This is a list of collectables that should be DEactivated when respawning here.
	[HideInInspector]
	public List<GameObject> collectables;

	void Start()
	{
		collectables = new List<GameObject>();
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player" && !collider.gameObject.GetComponent<PlayerController>().isDead)
			TurnOn();
	}

	void TurnOn()
	{
		if (activated)
			return;

		m_activated = true;

		// Turn off every other checkpoint.
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Checkpoint"))
		{
			if (obj != gameObject)
				obj.SendMessage("TurnOff");
		}

		// Store all of the collectables that have already been collected.
		collectables.Clear();
		foreach (GameObject obj in Collectable.AllCollectables)
			if (!obj.activeSelf)
				collectables.Add(obj);

		// Change the color of our children..
		foreach (Transform trans in ring.transform)
			if (trans.gameObject.GetComponent<Renderer>())
				trans.gameObject.GetComponent<Renderer>().material = onMaterial;
		foreach (Transform trans in raido.transform)
			if (trans.gameObject.GetComponent<Renderer>())
				trans.gameObject.GetComponent<Renderer>().material = onMaterial;

		// ..and the light.
		ambientLight.color = Color.green;

		GetComponent<AudioSource>().Play();
	}

	void TurnOff()
	{
		if (!activated)
			return;

		m_activated = false;

		// Change colors again.

		foreach (Transform trans in ring.transform)
			if (trans.gameObject.GetComponent<Renderer>())
				trans.gameObject.GetComponent<Renderer>().material = offMaterial;
		foreach (Transform trans in raido.transform)
			if (trans.gameObject.GetComponent<Renderer>())
				trans.gameObject.GetComponent<Renderer>().material = offMaterial;

		ambientLight.color = Color.red;
	}
}
