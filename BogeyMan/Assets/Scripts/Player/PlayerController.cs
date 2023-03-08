using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Player player = null;
    [SerializeField] private PlayerInput playerInput = null;
	[SerializeField] private Rigidbody rb = null;
	[SerializeField] private Animator animator = null;

	Vector2 movementDirection;
	Vector2 aimDirection;
	private float speed;
	private bool stuned = false;
	private bool canAttack = true;

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
	}

	public void Aim(InputAction.CallbackContext context)
	{
		if (!canAttack) return;
		Vector2 aim = context.ReadValue<Vector2>();
		if (aim.magnitude < 0.1f) return;
		aimDirection = aim.normalized;
		float angle = Mathf.Atan2(-aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, angle, 0);
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
		rb.AddForce(direction * player.settings.knockbackForceWhenHit, ForceMode.Impulse);
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
		animator.SetTrigger("LightAttack");
		DecreaseSpeed(player.settings.lightAttackSpeedReductionPercentage);
		player.SetInvulnerability(true);
	}

	public void LightAttackFinished()
	{
		StartCoroutine(WaitForCooldown(player.settings.lightAttackCooldown));
		ResetSpeed();
		player.SetInvulnerability(false);
	}
	#endregion LightAttack

	#region HeavyAttack
	public void HeavyAttack(InputAction.CallbackContext context)
	{
		if (!context.started || !canAttack) return;

		canAttack = false;
		DecreaseSpeed(player.settings.heavyAttackChargeSpeedReductionPercentage);
		StartCoroutine(HeavyAttackCharge());
	}

	private IEnumerator HeavyAttackCharge()
	{
		yield return new WaitForSeconds(player.settings.heavyAttackChargeDuration);

		animator.SetTrigger("HeavyAttack");
		ResetSpeed();
		DecreaseSpeed(100);
		player.SetInvulnerability(true);
	}

	public void HeavyAttackFinished()
	{
		StartCoroutine(WaitForCooldown(player.settings.heavyAttackCooldown));
		ResetSpeed();
		player.SetInvulnerability(false);
	}
	#endregion HeavyAttack

	private IEnumerator WaitForCooldown(float cooldown)
	{
		yield return new WaitForSeconds(cooldown);
		canAttack = true;
	}

	#endregion Attacks

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Enemies"))
		{
			Destroy(other.gameObject);
		}
	}
}
