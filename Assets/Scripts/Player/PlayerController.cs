using UnityEngine;
using System.Collections.Generic;

// Has all the player physics, sounds, effects, etc.

public class PlayerController : MonoBehaviour
{
	class GravityAlteration
	{
		public LocalGravity controller;
		public float lerp;
		public bool isInside;

		public GravityAlteration(LocalGravity con)
		{
			controller = con;
			lerp = 0.0f;
			isInside = true;
		}
	}

	class GravityContact
	{
		public Collider collider;
		public Vector3 normal;
		public Vector3 point;
		public float validUntil;
	}

	public GameObject model;

	// Game settings
	[Header("Player Physics")]
	[Tooltip("Player won't ACCELERATE past this speed from player input while in the air.")]
	public float maxAirSpeed = 15.0f;
	[Tooltip("Player won't ACCELERATE past this speed from player input while on the ground.")]
	public float maxGroundSpeed = 15.0f;
	/*public float Acceleration = 75.0f;
	public float groundDampening = 0.05f;*/
	[Tooltip("Time to accelerate from 0 to full when on the ground.")]
	public float groundAccelTime = 0.5f;
	[Tooltip("Time to deacceleration from full to 0 when not pressing anything.")]
	public float groundDeaccelTime = 0.25f;
	public float airAcceleration = 10.0f;
	public float jumpPower = 13.0f;
	public float wallJumpPower = 15.0f;
	[Tooltip("Degrees from the current gravity directional angle.")]
	public float maxGroundNormalAngle = 55.0f;
	public float airDashTime = 0.25f;
	public float airDashForceSlope = 0.5f;
	public float airDashForceMultiplier = 3.0f;
	[Tooltip("How long player has before losing a combo.")]
	public float comboTime = 5.0f;
	[Tooltip("If double jumping in the opposite direction we're moving, be launched in the desired direction, multiplied by the maximum air speed, and multiplied by this number.")]
	public float doubleJumpDirectionChangeAllowance = 0.25f;
	[Tooltip("Downwards force to KEEP the player stuck to the ground.")]
	public float groundStickForce = 1.0f;
	public float terminalVelocity = 150.0f;

	// Phys materials
	[Header("Physics Materials")]
	public PhysicMaterial groundPhysicsMaterial;
	public PhysicMaterial airPhysicsMaterial;

	// Gravity
	private Vector3 defaultGravity = Physics.gravity;
	private Vector3 gravityLerp = Physics.gravity;
	private float defaultGravityMagnitude = Physics.gravity.magnitude;
	private List<GravityContact> gravityContacts = new List<GravityContact>();
	private List<GravityAlteration> gravityAlterations = new List<GravityAlteration>();
	private Vector3 m_gravity;
	[HideInInspector]
	public Vector3 gravity
	{
		set
		{
			m_gravity = value;
			gravityDir = value.normalized;
			//warp = Quaternion.FromToRotation(gravityDir, Vector3.down);
		}
		get { return m_gravity; }
	}
	[HideInInspector]
	public Vector3 gravityDir;
	/*[HideInInspector]
	public Quaternion warp = Quaternion.identity;*/
	public Quaternion warp { get { return Quaternion.FromToRotation(-transform.up, Vector3.down); } }

	// Individual audio clips
	[Header("Sound")]
	public AudioClip[] footLandSoftSound;
	public AudioClip[] footLandHardSound;
	public AudioClip[] footStepSoftSound;
	public AudioClip[] footStepHardSound;
	public AudioClip airDashSound;
	public AudioClip airDashSecondSound;
	public AudioClip[] jumpSound;
	public AudioClip doubleJumpSound;
	public AudioClip wallJumpSound;
	public AudioClip[] deathSound;
	public AudioClip[] deathSoundImpaled;
	public AudioClip[] deathSoundFall;
	public AudioClip respawnSound;

	// Basic physics
	private bool canUseMidairAbility = true;
	private float nextJump;
	private float nextWallJump;
	private float nextAirDash;
	//private float nextAirDashFromGround;
	private float noGroundTime;

	// Footsteps
	[Header("Footsteps")]
	public float footStepMinTime = 0.35f;
	public float footStepMaxTime = 3.0f;
	public float footStepMinSpeed = 3.0f;
	private float lastFootStep;
	private bool swapfoot;
	private float nextGroundHitEffect;

	// Store the game HUD.
	private GameHUD gameHUD;

	// States
	[HideInInspector]
	public enum PlayerState { Idle, Dead, DeadImpaled, AirDash }
	private PlayerState state = PlayerState.Idle;
	private float stateStart;
	private float stateEnd;
	public float getStateStart() { return stateStart; }
	public float getStateEnd() { return stateEnd; }
	public PlayerState getState() { return state; }

	// Death state
	public bool isDead { get { return state == PlayerState.Dead || state == PlayerState.DeadImpaled; } }
	public bool diedFromFall { get { return m_diedFromFall; } }
	private bool m_diedFromFall;

	// Air dash
	private Vector3 airDashDirection;

	// Ground state
	[Header("Effects")]
	public ParticleSystem hitGroundHardRightParticles;
	public ParticleSystem hitGroundHardLeftParticles;
	public ParticleSystem hitGroundSoftRightParticles;
	public ParticleSystem hitGroundSoftLeftParticles;
	public ParticleSystem landGroundHardParticles;
	public ParticleSystem landGroundSoftParticles;
	private float lastGroundTime;
	public bool onGround
	{
		get { return groundObject != null; }
		set
		{
			if (!value)
			{
				groundObject = null;
				nextGroundHitEffect = Time.time + 0.5f;
			}
		}
	}
	private GameObject groundObject;
	private bool isOnSoftGround;
	private Vector3 groundNormal = Vector3.up;

