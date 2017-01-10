using UnityEngine;

// This is the player camera controller. It tracks mouse movement and forces the main camera to follow it.

public class PlayerCamera : MonoBehaviour
{
	// Maximum distance from eye position when in third person.
	public float maxThirdPersonCameraDistance = 5.0f;

	public GameObject gravityIndicator;
	public ParticleSystem dashScreenParticles;

	private const float maxAngle = 89.0f;

	private PlayerController playerController;

	// Store the current up/down look angle.
	private float camY = 0.0f;

	// View punches from hitting the ground and dashing.
	private float viewPunch = 0.0f;
	private bool viewPunching = false;

	// For the screen roll effect from walking.
	private float footRoll = 0.0f;
	private float footRollTarget = 0.0f;
	private float footRollEnd = 0.0f;

	private bool thirdPerson = false;
	private float thirdPersonCameraDistance = 0.0f;

	void Start()
	{
		//OnCameraAttached();
	}

	void OnCameraAttached()
	{
		playerController = GetComponentInParent<PlayerController>();
	}

	void Awake()
	{
		//Screen.lockCursor = true;
		//Cursor.visible = false;
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		else if (angle > 360)
			angle -= 360;

		return Mathf.Clamp(angle, min, max);
	}

	void Update()
	{
		if (!playerController)
			return;

		DoViewPunch();
		DoFootroll();

		if (Input.GetKeyDown(KeyCode.G))
			toggleThirdPerson();

		// If player died then play a short falling over animation.
		if (playerController.isDead && !playerController.diedFromFall)
		{
			float statestart = playerController.getStateStart();
			float stateend = playerController.getStateEnd();
			float statetime = stateend - statestart;

			footRoll = Mathf.Min(80.0f, (1.0f - Mathf.Pow((stateend - Time.time) / statetime, 2f)) * 180.0f);
		}
		else if (!Cursor.visible) // Don't track mouse movement if in a menu.
			camY = ClampAngle(camY - (Input.GetAxis("LookY") + Input.GetAxis("LookYJoystick") * 2.5f) * Game.Settings.lookSensitivity, -maxAngle, maxAngle);

		transform.localRotation = Quaternion.Euler(camY + viewPunch * 9.0f, 0.0f, footRoll);

		if (thirdPerson)
		{
			RaycastHit hitinfo;
			if (Physics.SphereCast(new Ray(playerController.eyePosition.position, -transform.forward), 0.2f, out hitinfo, maxThirdPersonCameraDistance, Mask.WORLD))
			{
				transform.position = hitinfo.point + hitinfo.normal * 0.2f + transform.forward * 0.2f;
				thirdPersonCameraDistance = Vector3.Distance(hitinfo.point, playerController.eyePosition.position);
			}
			else
			{
				transform.position = playerController.eyePosition.position - transform.forward * thirdPersonCameraDistance;
				thirdPersonCameraDistance = Mathf.Lerp(thirdPersonCameraDistance, maxThirdPersonCameraDistance, Time.deltaTime * 3.0f);
			}
		}

		// Adjust the on-screen gravity indicator.
		if (gravityIndicator)
		{
			Transform trans = gravityIndicator.transform;
			Vector3 pos = trans.localPosition;
			Vector3 scale = trans.localScale;

			pos.y = Mathf.Lerp(pos.y, playerController.gravity == Physics.gravity ? -4.0f : -1.55f, Time.deltaTime * 6.0f);
			scale.z = 0.3f - Mathf.Abs(Mathf.Sin(Time.time * 3.0f)) * 0.05f;
			scale.x = scale.y = 0.25f - scale.y * 0.25f;

			trans.localPosition = pos;
			trans.forward = playerController.gravityDir;
			trans.localScale = scale;

			gravityIndicator.GetComponent<Renderer>().material.color = playerController.onGround ? Color.green : Color.red;
		}
	}

	public void toggleThirdPerson()
	{
		thirdPerson = !thirdPerson;

		if (!thirdPerson)
		{
			transform.localPosition = Vector3.zero;
			thirdPersonCameraDistance = 0.0f;
		}
	}

	void DoViewPunch()
	{
		if (viewPunching)
		{
			viewPunch = Mathf.Lerp(viewPunch, 2.0f, Time.deltaTime * 5.0f);
			if (viewPunch >= 1.0f)
			{
				viewPunch = 1.0f;
				viewPunching = false;
			}
		}
		else if (viewPunch >= 0.0f)
			viewPunch = Mathf.Lerp(viewPunch, 0.0f, Time.deltaTime * 3.0f);
	}

	void DoFootroll()
	{
		footRoll = Mathf.Lerp(footRoll, Time.time < footRollEnd ? footRollTarget : 0.0f, Time.deltaTime * 5.0f);
	}

	public void Reset()
	{
		footRoll = footRollTarget = viewPunch = camY = 0.0f;
		viewPunching = false;
		transform.localRotation = Quaternion.identity;
	}

	public void SetPitch(float pitch)
	{
		camY = ClampAngle(pitch, -maxAngle, maxAngle);
	}

	// Called from the player controller.
	public void HitGround()
	{
		ViewPunch();
	}

	// Called from the player controller.
	public void DoFootstep(float magnitude, bool right)
	{
		footRollEnd = Time.time + 0.2f;
		footRollTarget = (right ? -1.0f : 1.0f) * magnitude * 2.0f;
	}

	public void ViewPunch()
	{
		viewPunching = true;
	}
}
