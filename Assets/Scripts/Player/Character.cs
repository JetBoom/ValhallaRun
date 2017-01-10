using UnityEngine;

public class Character : MonoBehaviour
{
	public Transform renderTransform;

	//public CharacterMotor motor { get; set; }
	//public CharacterInput input { get; set; }

	public bool isDead;

	public Transform eyePosition { get; set; }

	void Start()
	{
		//motor = GetComponent<CharacterMotor>();
		//input = GetComponent<CharacterInput>();
	}

	void Awake()
	{
		eyePosition = gameObject.GetComponentByNameInChildren<Transform>("eyes");
	}
}
