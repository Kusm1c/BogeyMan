using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[field: SerializeField] public PlayerSettings_SO settings { get; private set; } = null;
	[SerializeField] private PlayerController playerController = null;

    public int currentLife { get; private set; } = 0;

	private bool isInvulnerable = false;

	private void Start()
	{
		currentLife = settings.maxLife;
	}

	public void TakeHit(int damageTaken, Vector2 knockbackDirection)
	{
		currentLife -= damageTaken;
		playerController.Knockback(knockbackDirection);
		SetInvulnerability(true, settings.invulnerabilityDurationWhenHit);
	}

	public void SetInvulnerability(bool invulnerable, float duration = 0)
	{
		isInvulnerable = invulnerable;
		if (duration > 0)
		{
			StartCoroutine(InvulnerabilityCoroutine(duration));
		}
	}

	private IEnumerator InvulnerabilityCoroutine(float duration)
	{
		yield return new WaitForSeconds(duration);
		SetInvulnerability(false);
	}
}
