using UnityEngine;

public static class UtilDamage
{
	public static void AreaDamage(Vector3 origin, int damage, float radius, DamageType damagetype = DamageType.Generic, float force = 0.0f, GameObject attacker = null, GameObject inflictor = null, float minDamageMultiplier = 0.33f)
	{
		/*foreach (Collider collider in Physics.OverlapSphere(origin, radius))
		{
			Vector3 closest = collider.ClosestPointOnBounds(origin);
			if (!Physics.Linecast(origin, closest, Mask.WORLD))
			{
				float magnitude = Mathf.Clamp(1.0f - Vector3.Distance(origin, closest) / radius, minDamageMultiplier, 1.0f);

				damage = Mathf.CeilToInt((float)damage * magnitude);

				if (collider.gameObject.TakeDamage(damage, damagetype, attacker, inflictor, closest) && force != 0f)
				{
					Vector3 vel = magnitude * force * (collider.bounds.center - origin).normalized;

					if (collider.GetComponent<Rigidbody>() && !collider.GetComponent<Rigidbody>().isKinematic)
						collider.GetComponent<Rigidbody>().AddVelocity(vel);

					ICharacter character = (ICharacter)collider.GetComponent(typeof(ICharacter));
					if (character != null)
						character.motor.velocity += vel;
				}
			}
		}*/
	}
}
