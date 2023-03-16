using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
	[field: SerializeField] public int playerIndex { get; private set; } = 0;
	[field: SerializeField] public PlayerSettings_SO settings { get; private set; } = null;
	[field: SerializeField] public PlayerController playerController { get; private set; } = null;
	[field: SerializeField] public PlayerVfx playerVfx { get; private set; } = null;
	[field : SerializeField] public PlayerWorldUI worldUi { get; private set; } = null;
	[field: SerializeField] public PlayerState playerState { get; private set; } = null;
	[SerializeField] private GameObject revivalCollider = null;
	private PlayerUI ui = null;

	public int currentLife { get; private set; } = 0;

    private void Awake()
    {
		if(GameManager.Instance.Players[0] != null)
        {
			playerIndex = 1;
        }
		GameManager.Instance.Players[playerIndex] = this;
		CinemachineTargetGroup targetGroup = GameObject.Find("Target Group").GetComponent<CinemachineTargetGroup>();
		targetGroup.m_Targets[playerIndex].target = transform;
    }

	private void Start()
	{
		ui = CanvasManager.Instance.playerUIs[playerIndex];
		currentLife = settings.maxLife;
		worldUi.SetRevivalGaugeVisible(false);
		worldUi.FillRevivalGauge(0);
		worldUi.SetAimArrowActive(false);
		revivalCollider.SetActive(false);
	}

	public void TakeHit(int damageTaken, Vector2 knockbackDirection)
	{
		if (playerState.isInvulnerable) return;

		currentLife -= damageTaken;
		ui.UpdateLifeBar(currentLife, settings.maxLife);
		playerController.Knockback(knockbackDirection);
		SetInvulnerability(true, settings.invulnerabilityDurationWhenHit);
		playerController.characterAnimator.SetFloat("Random", UnityEngine.Random.Range(0, 3));
		playerController.characterAnimator.SetTrigger("Hit");
		playerVfx.PlayHitVfx();
		if (currentLife <= 0)
		{
			Die();
		}
	}

	public void SetInvulnerability(bool invulnerable, float duration = 0)
	{
		playerState.isInvulnerable = invulnerable;
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
		playerState.isDead = true;
		// anim + feedback
		revivalCollider.SetActive(true);
		playerController.rb.isKinematic = true;
		SetInvulnerability(true);
		playerController.characterAnimator.SetBool("Dead", true);
	}

	#region Revival
	public void Revive() 
	{
		// anim + feedback
		CancelRevival();
		worldUi.SetRevivalGaugeVisible(false);
		revivalCollider.SetActive(false);
		StartCoroutine(ReviveCoroutine());
		SetInvulnerability(false);
		playerController.characterAnimator.SetBool("Dead", false);
		currentLife = settings.maxLife;
		ui.UpdateLifeBar(currentLife, settings.maxLife);
		playerVfx.PlayRevivingVfx(false);
		playerVfx.PlayReviveEndVfx();
	}

	private IEnumerator ReviveCoroutine()
	{
		yield return new WaitForSeconds(settings.revivalDelayAfterAnim);
		playerState.isDead = false;
		playerController.rb.isKinematic = false;
	}

	public void CancelRevival()
	{
		worldUi.FillRevivalGauge(0);
		playerVfx.PlayRevivingVfx(false);
	}

	public void StartRevivingAlly()
	{
		playerState.isRevivingAlly = true;
		StartCoroutine(IsRevivingAlly(Time.time));
	}

	public void StopRevivingAlly()
	{
		playerState.isRevivingAlly = false;
		StopAllCoroutines();
	}

	private IEnumerator IsRevivingAlly(float time)
	{
		float timePressingInput = 0;
		Player ally = GameManager.Instance.Players[playerIndex == 1 ? 0 : 1];
		ally.playerVfx.PlayRevivingVfx(true);

		while (timePressingInput < settings.revivalDuration)
		{
			yield return new WaitForEndOfFrame();
			timePressingInput += Time.deltaTime;
			ally.worldUi.FillRevivalGauge(timePressingInput / settings.revivalDuration);
		}

		playerState.isOnDeadAlly = false;
		ally.Revive();
		StopRevivingAlly();
	}
	#endregion Revival
}