	// Level stuff
	[HideInInspector]
	public float lastTakeCollectable;
	[HideInInspector]
	public float levelStartTime;
	private int droppedCombo;
	private int currentCombo;

	// Camera
	private PlayerCamera playerCamera;
	[Header("Camera")]
	public Transform eyePosition;

	// Input
	private bool pressedJump;
	private bool pressedDash;

	// Frozen state
	private bool freeze { set { GetComponent<Rigidbody>().drag = value ? Mathf.Infinity : 0.0f; } get { return GetComponent<Rigidbody>().drag == Mathf.Infinity; } }

	// developer
	private bool cheatInfiniteDoubleJump;
	private bool showDebugInfo;

	void Start()
	{
		gravity = Physics.gravity;

		GameObject obj = GameObject.FindGameObjectWithTag("GameUI");
		if (obj)
			gameHUD = obj.GetComponent<GameHUD>();
	}

	void Awake()
	{
		//GetComponent<Rigidbody>().useConeFriction = true;

		// Respawn the player after we do some house keeping.

		//Screen.lockCursor = true;

		GetComponent<Collider>().material = airPhysicsMaterial;

		DoRespawn();

		AttachMainCamera();
	}

	void OnCameraAttached()
	{
		playerCamera = gameObject.GetComponentInChildren<PlayerCamera>();
	}

	public void AttachMainCamera()
	{
		if (Camera.main)
		{
			Camera.main.transform.parent = eyePosition;
			Camera.main.transform.localPosition = Vector3.zero;
			Camera.main.transform.localRotation = Quaternion.identity;

			Camera.main.SendMessage("OnCameraAttached");
			OnCameraAttached();
		}
	}

