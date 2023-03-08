using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerSettings", menuName = "Scriptable Objects/Player Controller Settings")]
public class PlayerControllerSettings_SO : ScriptableObject
{
	[field: Header("Movement")]
	[field: SerializeField] public float movementSpeed { get; private set; } = 1f;

	[field : Header("Light attack")]
	[field: SerializeField] public float lightAttackCooldown { get; private set; } = 0.05f;
	[field: SerializeField] public int lightAttackSpeedReductionPercentage { get; private set; } = 15;

	[field: Header("Heavy attack")]
	[field : SerializeField] public float heavyAttackCooldown { get; private set; } = 1.5f;
	[field: SerializeField] public int heavyAttackChargeSpeedReductionPercentage { get; private set; } = 40;
}
