using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput = null;
	[SerializeField] private Rigidbody rb = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private PlayerControllerSettings_SO settings = null;

	Vector2 movementDirection;
	Vector2 aimDirection;
	private float speed;
	private bool stuned = false;
	private bool canAttack = true;

	private void Start()
	{
		ResetSpeed();
	}

	private void FixedUpdate()
	{
		Move();
		Aim();
	}

	private void Move()
	{
		movementDirection = playerInput.actions["Movement"].ReadValue<Vector2>();
		Vector2 movement = movementDirection * speed;
		rb.velocity = new Vector3(movement.x, 0, movement.y);
	}

	private void Aim()
	{
		Vector2 aim = playerInput.actions["Aim"].ReadValue<Vector2>();
		if (aim.magnitude < 0.1f) return;
		aimDirection = aim.normalized;
		float angle = Mathf.Atan2(-aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, angle, 0);
	}

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

	public void LightAttack(InputAction.CallbackContext context)
	{
		if (!context.started || !canAttack) return;

		canAttack = false;
		animator.SetTrigger("LightAttack");
		DecreaseSpeed(settings.lightAttackSpeedReductionPercentage);
	}

	public void HeavyAttack(InputAction.CallbackContext context)
	{
		if (!context.started || !canAttack) return;

		canAttack = false;
		animator.SetTrigger("HeavyAttack");
		DecreaseSpeed(100);
	}

	public void LightAttackFinished()
	{
		StartCoroutine(WaitForCooldown(settings.lightAttackCooldown));
		ResetSpeed();
	}

	public void HeavyAttackFinished()
	{
		StartCoroutine(WaitForCooldown(settings.heavyAttackCooldown));
		ResetSpeed();
	}

	private IEnumerator WaitForCooldown(float cooldown)
	{
		yield return new WaitForSeconds(cooldown);
		canAttack = true;
	}

	private void DecreaseSpeed(int percentage)
	{
		speed *= (100 - percentage) * 0.01f;
	}

	private void ResetSpeed()
	{
		speed = settings.movementSpeed;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Enemies"))
		{
			print("enemy hit");
			Destroy(other.gameObject);
		}
	}
}
