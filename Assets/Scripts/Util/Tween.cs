using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Tweening controller.
// Allows the assigning of a list of transforms to tween through.
// Can use Bezier or CatMullRom. CatMullRom is better for looping tracks. Beziers are a bit smoother.

public class Tween : MonoBehaviour
{
	// Should be a blank game object with child gameobjects acting as the transforms.
	public GameObject transformList;

	// How long should the track take to complete? Not used if using CatMullRom.
	public float duration = 10.0f;

	// How many slices? If above 0 then will not use Bezier. More slices = longer duration.
	public int catmullSlices = 0;

	private IEnumerator<Vector3> pos_sequence;
	private IEnumerator<Vector3> ang_sequence;

	void Start()
	{
		ResetSequence();
	}

	void FixedUpdate()
	{
		if (pos_sequence == null)
			ResetSequence();
		else
		{
			if (pos_sequence.MoveNext())
				transform.position = pos_sequence.Current;
			if (ang_sequence.MoveNext())
				transform.rotation = Quaternion.Euler(ang_sequence.Current);
		}
	}

	void ResetSequence()
	{
		// Extracts and prepares a list of vectors and angles to use on the tracks.

		List<Transform> transforms = new List<Transform>();
		foreach (Transform trans in transformList.transform)
			transforms.Add(trans);

		if (catmullSlices <= 0)
		{
			Vector3[] positions = new Vector3[transforms.Count + 1];
			Vector3[] angles = new Vector3[transforms.Count + 1];

			int i = 0;
			foreach (Transform trans in transforms)
			{
				positions[i] = trans.position;
				angles[i] = trans.rotation.eulerAngles;
				i++;
			}

			positions[transforms.Count] = transforms[0].position;
			angles[transforms.Count] = transforms[0].rotation.eulerAngles;

			pos_sequence = Interpolate.NewBezier(Interpolate.Ease(Interpolate.EaseType.Linear), positions, duration).GetEnumerator();
			ang_sequence = Interpolate.NewBezier(Interpolate.Ease(Interpolate.EaseType.Linear), angles, duration).GetEnumerator();

			Invoke("ResetSequence", duration);
		}
		else
		{
			Vector3[] positions = new Vector3[transforms.Count];
			Vector3[] angles = new Vector3[transforms.Count];

			int i = 0;
			foreach (Transform trans in transforms)
			{
				positions[i] = trans.position;
				angles[i] = trans.rotation.eulerAngles;
				i++;
			}

			pos_sequence = Interpolate.NewCatmullRom(positions, catmullSlices, true).GetEnumerator();
			ang_sequence = Interpolate.NewCatmullRom(angles, catmullSlices, true).GetEnumerator();
		}
	}
}
