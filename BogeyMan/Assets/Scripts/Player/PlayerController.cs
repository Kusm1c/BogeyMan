using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput = null;
	[SerializeField] private Rigidbody rb = null;

	[SerializeField] private float speed = 1;

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
}
