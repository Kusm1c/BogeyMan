using System.Collections;
using System.Collections.Generic;
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
	[field: SerializeField] public Grab grab { get; private set; }  = null;

	[SerializeField] private Weapon weapon = Weapon.Censer;

	Vector2 movementDirection;
	public Vector2 aimDirection { get; private set; } = Vector2.right;
	private float speed;
	private bool stuned = false;

	private bool canLightAttack = true;
	private bool canHeavyAttack = true;
	private bool canSpecialAttack = true;

	private void Start()
	{
		ResetSpeed();
	}

	#region Movements
	private void FixedUpdate()
	{
		if (player.playerState.canMove == true)
        {
			Move();
		}
        else
        {
			rb.velocity = Vector3.zero;
			characterAnimator.SetFloat("speed", 0);
		}
	}

	public void Move()
	{
		movementDirection = playerInput.actions["Movement"].ReadValue<Vector2>();
		movementDirection = movementDirection.normalized * Mathf.Min(movementDirection.magnitude, 1f);
		Vector2 movement = movementDirection * speed;
		rb.velocity = new Vector3(movement.x, 0, movement.y);
		characterAnimator.SetFloat("speed", movementDirection.magnitude * speed / player.settings.movementSpeed);
	}

	public void Aim(InputAction.CallbackContext context)
	{
		if (!player.playerState.canAim) return;
		Vector2 aim = context.ReadValue<Vector2>();
		if (aim.magnitude < 0.1f) return;
		aimDirection = aim.normalized;
		float angle = Mathf.Atan2(-aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
		partToRotate.rotation = Quaternion.Euler(0, angle, 0);
	}

	private void DecreaseSpeed(int percentage)
	{
		speed = player.settings.movementSpeed * (100 - percentage) * 0.01f;
	}

	private void ResetSpeed()
	{
		speed = player.settings.movementSpeed;
	}

	public void Knockback(Vector2 direction)
	{

		player.playerState.isKnockedBack = true;

		Vector3 directionVector3 = new Vector3(direction.x, 0, direction.y);
		rb.AddForce(directionVector3.normalized * player.settings.knockbackSpeedWhenHit, ForceMode.VelocityChange);
		StartCoroutine(WaitForEndOfKnockback());
	}

	private IEnumerator WaitForEndOfKnockback()
    {
		float duration = player.settings.knockbackDistanceWhenHit / player.settings.knockbackSpeedWhenHit;
		yield return new WaitForSeconds(duration);

		rb.AddForce(Vector3.zero, ForceMode.VelocityChange);
		player.playerState.isKnockedBack = false;
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
		if (!context.started || !player.playerState.canAttack) return;

		player.playerState.isAttacking = true;
		hitBoxesAnimator.SetTrigger("LightAttack");
		characterAnimator.SetTrigger("LightAttack");
		DecreaseSpeed(player.settings.lightAttackSpeedReductionPercentage);
		player.SetInvulnerability(true);
	}

	private void LightAttackFinished()
	{
		StartCoroutine(WaitForLightAttackCooldown(player.settings.lightAttackCooldown));
		ResetSpeed();
		player.SetInvulnerability(false);
		player.playerState.isAttacking = false;
	}

	private IEnumerator WaitForLightAttackCooldown(float cooldown)
	{
		canLightAttack = false;
		yield return new WaitForSeconds(cooldown);
		canLightAttack = true;
	}
	#endregion LightAttack

	#region HeavyAttack
	public void HeavyAttack(InputAction.CallbackContext context)
	{
		if (!context.started || !player.playerState.canAttack) return;

		player.playerState.isAttacking = true;
		DecreaseSpeed(player.settings.heavyAttackChargeSpeedReductionPercentage);
		StartCoroutine(HeavyAttackCharge());
		characterAnimator.SetTrigger("HeavyAttack");
	}

	private IEnumerator HeavyAttackCharge()
	{
		yield return new WaitForSeconds(player.settings.heavyAttackChargeDuration);

		hitBoxesAnimator.SetTrigger("HeavyAttack");
		DecreaseSpeed(100);
		player.SetInvulnerability(true);
	}

	private void HeavyAttackFinished()
	{
		StartCoroutine(WaitForHeavyAttackCooldown(player.settings.heavyAttackCooldown));
		ResetSpeed();
		player.SetInvulnerability(false);
		player.playerState.isAttacking = false;
	}

	private IEnumerator WaitForHeavyAttackCooldown(float cooldown)
	{
		canHeavyAttack = false;
		yield return new WaitForSeconds(cooldown);
		canHeavyAttack = true;
	}
	#endregion HeavyAttack

	#region SpecialAttack
	public void SpecialAttack(InputAction.CallbackContext context)
	{
		if (!context.started || !player.playerState.canAttack)
			return;

		switch (weapon)
		{
			case Weapon.Shovel:
			{
				break;
			}
			case Weapon.Censer:
			{
				player.playerState.isAttacking = true;
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
		DecreaseSpeed(- player.settings.censerAttackSpeedIncreasePercentage);
		player.SetInvulnerability(true);
	}

	private void CenserSpecialAttackFinished()
	{
		StartCoroutine(WaitForSpecialAttackCooldown(player.settings.censerAttackCooldown));
		ResetSpeed();
		player.SetInvulnerability(false);
		player.playerState.isAttacking = false;
	}

	private IEnumerator WaitForSpecialAttackCooldown(float cooldown)
	{
		canSpecialAttack = false;
		yield return new WaitForSeconds(cooldown);
		canSpecialAttack = true;
	}
	#endregion SpecialAttack

	#endregion Attacks

	public void Grab(InputAction.CallbackContext context)
	{
		if (!context.performed)
			return;

		grab.GrabInput();
	}
}

public enum Weapon
{
	Shovel, Censer
}
