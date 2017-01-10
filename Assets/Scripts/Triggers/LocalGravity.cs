using UnityEngine;

public class LocalGravity : MonoBehaviour
{
	public enum FieldBehavior { Plane, Point, RotatingPlane };
	public enum FieldShape { Box, Sphere, Cylinder };

	[Range(-10.0f, 10.0f)]
	public float gravity = 1.0f;
	[Range(0.0f, 100.0f)]
	public float particleMultiplier = 1.0f;
	public FieldBehavior fieldBehavior = FieldBehavior.Plane;
	public bool resetGravityNormalOnLeave = true;
	public GameObject emitterObject;

	public Vector3 gravityNormal = Vector3.down;
	public Vector3 realGravityNormal { get { return transform.rotation * gravityNormal; } }

	public Transform rotatingPlanePointA;
	public Transform rotatingPlanePointB;

	//[HideInInspector]
	//public Vector3 desiredGravityNormal;

	// Field shapes don't have to do with how the gravity behaves, these are just for calculating particle amount.
	public float volume
	{
		get
		{
			if (fieldShape == FieldShape.Sphere)
				return VolumeOfSphere();

			if (fieldShape == FieldShape.Cylinder)
				return VolumeOfCylinder();

			return VolumeOfBox();
		}
	}

	private FieldShape fieldShape;
	private Vector3 defaultGravityNormal;

	void OnValidate()
	{
		DoEmissionSettings();
	}

	void Awake()
	{
		//desiredGravityNormal = defaultGravityNormal = gravityNormal;
		defaultGravityNormal = gravityNormal;

		DoEmissionSettings();
	}

	void Update()
	{
		/*if (desiredGravityNormal != gravityNormal)
			gravityNormal = Vector3.RotateTowards(gravityNormal, desiredGravityNormal, Time.deltaTime * 4.0f, 1.0f);*/

		if (emitterObject)
			emitterObject.transform.rotation = Quaternion.LookRotation(gravityNormal);
	}

	private void DoEmissionSettings()
	{
		if (gameObject.GetComponent<SphereCollider>() != null)
			fieldShape = FieldShape.Sphere;
		else if (gameObject.GetComponent<CapsuleCollider>() != null)
			fieldShape = FieldShape.Cylinder;
		else
			fieldShape = FieldShape.Box;

		if (emitterObject)
			emitterObject.GetComponent<ParticleSystem>().emissionRate = Mathf.Clamp(particleMultiplier * volume * 0.033f, 20.0f, 5000.0f);
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
			collider.gameObject.SendMessage("OnEnterLocalGravityZone", this);
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			collider.gameObject.SendMessage("OnExitLocalGravityZone", this);

			if (resetGravityNormalOnLeave)
				SetGravity(defaultGravityNormal);
		}
	}

	public void SetGravity(Vector3 dir, float amount, bool instant)
	{
		if (instant)
		{
			//gravityNormal = desiredGravityNormal = dir;
			gravityNormal = dir;
			if (emitterObject)
				emitterObject.transform.rotation = Quaternion.LookRotation(gravityNormal);
		}
		else
			gravityNormal = dir; //desiredGravityNormal = dir;

		gravity = amount;
	}

	public void SetGravity(Vector3 dir, float amount) { SetGravity(dir, amount, false); }
	public void SetGravity(Vector3 dir) { SetGravity(dir, gravity, false); }

	public Vector3 getLocalGravity(PlayerController player)
	{
		float mag = Physics.gravity.magnitude * gravity;

		if (fieldBehavior == FieldBehavior.Point)
			return (transform.position - player.GetComponent<Rigidbody>().worldCenterOfMass).normalized * mag;

		if (fieldBehavior == FieldBehavior.RotatingPlane)
		{
			Vector3 origin = player.GetComponent<Rigidbody>().position;
			Vector3 nearest = origin.NearestPointToLine(rotatingPlanePointA.position, rotatingPlanePointB.position);
			if (origin == nearest)
				return Vector3.zero;

			return (nearest - origin).normalized * mag;
		}

		return realGravityNormal * mag;
	}

	public float VolumeOfBox()
	{
		Vector3 scale = transform.localScale;
		return scale.x * scale.y * scale.z;
	}

	public float VolumeOfSphere()
	{
		return 4.1888f * Mathf.Pow(transform.localScale.magnitude / 2.0f, 3.0f); // 4/3*pi*r^3
	}

	public float VolumeOfCylinder()
	{
		return Mathf.PI * Mathf.Pow(transform.localScale.x * 2.0f, 2.0f) * transform.localScale.y * 2.0f;
	}
}