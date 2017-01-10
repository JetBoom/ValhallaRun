using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
	// Individual audio clips
	[Header("Sound")]
	public AudioClip[] footLandSoftSound;
	public AudioClip[] footLandHardSound;
	public AudioClip[] footStepSoftSound;
	public AudioClip[] footStepHardSound;
	public AudioClip[] jumpSound;
	public AudioClip[] deathSound;

	// Foot effects
	[Header("Effects")]
	public ParticleSystem hitGroundHardRightParticles;
	public ParticleSystem hitGroundHardLeftParticles;
	public ParticleSystem hitGroundSoftRightParticles;
	public ParticleSystem hitGroundSoftLeftParticles;
	public ParticleSystem landGroundHardParticles;
	public ParticleSystem landGroundSoftParticles;

	// Footsteps
	[Header("Footsteps")]
	public float footStepMinTime = 0.35f;
	public float footStepMaxTime = 3.0f;
	public float footStepMinSpeed = 3.0f;

	public PlayerController player;

	private float lastFootStep;
	private bool swapfoot;
	private float nextGroundHitEffect;
	private float nextJumpEffect;

	void Update()
	{
		PlayFootsteps();
	}

	public virtual void PlayJumpEffects()
	{
		float time = GameTime.time;
		if (time < nextJumpEffect) return;
		nextJumpEffect = time + 0.333f;

		if (jumpSound.Length > 0)
			GetComponent<AudioSource>().PlayOneShot(UtilRandom.RandomFromArray(jumpSound) as AudioClip);
	}

	public virtual void PlayDeathEffects()
	{
		if (deathSound.Length > 0)
			GetComponent<AudioSource>().PlayOneShot(UtilRandom.RandomFromArray(deathSound) as AudioClip);
	}

	public virtual void PlayGroundLandEffects()
	{
		float time = GameTime.time;
		if (time < nextGroundHitEffect) return;
		nextGroundHitEffect = time + 0.333f;

		if (footLandHardSound.Length > 0)
			GetComponent<AudioSource>().PlayOneShot(UtilRandom.RandomFromArray(footLandHardSound) as AudioClip);
		if (landGroundHardParticles)
			landGroundHardParticles.Play();

		// Send the camera info about the footstep for rolling effect.
		PlayerCamera pc = Camera.main.GetComponent<PlayerCamera>();
		if (pc)
			pc.HitGround();
	}

	protected virtual void PlayFootsteps()
	{
		/*if (!motor.shouldPlayFootsteps)
			return;

		// Play footstep effects based on how fast we're moving in our ground normalized plane.
		float speed = motor.velocity.magnitude;
		if (speed < footStepMinSpeed)
			return;

		// Next time we play is based on our speed.
		float speedPerMaxSpeed = Mathf.Min(1.0f, speed / motor.maxGroundSpeed);
		float nextFootStep = lastFootStep + Mathf.Max(footStepMinTime, footStepMaxTime * (1 - speedPerMaxSpeed));

		if (Time.time >= nextFootStep)
		{
			lastFootStep = Time.time;
			swapfoot = !swapfoot;

			//AudioClip[] audioclips = isOnSoftGround ? footStepSoftSound : footStepHardSound; // Hard or soft material sounds?
			AudioClip[] audioclips = footStepHardSound; // Hard or soft material sounds?
			AudioClip clip = UtilRandom.RandomFromArray(audioclips) as AudioClip;

			// Play softer/louder depending on current speed.
			float magnitude = 0.4f + speedPerMaxSpeed * 0.4f;

			if (clip != null)
				GetComponent<AudioSource>().PlayOneShot(clip, magnitude);

			//ParticleSystem system = isOnSoftGround ? swapfoot ? effects.hitGroundSoftRightParticles : effects.hitGroundSoftLeftParticles : swapfoot ? effects.hitGroundHardRightParticles : effects.hitGroundHardLeftParticles;
			ParticleSystem system = swapfoot ? hitGroundHardRightParticles : hitGroundHardLeftParticles;
			system.Play();

			// Send the camera info about the footstep for rolling effect.
			if (Player.localPlayer == player)
			{
				PlayerCamera pc = Camera.main.GetComponent<PlayerCamera>();
				if (pc)
					pc.DoFootstep(magnitude, swapfoot);
			}
		}*/
	}
}