	void Update()
	{
		if (!Cursor.visible && (!isDead || diedFromFall))
		{
			float camx = (Input.GetAxis("LookX") + Input.GetAxis("LookXJoystick") * 2.5f) * Game.Settings.lookSensitivity;
			if (camx != 0)
				transform.Rotate(Vector3.up, camx, Space.Self);
		}

		DoCheats();

		// Check if dropping the combo.
		if (currentCombo > 0 && Time.time >= lastTakeCollectable + comboTime)
		{
			droppedCombo++;
			currentCombo = 0;
		}

		if (Input.GetButtonDown("Restart"))
			DoRespawn();
		else if (Input.GetButtonDown("Suicide"))
			DoPlayerDeath(DeathType.Suicide);

		// Don't do very much if we're dead...
		if (isDead)
			return;

		// Need to store these and feed them to FixedUpdate later on because only Update can get these true values.
		if (Input.GetButtonDown("Dash"))
			pressedDash = true;
		if (Input.GetButtonDown("Jump"))
			pressedJump = true;

		// developer
		if (Input.GetKeyDown(KeyCode.F1))
			showDebugInfo = !showDebugInfo;
		if (Input.GetKeyDown(KeyCode.F2))
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Input.GetKeyDown(KeyCode.F3))
			GetComponent<Rigidbody>().velocity += transform.forward * 10;
	}

	// Cheat codes for debugging. To be disabled later.
	private void DoCheats()
	{
		if (Input.GetKey(KeyCode.LeftControl))
		{
			if (Input.GetKeyDown(KeyCode.L))
			{
				Camera cam = Camera.main.GetComponent<Camera>();
				cam.farClipPlane = cam.farClipPlane == 100f ? 1000f : 100f;
			}
			if (Input.GetKeyDown(KeyCode.T))
			{
				Time.timeScale = Time.timeScale == 1.0f ? 0.25f : 1.0f;

				GetComponent<AudioSource>().PlayOneShot(respawnSound, 0.5f);
				print("CHEAT: Time scale set to " + Time.timeScale);
			}
			if (Input.GetKeyDown(KeyCode.J))
			{
				cheatInfiniteDoubleJump = !cheatInfiniteDoubleJump;

				GetComponent<AudioSource>().PlayOneShot(respawnSound, 0.5f);
				print("CHEAT: Infinite double jumps " + (cheatInfiniteDoubleJump ? "ON" : "OFF"));
			}
		}
	}

	private void DoChangeGravity()
	{
		gravityContacts.RemoveAll(x => Time.time > x.validUntil);

		// First calculate our "default" gravity, without taking in to account fields.
		// We use the gravityContacts from our recent collisions for this.
		Vector3 dgrav = Vector3.zero;
		if (gravityContacts.Count == 0)
			dgrav = Vector3.down;
		else
			foreach (GravityContact gc in gravityContacts)
				dgrav -= gc.normal; //dgrav += gc.normal * Mathf.Clamp01(1.0f - Vector3.Distance(gc.point, collider.ClosestPointOnBounds(gc.point)));
		/*defaultGravity = Vector3.Lerp(defaultGravity.normalized, dgrav.normalized, Time.deltaTime * 5.0f);
		defaultGravity = defaultGravity.normalized * defaultGravityMagnitude;*/
		defaultGravity = dgrav.normalized * defaultGravityMagnitude;

		float totallerp = 0.0f;
		int total = 0;
		Vector3 grav = Vector3.zero;
		float gravmag = 0.0f;

		//float rate = Time.deltaTime * 3.0f;

		foreach (GravityAlteration alteration in gravityAlterations)
		{
			/*alteration.lerp = Mathf.Lerp(alteration.lerp, alteration.isInside ? 1.05f : -0.05f, rate);
			alteration.lerp = Mathf.Clamp01(alteration.lerp);

			if (alteration.lerp > 0.0f)
			{
				Vector3 g = alteration.controller.getLocalGravity(this);

				grav += g * alteration.lerp;
				gravmag += g.magnitude;

				totallerp += alteration.lerp;
				total++;
			}*/

			Vector3 g = alteration.controller.getLocalGravity(this);

			grav += g;
			gravmag += g.magnitude;

			totallerp += 1;
			total++;
		}

		// No alterations applied, just use default gravity.
		if (total == 0)
			gravity = defaultGravity;
		else
		{
			// If total weight < 1 then apply the default gravity for the missing section.
			/*if (totallerp < 1.0f)
			{
				grav += defaultGravity * (1.0f - totallerp);
				totallerp = 1.0f;
			}

			gravity = (grav / total).normalized * (gravmag / total);*/

			gravity = (grav / totallerp).normalized * (gravmag / total);

			//gravityAlterations.RemoveAll(x => x.lerp <= 0.0f);
		}

		// Face player towards new gravity normal but follow our current forward vector.
		/*Vector3 forwardVector = transform.forward - gravityDir * Vector3.Dot(transform.forward, gravityDir);
		if (forwardVector != Vector3.zero)
			transform.localRotation = Quaternion.LookRotation(forwardVector, -gravityDir);*/

		if (Vector3.Dot(gravityLerp, gravityDir) < 0.9f)
			gravityLerp = Vector3.Slerp(gravityLerp, gravityDir, Time.deltaTime * 6.0f).normalized;
		else
			gravityLerp = Vector3.Lerp(gravityLerp, gravityDir, Time.deltaTime * 6.0f).normalized;
		if (gravityLerp == Vector3.zero)
			gravityLerp = gravityDir;

		/*Vector3 forwardVector = transform.forward - gravityDir * Vector3.Dot(transform.forward, gravityDir);
		if (forwardVector != Vector3.zero)
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(forwardVector, -gravityDir), Time.deltaTime * 5.0f);
			//transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.LookRotation(forwardVector, -gravityDir), Time.deltaTime * 360.0f);*/

		Vector3 forwardVector = transform.forward - gravityLerp * Vector3.Dot(transform.forward, gravityLerp);
		if (forwardVector != Vector3.zero)
			transform.localRotation = Quaternion.LookRotation(forwardVector, -gravityLerp);
	}

	// The bread and butter. Physics work in a state system. The player can have one state at a time as well as a start/end time for that state.
	void FixedUpdate()
	{
		// Frozen then don't accept any input.
		if (freeze)
			return;

		// Detect nearby surfaces for altering default gravity (walking on walls).
		//DoSurfaceDetection();

		// Check if we're on the ground or not, if we're on soft/hard ground, update variables accordingly.
		DoGroundCheck();

		// Gravity altering stuff.
		DoChangeGravity();

		// Check if state should end.
		DoStateThink();

		// Apply (fake) gravity.
		if (!onGround)
			GetComponent<Rigidbody>().velocity += gravity * Time.deltaTime;

		switch (state)
		{
			case PlayerState.Idle: // Not doing anything special.
				DoMovement();

				//if (pressedDash && canUseMidairAbility && Time.time >= nextAirDash && (!onGround || Time.time >= nextAirDashFromGround))
				if (pressedDash && canUseMidairAbility && Time.time >= nextAirDash)
					SetState(PlayerState.AirDash);

				if (pressedJump)
					DoJump();

				if (onGround)
					DoFootsteps();

				break;
			case PlayerState.AirDash:
				DoAirDash();

				break;
			case PlayerState.DeadImpaled: // If we're dead from impaling then freeze in mid-air (like we got stuck on the spikes).
				GetComponent<Rigidbody>().velocity = Vector3.zero;

				break;
		}

		// Terminal velocity.
		Vector3 vel = GetComponent<Rigidbody>().velocity;
		if (vel.magnitude >= terminalVelocity)
			GetComponent<Rigidbody>().velocity = vel.normalized * terminalVelocity;

		// Reset the key states from Update.
		pressedJump = false;
		pressedDash = false;
	}

	public int getCombo()
	{
		return currentCombo;
	}

	// How far off are we from dropping our combo from 0.0 to 1.0?
	public float getComboDropPercent()
	{
		if (currentCombo == 0)
			return 0;

		return ((lastTakeCollectable + comboTime) - Time.time) / comboTime;
	}

	void OnEnterLocalGravityZone(LocalGravity controller)
	{
		GravityAlteration alteration = gravityAlterations.Find(x => x.controller == controller);
		if (alteration == null)
			gravityAlterations.Add(new GravityAlteration(controller));
		else
			alteration.isInside = true;
	}

	void OnEnterLocalGravityZone()
	{
	}

	void OnExitLocalGravityZone(LocalGravity controller)
	{
		/*GravityAlteration alteration = gravityAlterations.Find(x => x.controller == controller);
		if (alteration != null)
			alteration.isInside = false;*/
		gravityAlterations.RemoveAll(x => x.controller == controller);
	}

	private void DoFootsteps()
	{
		// Play footstep effects based on how fast we're moving in our ground normalized plane.
		float speed = GetComponent<Rigidbody>().velocity.magnitude;
		if (speed < footStepMinSpeed)
			return;

		// Next time we play is based on our speed.
		float speedPerMaxSpeed = Mathf.Min(1.0f, speed / maxGroundSpeed);
		float nextFootStep = lastFootStep + Mathf.Max(footStepMinTime, footStepMaxTime * (1 - speedPerMaxSpeed));

		if (Time.time >= nextFootStep)
		{
			lastFootStep = Time.time;
			swapfoot = !swapfoot;

			AudioClip[] audioclips = isOnSoftGround ? footStepSoftSound : footStepHardSound; // Hard or soft material sounds?
			AudioClip clip = UtilRandom.RandomFromArray(audioclips) as AudioClip;
			
			// Play softer/louder depending on current speed.
			float magnitude = 0.5f + speedPerMaxSpeed * 0.5f;

			if (clip != null)
				GetComponent<AudioSource>().PlayOneShot(clip, magnitude);

			ParticleSystem system = isOnSoftGround ? swapfoot ? hitGroundSoftRightParticles : hitGroundSoftLeftParticles : swapfoot ? hitGroundHardRightParticles : hitGroundHardLeftParticles;
			system.Play();

			// Send the camera info about the footstep for rolling effect.
			if (playerCamera)
				playerCamera.DoFootstep(magnitude, swapfoot);
		}
	}

	private void SetState(PlayerState newstate)
	{
		if (state == newstate)
			return;

		PlayerState oldstate = state;
		state = newstate;

		StateEnded(oldstate);
		StateStarted(newstate);

		stateStart = Time.time;
	}

	private void DoStateThink()
	{
		// If we're scheduled for a state end, then end it. Nothing else to do here.
		if (stateEnd > 0 && Time.time >= stateEnd)
			SetState(PlayerState.Idle);
	}

	private void StateEnded(PlayerState oldstate)
	{
		if (oldstate == PlayerState.AirDash)
		{
			Vector3 vel = GetComponent<Rigidbody>().velocity;
			float maxmagnitude = onGround ? maxGroundSpeed : maxAirSpeed;
			if (vel.magnitude >= maxmagnitude)
				GetComponent<Rigidbody>().velocity = vel.normalized * maxmagnitude;
		}
		else if (oldstate == PlayerState.Dead || oldstate == PlayerState.DeadImpaled) // If we just finished a death state then respawn.
			DoRespawn();
	}

	private void DoRespawn()
	{
		SetState(PlayerState.Idle);

		if (gameHUD)
			gameHUD.DoRespawn();

		gravityAlterations.Clear();
		gravityContacts.Clear();
		gravity = Physics.gravity;
		gravityLerp = gravityDir;

		Time.timeScale = 1.0f;
		
		GetComponent<Rigidbody>().velocity = Vector3.zero;

		// We have a checkpoint?
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Checkpoint"))
		{
			CheckpointTrigger checkpoint = obj.GetComponent<CheckpointTrigger>();
			if (checkpoint && checkpoint.activated)
			{
				// Restore every collectable active state to the states stored in the checkpoint.
				foreach (GameObject collectable in Collectable.AllCollectables)
					if (collectable)
						collectable.SetActive(true);
				foreach (GameObject collectable in checkpoint.collectables)
					if (collectable)
						collectable.SetActive(false);

				// Go to the checkpoint position and rotation.
				transform.position = obj.transform.position;
				transform.rotation = obj.transform.rotation;

				if (playerCamera)
					playerCamera.Reset();

				if (respawnSound)
					GetComponent<AudioSource>().PlayOneShot(respawnSound);

				return;
			}
		}

		// Otherwise just start the entire level over again, including the timer.
		GameObject respawnobj = GameObject.FindGameObjectWithTag("Respawn");
		if (respawnobj)
		{
			transform.position = respawnobj.transform.position;
			transform.rotation = respawnobj.transform.rotation;
		}
		else
		{
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
		}

		levelStartTime = 0.0f;

		// Restore all the collectables.
		if (Collectable.AllCollectables != null)
			foreach (GameObject collectable in Collectable.AllCollectables)
				if (collectable)
					collectable.SetActive(true);
	}

	void DoStartLevel()
	{
		if (levelStartTime <= 0.0f)
			levelStartTime = Time.time;
	}

	// Called after SetState. StateEnded is called before this.
	private void StateStarted(PlayerState newstate)
	{
		if (newstate == PlayerState.AirDash)
		{
			stateEnd = Time.time + airDashTime;

			if (playerCamera)
				airDashDirection = playerCamera.transform.forward;
			else
				airDashDirection = transform.forward;

			if (Input.GetAxis("Vertical") < 0)
			{
				airDashDirection = -airDashDirection;

				if (playerCamera)
				{
					playerCamera.dashScreenParticles.transform.localRotation = Quaternion.LookRotation(Vector3.forward);
					playerCamera.dashScreenParticles.Play();
				}
			}
			else if (playerCamera)
			{
				playerCamera.dashScreenParticles.transform.localRotation = Quaternion.LookRotation(Vector3.back);
				playerCamera.dashScreenParticles.Play();
			}

			// Don't let player air dash straight up.
			float min = 0.25f; //onGround ? 0.25f : 0.0f;
			airDashDirection = warp * airDashDirection;
				for (int i = 0; i < 5; i++)
				{
					airDashDirection.Normalize();
					airDashDirection.y = Mathf.Clamp(airDashDirection.y, min, 0.5f);
				}
			airDashDirection = Quaternion.Inverse(warp) * airDashDirection;

			if (onGround)
			{
				onGround = false;
				noGroundTime = Time.time + 0.1f;

				//nextAirDashFromGround = Time.time + 0.55f;
				//nextAirDash = Time.time + 0.15f;
			}
			else
			{
				canUseMidairAbility = false;

				//nextAirDash = Time.time + 0.25f;
			}

			nextAirDash = Time.time + 0.45f;

			if (playerCamera)
				playerCamera.ViewPunch();

			if (airDashSound != null)
				GetComponent<AudioSource>().PlayOneShot(airDashSound);
		}
		else if (newstate == PlayerState.Dead || newstate == PlayerState.DeadImpaled)
		{
			Time.timeScale = 0.5f; // Slow time for DRAMATIC EFFECT.
			stateEnd = Time.time + 1.0f; // State actually ends in 2 seconds because of above.
			lastTakeCollectable = -50.0f; // Kill the current combo.
		}
	}

	void OnGUI()
	{
		if (showDebugInfo)
		{
			UtilGUI.DrawRectangle(new Rect(0, 0, 248, 148), Color.black, 0.2f);
			GUI.Label(new Rect(4, 4, 240, 20), "grav norm: " + gravityDir);
			GUI.Label(new Rect(4, 24, 240, 20), "grav force: " + gravity.magnitude);
			GUI.Label(new Rect(4, 44, 240, 20), "grav alts: " + gravityAlterations.Count);
			GUI.Label(new Rect(4, 64, 240, 20), "ground norm: " + groundNormal);
			GUI.Label(new Rect(4, 84, 240, 20), "ground local norm: " + warp * groundNormal);
			GUI.Label(new Rect(4, 108, 240, 20), "can air ability: " + canUseMidairAbility);
			if (onGround)
				GUI.Label(new Rect(4, 128, 240, 20), "grnd spd/max: " + Mathf.Round(GetComponent<Rigidbody>().velocity.magnitude / maxGroundSpeed * 10000.0f) / 10000.0f);
			else
			{
				Vector3 vel = GetComponent<Rigidbody>().velocity;

				vel = warp * vel;
					vel.y = 0;
				vel = Quaternion.Inverse(warp) * vel;

				GUI.Label(new Rect(4, 128, 240, 20), "air spd/max: " + Mathf.Round(vel.magnitude / maxAirSpeed * 10000.0f) / 10000.0f);
			}
		}
	}

	private void DoAirDash()
	{
		if (onGround)
		{
			SetState(PlayerState.Idle);
			return;
		}

		// We have much more force at the start of the dash than at the end.
		float force = Mathf.Pow((stateEnd - Time.time) / airDashTime, airDashForceSlope) * airDashForceMultiplier;

		// airDashDirection was assigned when the state started. So we're dead set on going this way, even if we change the camera direction.
		GetComponent<Rigidbody>().velocity = maxAirSpeed * force * airDashDirection;
	}

	private void DoGroundCheck()
	{
		// Only on ground if the normal of the ground is facing upwards.
		// Don't set to the ground if we just detached from the ground via jump.
		RaycastHit hitinfo;
		if (Time.time >= noGroundTime
			&& Physics.SphereCast(transform.position, 0.5f, -transform.up, out hitinfo, 0.55f, Mask.WORLD) // Center logic
			//&& Physics.SphereCast(transform.position + transform.up, 0.05f, -transform.up, out hitinfo, 1.1f, Mask.WORLD) // Feet = 0 logic
			&& Vector3.Angle(hitinfo.normal, transform.up) <= maxGroundNormalAngle)
		{
			groundObject = hitinfo.collider.gameObject;
			isOnSoftGround = hitinfo.collider.tag == "WorldSoft";
			groundNormal = hitinfo.normal;
			lastGroundTime = Time.time;
		}
		else if (onGround)
			onGround = false;

		// Change our physics material (friction) based on ground state and keys being pressed.
		PhysicMaterial physmat = groundPhysicsMaterial;
		if (onGround)
			canUseMidairAbility = true; // Resets being able to use once-per-ground-touch abilities.
		else
			physmat = airPhysicsMaterial;

		if (GetComponent<Collider>().material != physmat)
			GetComponent<Collider>().material = physmat;
	}

	void DoJump()
	{
		if (Time.time < nextJump)
			return;

		// Allow regular jumping if we're on the ground or we WERE on the ground 0.15 seconds earlier.
		// If we don't then players complained that they seemed to activate the double jump instead of regular jump when near edges.
		if (onGround || Time.time <= lastGroundTime + 0.15f)
		{
			nextJump = noGroundTime = Time.time + 0.1f;
			nextAirDash = nextJump + 0.1f;

			onGround = false;

			Vector3 vel = GetComponent<Rigidbody>().velocity;

			vel = warp * vel;

				vel.y = Mathf.Max(0.0f, vel.y) + jumpPower;

			vel = Quaternion.Inverse(warp) * vel;

			GetComponent<Rigidbody>().velocity = vel;

			if (jumpSound.Length > 0)
				GetComponent<AudioSource>().PlayOneShot(UtilRandom.RandomFromArray(jumpSound) as AudioClip);

			return;
		}

		// See if we should wall jump.
		if (Time.time >= nextWallJump)
		{
			Vector3 pos = eyePosition.position;
			Vector3 forward = transform.forward;
			Vector3 down = -transform.up;

			// Twice, once for each direction (back and forwards).
			for (int i = 0; i < 2; i++)
			{
				// First try at the camera.
				if (TryWallJump(pos, forward)) return;

				// Try wall jumping multiple times at different local y levels.
				for (int j = 1; j <= 4; j++)
					if (TryWallJump(pos + j * 0.2f * down, forward)) return;

				// Try other direction.
				pos = eyePosition.position;
				forward = -forward;
			}
		}

		// OK, see if we can double jump?
		if (canUseMidairAbility || cheatInfiniteDoubleJump)
		{
			canUseMidairAbility = false;

			float fwdspeed = Input.GetAxis("Vertical");

			// If not pressing anything just set the y velocity to jump power.
			if (fwdspeed == 0)
			{
				Vector3 vel = GetComponent<Rigidbody>().velocity;

				vel = warp * vel;
					vel.y = jumpPower;
				vel = Quaternion.Inverse(warp) * vel;

				GetComponent<Rigidbody>().velocity = vel;
			}
			else
			{
				// If we're holding a direction then it's a bit trickier.
				// We want to give some degree of enhanced control when double jumping without the ability to instantly change directions.
				// So we scale by the dot product between the current movement direction and the desired movement direction.
				// But the scale can never be below 0.5 otherwise we would go no where.
				Quaternion dewarp = Quaternion.Inverse(warp);

				// Always move relative to the camera.
				Vector3 forward = transform.forward;

				// Find out the current movement direction
				Vector3 current_velocity = GetComponent<Rigidbody>().velocity;

				current_velocity = warp * current_velocity;

					// Ignore the warped Y axis.
					current_velocity.y = 0.0f;

				current_velocity = dewarp * current_velocity;

				Vector3 current_dir = current_velocity.normalized;
				float current_speed = current_velocity.magnitude;

				Vector3 new_velocity = forward * fwdspeed;
				new_velocity *= Mathf.Max(doubleJumpDirectionChangeAllowance, Vector3.Dot(current_dir, new_velocity));
				new_velocity *= current_speed;

				new_velocity = warp * new_velocity;
				
					new_velocity.y = jumpPower;

				new_velocity = dewarp * new_velocity;

				GetComponent<Rigidbody>().velocity = new_velocity;
			}
						
			if (doubleJumpSound)
				GetComponent<AudioSource>().PlayOneShot(doubleJumpSound);
		}
	}

	private bool TryWallJump(Vector3 pos, Vector3 normal)
	{
		RaycastHit hitinfo;
		if (Physics.Raycast(pos, normal, out hitinfo, 0.7f, Mask.WORLD))
		{
			Vector3 hitnormal = hitinfo.normal;
			Vector3 warpedhitnormal = warp * hitnormal;

			if (Mathf.Abs(warpedhitnormal.y) <= 0.25f) // Hit something and hit it at a normal close to the local horizon.
			{
				// The desired direction is the wall hit normal rotated 45 degrees towards the local up vector.
				// So if we hit a slanted wall we get an extra (or less) angle.
				Vector3 dir = Vector3.RotateTowards(hitnormal, transform.up, Mathf.PI / 4, 1.0f);

				GetComponent<Rigidbody>().velocity = dir * wallJumpPower;

				nextWallJump = Time.time + 0.1f;

				if (wallJumpSound != null)
					GetComponent<AudioSource>().PlayOneShot(wallJumpSound);

				//Debug.DrawLine(hitinfo.point, hitinfo.point + dir * 4.0f, Color.green, 3.0f, false);

				return true;
			}
		}

		return false;
	}

	/*private const float surfaceDetectionDistance = 1.5f;
	private const float surfaceRadiusRequired = 0.5f;
	private const int surfaceDetectionInaccuracy = 15;*/
	private bool isValidSurface(Collider objcollider, Vector3 contact_point, Vector3 contact_normal)
	{
		/*if (contact_normal == Vector3.up)
			return true;*/

		/*if (Vector3.Angle(contact_normal, Vector3.up) <= maxGroundNormalAngle)
		{
			//Debug.DrawLine(contact_point, contact_point + contact_normal * 4, Color.red);
			return false;
		}*/

		/*int maxinvalids = 90 / surfaceDetectionInaccuracy;

		RaycastHit hitinfo;

		Vector3 pos = transform.position;

		// Check if we have enough surface area.
		Vector3 checkplane = contact_point - contact_normal * 0.05f;

		bool valid = true;
		int invalids = 0;

		Quaternion rot = Quaternion.LookRotation(contact_normal);
		Vector3 ang = rot.eulerAngles;
		for (int i = 0; i < 360; i += surfaceDetectionInaccuracy)
		{
			ang.z += surfaceDetectionInaccuracy;

			Vector3 up = Quaternion.Euler(ang) * Vector3.up;
			Vector3 checkpos = checkplane + up * surfaceRadiusRequired;

			if (!Physics.Linecast(pos, checkpos, out hitinfo, Mask.WORLD) || hitinfo.collider != objcollider || hitinfo.normal != contact_normal)
			{
				Debug.DrawLine(pos, checkpos, new Color(1f, 0f, 0f, 0.5f));

				if (++invalids > maxinvalids)
					return false;
			}
			else
				Debug.DrawLine(pos, checkpos, new Color(0f, 1f, 0f, 0.5f));
		}*/

		return true;
	}

	/*private float surfaceDetectionDistance = 1.5f;
	private float surfaceRadiusRequired = 0.5f;
	private const int surfaceDetectionInaccuracy = 15;
	private void DoSurfaceDetection()
	{
		//if (transform.up == Vector3.up && onGround)
		//	return;

		// Allow 1/4 of traces to miss.
		int maxinvalids = 90 / surfaceDetectionInaccuracy;

		float radius = (collider as CapsuleCollider).radius;
		Vector3 pos = transform.position + transform.up * radius; // Where the cylinder meets the bottom circle of the collider.

		RaycastHit hitinfo;

		foreach (Collider objcollider in Physics.OverlapSphere(pos, surfaceDetectionDistance, Mask.WORLD))
		{
			Vector3 closest = objcollider.ClosestPointOnBounds(pos);
			if (closest == pos)
				closest = objcollider.collider.bounds.center;

			//closest += transform.forward * 0.01f; // Fudge the position a little so we don't hit corners.
			
			// Might be convex/rotated bounding box.
			if (!Physics.Raycast(pos, (closest - pos).normalized, out hitinfo, surfaceDetectionDistance, Mask.WORLD))
			{
				//Debug.DrawLine(pos, closest, Color.red);
				continue;
			}
			//else
				//Debug.DrawLine(pos, closest, Color.green);

			// Check if we have enough surface area.
			Vector3 hitpos = hitinfo.point;
			Vector3 surfacenormal = hitinfo.normal;
			Vector3 checkplane = hitpos - surfacenormal * 0.05f;

			bool valid = true;
			int invalids = 0;

			Quaternion rot = Quaternion.LookRotation(surfacenormal);
			Vector3 ang = rot.eulerAngles;
			for (int i = 0; i < 360; i += surfaceDetectionInaccuracy)
			{
				ang.z += surfaceDetectionInaccuracy;

				Vector3 up = Quaternion.Euler(ang) * Vector3.up;
				Vector3 checkpos = checkplane + up * surfaceRadiusRequired;

				if (!Physics.Linecast(pos, checkpos, out hitinfo, Mask.WORLD) || hitinfo.collider != objcollider || hitinfo.normal != surfacenormal)
				{
					Debug.DrawLine(pos, checkpos, new Color(1f, 0f, 0f, 0.5f));

					if (++invalids > maxinvalids)
					{
						valid = false;
						break;
					}
				}
				else
					Debug.DrawLine(pos, checkpos, new Color(0f, 1f, 0f, 0.5f));
			}

			Debug.DrawLine(pos, closest, valid ? Color.cyan : Color.yellow);
		}
	}

	private void DetectSurface(Vector3 pos, Vector3 dir)
	{
		Debug.DrawLine(pos, pos + dir, Color.red);
	}*/

	// This is the basic ground movement when not in any special state.
	void DoMovement()
	{
		// Always move relative to the camera.
		Vector3 forward = transform.forward;
		Vector3 right = transform.right;

		// Get our desired direction.
		Vector3 heading = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
		heading.ClampMagnitude01();

		if (onGround)
		{
			float speed = maxGroundSpeed;

			// Going up or down a hill we need to make the movement direction follow the hill.
			if (heading == Vector3.zero)
				speed /= groundDeaccelTime;
			else
			{
				speed /= groundAccelTime;

				if (groundNormal != transform.up)
				{
					heading = Vector3.Cross(groundNormal, Vector3.Cross(heading, groundNormal));
					heading.Normalize();
				}

				heading *= maxGroundSpeed;
			}

			GetComponent<Rigidbody>().velocity = Vector3.MoveTowards(GetComponent<Rigidbody>().velocity, heading, speed * Time.deltaTime) - groundNormal * groundStickForce;
		}
		else if (heading != Vector3.zero)
		{
			// Current velocity not including falling / rising.
			Vector3 vel = warp * GetComponent<Rigidbody>().velocity;
				float yspeed = vel.y;
				vel.y = 0;
			vel = Quaternion.Inverse(warp) * vel;

			heading *= maxAirSpeed;

			// Allow the player to maintain speed above their maximum air speed, but not accelerate past it.
			if (vel.magnitude > maxAirSpeed)
			{
				float dot = Vector3.Dot(heading.normalized, vel.normalized);
				if (dot < 1)
					vel = Vector3.MoveTowards(vel, heading, (1.0f - Mathf.Max(0.0f, dot)) * airAcceleration * Time.deltaTime);
			}
			else
				vel = Vector3.MoveTowards(vel, heading, airAcceleration * Time.deltaTime);

			vel += transform.up * yspeed; // Restore the y velocity.

			GetComponent<Rigidbody>().velocity = vel;
		}
	}

	void HitGround(Collision collision)
	{
		// Don't play this effect too often or while dead.
		if (Time.time < nextGroundHitEffect || isDead)
			return;

		nextGroundHitEffect = Time.time + 0.25f;

		// Only play this effect if we hit the ground hard enough on the local Y axis.
		if ((warp * collision.relativeVelocity).y >= 5.0f)
		{
			// Play a different impact sound depending on hard / soft ground.
			AudioClip[] audioclips = isOnSoftGround ? footLandSoftSound : footLandHardSound;
			if (audioclips.Length > 0)
				GetComponent<AudioSource>().PlayOneShot(UtilRandom.RandomFromArray(audioclips) as AudioClip);

			ParticleSystem system = isOnSoftGround ? landGroundSoftParticles : landGroundHardParticles;
			system.Play();

			// Send camera message we hit the ground so it can play a view punch effect.
			if (playerCamera)
				playerCamera.HitGround();
		}
	}

	void DoPlayerDeath(DeathType deathtype)
	{
		if (isDead)
			return;

		if (deathtype == DeathType.Pierce)
			SetState(PlayerState.DeadImpaled);
		else
			SetState(PlayerState.Dead);

		// Send HUD info so it can dim and red the screen.
		if (gameHUD)
			gameHUD.DoPlayerDeath(deathtype);

		// Used for allowing the camera to free rotate if we're falling forever.
		m_diedFromFall = deathtype == DeathType.Fall;

		/*if (deathtype != DeathType.Fall)
			resetGravity(true);*/

		// All sorts of different sounds depending on how we died!
		AudioClip[] sounds;
		if (deathtype == DeathType.Pierce)
			sounds = deathSoundImpaled;
		else if (deathtype == DeathType.Fall)
			sounds = deathSoundFall;
		else
			sounds = deathSound;

		if (sounds.Length > 0)
			GetComponent<AudioSource>().PlayOneShot(UtilRandom.RandomFromArray(sounds) as AudioClip, 1.0f);
	}

	void OnTouchCollectable(GameObject collectable)
	{
		// If we're in the air and touch a bolt, we're allowed to use 1 more special move.
		if (!onGround)
			canUseMidairAbility = true;

		// Increment the combo and reset the combo loss timer.
		currentCombo++;
		lastTakeCollectable = Time.time;

		// Let the HUD know.
		if (gameHUD)
			gameHUD.OnTouchCollectable(collectable);
	}

	void OnTouchGoal(GameObject goal)
	{
		if (freeze)
			return;

		// Level completed so just freeze the player.
		freeze = true;

		float level_completion_time = Time.time - levelStartTime;
		int total_collectables = Collectable.AllCollectables.Length;
		int collected_collectables = 0;
		foreach (GameObject obj in Collectable.AllCollectables)
			if (obj && !obj.activeSelf)
				collected_collectables++;
		
		// Would be used for high scores.
		/*Level currentlevel = LevelController.currentLevel;
		if (currentlevel != null)
		{
			currentlevel.BestCollectables = Mathf.Max(collected_collectables, currentlevel.BestCollectables);
			currentlevel.BestTime = Mathf.Min(level_completion_time, currentlevel.BestTime);
			currentlevel.BestComboLost = Mathf.Min(droppedCombo, currentlevel.BestComboLost);
		}*/

		if (gameHUD != null)
			gameHUD.LevelFinished(level_completion_time, collected_collectables, total_collectables, droppedCombo);
	}

	void OnCollisionEnter(Collision collision)
	{
		// If we touch an object tagged as world with our feet then we set it as our current ground object.
		foreach (ContactPoint contact in collision.contacts)
		{
			if (Vector3.Angle(contact.normal, -gravityDir) <= maxGroundNormalAngle && (contact.otherCollider.gameObject.layer == Layers.WORLD || contact.otherCollider.gameObject.layer == Layers.WORLD2))
			{
				groundObject = contact.otherCollider.gameObject;
				HitGround(collision);
				isOnSoftGround = contact.otherCollider.tag == "WorldSoft";
				break;
			}
		}
	}
	
	void OnCollisionExit(Collision collision)
	{
		// If we stop touching our ground object then immediately clear our ground object.
		foreach (ContactPoint contact in collision.contacts)
		{
			if (contact.otherCollider.gameObject.layer == Layers.WORLD || contact.otherCollider.gameObject.layer == Layers.WORLD2)
			{
				if (contact.otherCollider.gameObject == groundObject)
				{
					onGround = false;
					break;
				}
			}
		}
	}

	void OnCollisionStay(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			//if (contact.otherCollider.gameObject.layer == Layers.WORLD || contact.otherCollider.gameObject.layer == Layers.WORLD2)
			if (contact.otherCollider.gameObject.layer == Layers.WORLD2)
			{
				//Debug.DrawLine(contact.point, contact.point + contact.normal * 4.0f, Color.green);

				Vector3 normal = contact.normal;

				GravityContact gc = null;
				foreach (GravityContact cont in gravityContacts)
				{
					if (cont.collider == contact.otherCollider)
					{
						gc = cont;
						break;
					}
				}

				if (gc != null)
				{
					if (isValidSurface(gc.collider, contact.point, normal))
					{
						gc.normal = normal;
						gc.point = contact.point;
						gc.validUntil = Time.time + 0.25f;
					}
					else
						gravityContacts.Remove(gc);
				}
				else if (isValidSurface(contact.otherCollider, contact.point, normal))
				{
					gc = new GravityContact();
					gc.collider = contact.otherCollider;
					gc.normal = normal;
					gc.point = contact.point;
					gc.validUntil = Time.time + 0.25f;

					gravityContacts.Add(gc);
				}
			}
		}
	}
}
