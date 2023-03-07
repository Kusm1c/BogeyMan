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

	private void FixedUpdate()
	{
		Move();
	}

	private void Move()
	{
		Vector2 direction = playerInput.actions["Movement"].ReadValue<Vector2>();
		Vector2 movement = direction * speed;
		rb.velocity = (new Vector3(movement.x, 0, movement.y));
	}
}
