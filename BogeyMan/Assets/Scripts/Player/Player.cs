using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[field: SerializeField] public int playerIndex { get; private set; } = 0;
	[field: SerializeField] public PlayerSettings_SO settings { get; private set; } = null;
	[field: SerializeField] public PlayerController playerController { get; private set; } = null;
	[SerializeField] private PlayerWorldUI worldUi = null;
	[field: SerializeField] public PlayerState playerState { get; private set; } = null;
	private PlayerUI ui = null;

    public int currentLife { get; private set; } = 0;

    private void Awake()
    {
		if(GameManager.Instance.Players[0] != null)
        {
			playerIndex = 1;
        }
		GameManager.Instance.Players[playerIndex] = this;
	}


	private void Start()
	{
		ui = CanvasManager.Instance.playerUIs[playerIndex];
		currentLife = settings.maxLife;
	}

	public void TakeHit(int damageTaken, Vector2 knockbackDirection)
	{
		currentLife -= damageTaken;
		ui.UpdateLifeBar(currentLife, settings.maxLife);
		playerController.Knockback(knockbackDirection);
		print(knockbackDirection);
		SetInvulnerability(true, settings.invulnerabilityDurationWhenHit);
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

	}
}
