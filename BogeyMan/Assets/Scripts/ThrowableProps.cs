using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableProps : MonoBehaviour, IGrabable
{
	[SerializeField] private Rigidbody rb = null;

	[Header("Throw & Impact")]
	public bool throwable = true;
	[Min(0)] public float throwSpeed = 10f;
	[Min(0)] public float throwDuration = 1.5f;
	[Min(0)] public int damageOnCollisionWhenFlying = 1;
	[Min(0)] public float forceOnCollisionWhenFlying = 15f;
	[Min(0)] public int damageOnImpact = 2;
	[Min(0)] public float impactRadius = 3f;
	[Min(0)] public float impactForce = 20f;


	public Collider GetCollider()
	{
		return GetComponent<Collider>();
	}

	public float GetThrowSpeed()
	{
		return throwSpeed;
	}

	public float GetThrowDuration()
	{
		return throwDuration;
	}

	public int GetThrowDamage()
	{
		return damageOnCollisionWhenFlying;
	}

	public float GetThrowForce()
	{
		return forceOnCollisionWhenFlying;
	}

	public bool IsThrowable()
	{
		return throwable;
	}

	public void OnGrab(Player grabbingPlayer)
	{
		
	}

	public void OnImpact()
	{
		Collider[] enemiesHit;
		enemiesHit = Physics.OverlapSphere(transform.position, impactRadius);
		foreach (Collider hit in enemiesHit)
		{
			Enemies.Enemy enemy = hit.transform.GetComponent<Enemies.Enemy>();
			if (enemy != null)
			{
				enemy.TakeHit(impactForce, (enemy.transform.position - transform.position).normalized, damageOnImpact);
			}
		}

		rb.velocity = Vector3.zero;
	}

	public void OnRelease()
	{
		rb.velocity = Vector3.zero;
	}

	public void OnThrow()
	{
		
	}
}
