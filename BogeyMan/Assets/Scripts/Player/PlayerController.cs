using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Player player = null;
    [SerializeField] private PlayerInput playerInput = null;
	[SerializeField] private Rigidbody rb = null;
	[SerializeField] private Animator characterAnimator = null;
	[SerializeField] private Animator hitBoxesAnimator = null;
	[SerializeField] private Transform partToRotate = null;

	[SerializeField] private Weapon weapon = Weapon.Censer;

	Vector2 movementDirection;
	Vector2 aimDirection;
	private float speed;
	private bool stuned = false;
	private bool canAttack = true;
	private bool canAim = true;

	private void Start()
	{
		ResetSpeed();
	}

	#region Movements
	public void Move(InputAction.CallbackContext context)
	{
		movementDirection = context.ReadValue<Vector2>();
		Vector2 movement = movementDirection * speed / Time.deltaTime;
		rb.velocity = new Vector3(movement.x, 0, movement.y);
		characterAnimator.SetFloat("speed", movementDirection.magnitude);
	}

	public void Aim(InputAction.CallbackContext context)
	{
		if (!canAim) return;
		Vector2 aim = context.ReadValue<Vector2>();
		if (aim.magnitude < 0.1f) return;
		aimDirection = aim.normalized;
		float angle = Mathf.Atan2(-aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
		partToRotate.rotation = Quaternion.Euler(0, angle, 0);
	}

	private void DecreaseSpeed(int percentage)
	{
		speed *= (100 - percentage) * 0.01f;
	}

	private void ResetSpeed()
	{
		speed = player.settings.movementSpeed;
	}

	public void Knockback(Vector2 direction)
	{
		rb.AddForce(direction * player.settings.knockbackDistanceWhenHit, ForceMode.Impulse);
	}
	#endregion Movements

	#region Stun
	public void Stun(float duration)
	{
		if (stuned == false)
		{
			StartCoroutine(StunCoroutine(duration));
		}
	}

	private IEnumerator StunCoroutine(float duration)
	{
		stuned = true;
		playerInput.enabled = false;
		yield return new WaitForSeconds(duration);
		playerInput.enabled = true;
		stuned = false;
	}
	#endregion Stun

	#region Attacks

	#region LightAttack
	public void LightAttack(InputAction.CallbackContext context)
	{
		if (!context.started || !canAttack) return;

		canAttack = false;
		canAim = false;
		hitBoxesAnimator.SetTrigger("LightAttack");
		DecreaseSpeed(player.settings.lightAttackSpeedReductionPercentage);
		player.SetInvulnerability(true);
	}

	private void LightAttackFinished()
	{
		StartCoroutine(WaitForCooldown(player.settings.lightAttackCooldown));
		ResetSpeed();
		player.SetInvulnerability(false);
		canAim = true;
	}
	#endregion LightAttack

	#region HeavyAttack
	public void HeavyAttack(InputAction.CallbackContext context)
	{
		if (!context.started || !canAttack) return;

		canAttack = false;
		canAim = false;
		DecreaseSpeed(player.settings.heavyAttackChargeSpeedReductionPercentage);
		StartCoroutine(HeavyAttackCharge());
	}

	private IEnumerator HeavyAttackCharge()
	{
		yield return new WaitForSeconds(player.settings.heavyAttackChargeDuration);

		hitBoxesAnimator.SetTrigger("HeavyAttack");
		ResetSpeed();
		DecreaseSpeed(100);
		player.SetInvulnerability(true);
	}

	private void HeavyAttackFinished()
	{
		StartCoroutine(WaitForCooldown(player.settings.heavyAttackCooldown));
		ResetSpeed();
		player.SetInvulnerability(false);
		canAim = true;
	}
	#endregion HeavyAttack

	#region SpecialAttack
	public void SpecialAttack(InputAction.CallbackContext context)
	{
		if (!context.started || !canAttack)
			return;

		switch (weapon)
		{
			case Weapon.Shovel:
			{
				break;
			}
			case Weapon.Censer:
			{
				canAttack = false;
				canAim = false;
				DecreaseSpeed(100);
				StartCoroutine(CenserSpecialAttackCharge());
				break;
			}
		}
	}

	private IEnumerator CenserSpecialAttackCharge()
	{
		yield return new WaitForSeconds(player.settings.censerAttackChargeDuration);

		hitBoxesAnimator.SetTrigger("CenserSpecialAttack");
		ResetSpeed();
		DecreaseSpeed(- player.settings.censerAttackSpeedIncreasePercentage);
		player.SetInvulnerability(true);
	}

	private void CenserSpecialAttackFinished()
	{
		StartCoroutine(WaitForCooldown(player.settings.censerAttackCooldown));
		ResetSpeed();
		player.SetInvulnerability(false);
		canAim = true;
	}
	#endregion SpecialAttack

	private IEnumerator WaitForCooldown(float cooldown)
	{
		yield return new WaitForSeconds(cooldown);
		canAttack = true;
	}

	#endregion Attacks
}

public enum Weapon
{
	Shovel, Censer
}
