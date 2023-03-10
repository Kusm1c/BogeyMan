using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private int playerIndex = 0;
	[field: SerializeField] public PlayerSettings_SO settings { get; private set; } = null;
	[SerializeField] private PlayerController playerController = null;
	[SerializeField] private PlayerWorldUI ui = null;

    public int currentLife { get; private set; } = 0;

	private bool isInvulnerable = false;

	private void Start()
	{
		currentLife = settings.maxLife;
	}

	public void TakeHit(int damageTaken, Vector2 knockbackDirection)
	{
		currentLife -= damageTaken;
		//ui.UpdateLifeBar(currentLife, settings.maxLife);
		playerController.Knockback(knockbackDirection);
		SetInvulnerability(true, settings.invulnerabilityDurationWhenHit);
		playerController.Stun(0.4f);
		if (currentLife <= 0)
		{
			Die();
		}
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

	private void Die()
	{

	}
}
