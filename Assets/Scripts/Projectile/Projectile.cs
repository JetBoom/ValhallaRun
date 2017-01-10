/*
* Base deterministic projectile behaviour.
* If they are given a position, direction, and start time, the end result should be the same no matter what.
* This class usually doesn't have to be overridden. It handles all projectiles that use ballistic physics with no drag and variable gravity.
*/

using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed = 30.0f;
	public float lifeTime = 5.0f;
	public int damage = 0;
	public DamageType damageType = DamageType.Generic;
	public float damageRadius = 0.0f;
	[Tooltip("Only if damageRadius > 0")]
	public float damageForce = 0.0f;
	[Tooltip("Use a SphereCast with this radius. If radius is 0, then use a LineCast.")]
	public float collisionRadius = 0.25f;
	[Tooltip("Created (only on the server) when projectile hits.")]
	public GameObject hitPrefab;
	[Tooltip("This is sent to clients, so it should only be used for effects or client things.")]
	public GameObject fxHit;
	public float gravityMultiplier = 0.0f;
	[Tooltip("These objects will be deparented and destroyed in 5 seconds when this projectile dies.")]
	public GameObject[] keepOnDeath;
	[Tooltip("If true then the position, velocity, etc. is determinstic from start position, direction, and time. Otherwise the transform will be sent through the network.")]
	public bool isDeterministic = true;

	private bool projectileHit;
	private Vector3 lastPhysicalPosition;
	public Vector3 startPosition;
	public Vector3 startDirection;
	public float startTime;

	void Awake()
	{
		lastPhysicalPosition = transform.position;

		if (lifeTime > 0)
			Invoke("DestroyProjectile", lifeTime);
	}

	void OnDestroy()
	{
		foreach (GameObject obj in keepOnDeath)
		{
			if (obj)
			{
				Transform trans = obj.transform;
				Vector3 oldpos = trans.position;
				Quaternion oldrot = trans.rotation;
				trans.parent = null;
				trans.position = oldpos;
				trans.rotation = oldrot;

				if (obj.GetComponent<ParticleSystem>())
					obj.GetComponent<ParticleSystem>().enableEmission = false;

				Destroy(obj, 5f);
			}
		}
	}

	public static GameObject CreateProjectile(string prefabid, GameObject owner)
	{
		Transform trans = owner.GetEyeTransform();
		//Vector3 forward = trans.forward;
		Vector3 pos = trans.position;
		Quaternion rot = trans.rotation;

		var projectile = PrefabStorage.Instantiate(prefabid, pos, rot);

		return projectile;
	}

	void Update()
	{
		UpdateRenderTransform();
	}

	void UpdateRenderTransform()
	{
		Vector3 oldposition = transform.position;
		Vector3 newposition = DeterminePosition();

		if (newposition != oldposition)
			transform.forward = (newposition - oldposition).normalized;

		transform.position = newposition;
	}

	void FixedUpdate()
	{
		Vector3 position = DeterminePosition();

		if (lastPhysicalPosition != position)
		{
			/*int prevlayer = 0;
			GameObject owner = gameObject.getOwner();
			if (owner)
			{
				prevlayer = owner.layer;
				owner.layer = Layers.TEMP;
			}*/

			bool hit = false;
			RaycastHit hitinfo;
			if (collisionRadius > 0)
			{
				if (Physics.SphereCast(lastPhysicalPosition, collisionRadius, (position - lastPhysicalPosition).normalized, out hitinfo, Vector3.Distance(position, lastPhysicalPosition), Mask.PROJECTILE))
					hit = true;
			}
			else if (Physics.Linecast(lastPhysicalPosition, position, out hitinfo, Mask.PROJECTILE))
				hit = true;

			/*if (owner)
				owner.layer = prevlayer;*/

			if (hit)
				OnProjectileCollide(hitinfo);
		}

		lastPhysicalPosition = position;
	}

	void DestroyProjectile()
	{
		Destroy(gameObject);
	}
	
	void OnProjectileCollide(RaycastHit hitinfo)
	{
		if (projectileHit)
			return;

		if (PreProjectileCollide(hitinfo))
			return;

		projectileHit = true;

		Vector3 hitnormal = hitinfo.normal;
		Vector3 hitpos = hitinfo.point + hitnormal * 0.1f;
		Quaternion hitnormalrotation = Quaternion.LookRotation(hitnormal);

		DoProjectileBehaviour(hitpos, hitnormal, hitinfo);

		if (fxHit)
			Effects.Dispatch(fxHit.name, hitpos, hitnormalrotation);

		if (hitPrefab)
			Instantiate(hitPrefab, hitpos, hitnormalrotation);

		Destroy(gameObject);
	}

	/// <summary>Called right before impacting with an object. return true to override default behaviour, resulting in a miss. Warning: this does not redo the ray trace.</summary>
	public virtual bool PreProjectileCollide(RaycastHit hitinfo)
	{
		return false;
	}

	/// <summary>Called when we hit something. This is where damage and game changes should happen.</summary>
	public virtual void DoProjectileBehaviour(Vector3 hitpos, Vector3 hitnormal, RaycastHit hitinfo)
	{
		/*if (damage > 0.0f)
		{
			if (damageRadius > 0.0f)
				UtilDamage.AreaDamage(hitpos, damage, damageRadius, damageType, damageForce, gameObject.getOwner(), gameObject);
			else if (hitinfo.collider)
				hitinfo.collider.gameObject.TakeDamage(damage, damageType, gameObject.getOwner(), gameObject, hitpos);
		}*/
	}

	/// <summary>Calculate the position that this projectile should have at this point in time, based on projectile characteristics.</summary>
	public virtual Vector3 DeterminePosition()
	{
		float dt = GameTime.time - startTime;

		// Very simple, constant velocity
		if (gravityMultiplier == 0f)
			return startPosition + dt * speed * startDirection;

		return startPosition + dt * speed * startDirection + dt * dt * gravityMultiplier * Physics.gravity;
	}

	/// <summary>Calculate the velocity that this projectile should have at this point in time, based on projectile characteristics. Not used internally.</summary>
	public virtual Vector3 DetermineVelocity()
	{
		if (gravityMultiplier == 0f)
			return startDirection * speed;

		return startDirection * speed + (GameTime.time - startTime) * gravityMultiplier * Physics.gravity;
	}

	/// <summary>Move the projectile to this position and direction, updating internal projectile values as you do so.</summary>
	public void StartFrom(Vector3 pos, Vector3 dir)
	{
		startPosition = pos;
		startDirection = dir;

		UpdateRenderTransform();

		lastPhysicalPosition = transform.position;
	}

	public void StartFrom(Vector3 pos, Vector3 dir, float absolute_start_time)
	{
		startTime = absolute_start_time;
		StartFrom(pos, dir);
	}
}
