using UnityEngine;
using System.Collections;

public class NexusLevelGate : MonoBehaviour
{
	public TextMesh levelTextMesh;
	public string LevelScene = "";

	void Update()
	{
		Vector3 campos = Camera.main.transform.position;
		float dist = Vector3.Distance(campos, transform.position);
		float a = Mathf.Clamp(1.25f - dist * 0.02f, 0f, 1f);

		levelTextMesh.color = new Color(1f, 1f, 1f, a);

		if (a > 0f)
		{
			levelTextMesh.transform.LookAt(campos);
			levelTextMesh.transform.forward = levelTextMesh.transform.forward * -1;
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
			Application.LoadLevel(LevelScene);
	}
}
