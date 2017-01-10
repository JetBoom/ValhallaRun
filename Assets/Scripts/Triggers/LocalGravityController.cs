#if NO
using UnityEngine;

public class LocalGravityController : MonoBehaviour
{
	public LocalGravity gravityZone;
	public GameObject shape;
	public Light shapeLight;

	private Vector3 defaultShapeScale;

	void Awake()
	{
		if (gravityZone && shape)
		{
			shape.renderer.material.color = shapeLight.color = Color.red;
			defaultShapeScale = shape.transform.localScale;
		}
	}

	void Update()
	{
		if (gravityZone && shape)
		{
			shape.renderer.material.color = shapeLight.color = Color.Lerp(Color.red, Color.green, Mathf.Max(Vector3.Dot(gravityZone.gravityNormal, -transform.up), 0.0f));

			shape.transform.localScale = Vector3.Lerp(shape.transform.localScale, defaultShapeScale, Time.deltaTime * 2.0f);
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		//if (collider.gameObject.tag == "Player" && gravityZone != null && gravityZone.desiredGravityNormal != -transform.up)
		if (collider.gameObject.tag == "Player" && gravityZone != null && gravityZone.gravityNormal != -transform.up)
		{
			gravityZone.SetGravity(transform.localRotation * Vector3.down); //gravityZone.SetGravity(-transform.up);

			if (shape)
				shape.transform.localScale = defaultShapeScale * 3.0f;
		}
	}
}
#endif