using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput = null;
	[SerializeField] private Rigidbody rb = null;

	[SerializeField] private float speed = 1;

	[SerializeField] private float fastAttackRange = 1f;
	[Range(0, 180)]
	[SerializeField] private float fastAttackRadius = 90;

	Vector2 movementDirection;
	Vector2 aimDirection;

	private bool stuned = false;

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

	public void FastAttack(InputAction.CallbackContext context)
	{
		RaycastHit[] enemiesHit;
		enemiesHit = Physics.SphereCastAll(transform.position, fastAttackRange, 
			(Vector3)aimDirection, LayerMask.NameToLayer("Enemies"));
		{
			foreach( RaycastHit hit in enemiesHit)
			{
				Vector2 vectorBetweenPlayerAndEnemy = transform.position - hit.transform.position;
				Vector2 playerForwardVector = transform.right;
				float dotProduct = Vector2.Dot(vectorBetweenPlayerAndEnemy, playerForwardVector);
				if ( dotProduct > Mathf.Lerp(1, 0, fastAttackRadius / 180 ))
				{
					// Enemy hit
					print(hit.transform.name);
					Destroy(hit.transform.gameObject);
				}
			}
		}
	}

	public void HeavyAttack(InputAction.CallbackContext context)
	{

	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + transform.right * fastAttackRange);
	}
#endif
}
