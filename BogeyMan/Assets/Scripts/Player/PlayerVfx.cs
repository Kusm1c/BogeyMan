using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVfx : MonoBehaviour
{
	[Header("Attacks")]
	[SerializeField] private ParticleSystem lightAttackVfx = null;
	[SerializeField] private ParticleSystem heavyAttackVfx = null;
	[SerializeField] private ParticleSystem heavyAttackChargeVfx = null;
	[SerializeField] private ParticleSystem heavyAttackWeaponChargeVfx = null;
	[SerializeField] private ParticleSystem specialAttackVfx = null;

	[Header("Other")]
	[SerializeField] private ParticleSystem bloodVfx = null;

	private void PlayLightAttackVfx()
	{
		lightAttackVfx.Play();
	}

	private void PlayHeavyAttackVfx()
	{
		heavyAttackVfx.Play();
		heavyAttackChargeVfx.Stop();
		heavyAttackWeaponChargeVfx.Stop();
	}

	public void StartHeavyAttackCharge()
	{
		heavyAttackChargeVfx.Play();
		heavyAttackWeaponChargeVfx.Play();
	}

	public void PlayHitVfx()
	{
		bloodVfx.Play();
	}
}
