using UnityEngine;

// These are the yellow lightning bolts the player picks up.

public class Collectable : MonoBehaviour
{
	// FX made when player picks me up.
	public GameObject pickupEffect;

	// Stores every collectable on the map.
	public static GameObject[] AllCollectables;

	private Vector3 rotation;

	void Start()
	{
		rotation = new Vector3(45.0f, 0.0f, 45.0f);

		transform.rotation = Quaternion.Euler(Random.insideUnitSphere);
	}

	void Awake()
	{
		AllCollectables = GameObject.FindGameObjectsWithTag("Collectable");
	}

	void Update()
	{
		transform.Rotate(rotation * Time.deltaTime);
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			// Tell the player we picked up a collectable.
			collider.gameObject.SendMessage("OnTouchCollectable", gameObject);

			// Make the FX.
			if (pickupEffect)
				Instantiate(pickupEffect, transform.position, transform.rotation);

			// Hide (but don't delete) me.
			gameObject.SetActive(false);

			// Level creation purposes.
			int count = 0;
			foreach (GameObject obj in Collectable.AllCollectables)
				if (!obj.activeSelf)
					count++;
			print("Collectables: " + count + " / " + Collectable.AllCollectables.Length);
		}
	}
}
