using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/Player Settings")]
public class PlayerSettings_SO : ScriptableObject
{
	[field : Header("Life")]
	[field: SerializeField] public int maxLife { get; private set; } = 100;

	[field: Header("Movement")]
	[field: SerializeField] public float movementSpeed { get; private set; } = 1f;
	[field: SerializeField] public float knockbackDistanceWhenHit { get; private set; } = 1f;
	[field: SerializeField] public float knockbackSpeedWhenHit { get; private set; } = 8f;
	[field: SerializeField] public float invulnerabilityDurationWhenHit { get; private set; } = 2.5f;

	[field : Header("Light attack")]
	[field: SerializeField] public int lightAttackDamage { get; private set; } = 1;
	[field: SerializeField] public float lightAttackCooldown { get; private set; } = 0.05f;
	[field : Range(0, 100)]
	[field: SerializeField] public int lightAttackSpeedReductionPercentage { get; private set; } = 15;
	[field: SerializeField] public float lightAttackProjectionForce { get; private set; } = 0.2f;
	[field: SerializeField] public float lightAttackProjectionDuration { get; private set; } = 0.3f;

	[field: Header("Heavy attack")]
	[field: SerializeField] public int heavyAttackDamage { get; private set; } = 2;
	[field : SerializeField] public float heavyAttackCooldown { get; private set; } = 1.5f;
	[field: Range(0, 100)]
	[field: SerializeField] public int heavyAttackChargeSpeedReductionPercentage { get; private set; } = 40;
	[field: SerializeField] public float heavyAttackChargeDuration { get; private set; } = 0.8f;
	[field: SerializeField] public float heavyAttackProjectionForce { get; private set; } = 0.4f;
	[field: SerializeField] public float heavyAttackProjectionDuration { get; private set; } = 0.4f;

	[field: Header("Censer special attack")]
	[field: SerializeField] public int censerAttackDamage { get; private set; } = 2;
	[field: SerializeField] public float censerAttackCooldown { get; private set; } = 1.5f;
	[field: Range(0, 100)]
	[field: SerializeField] public int censerAttackSpeedIncreasePercentage { get; private set; } = 15;
	[field: SerializeField] public float censerAttackChargeDuration { get; private set; } = 0.8f;
	[field: SerializeField] public float censerAttackProjectionForce { get; private set; } = 0.8f;
	[field: SerializeField] public float censerAttackProjectionDuration { get; private set; } = 0.2f;

	[field: Header("Grab")]
	[field: SerializeField] public int throwDamage { get; private set; } = 1;
	[field: SerializeField] public float throwSpeed { get; private set; } = 1;
	[field: SerializeField] public float throwDuration { get; private set; } = 2;



	[field: SerializeField] public float slowMotionReflectPRange { get; private set; } = 3f;
	[field: SerializeField] public float reflectSpeedMultiplier { get; private set; } = 1.3f;
	[field: SerializeField] public int reflectSAdditionnalDamage { get; private set; } = 1;
	[field: SerializeField] public float slowmotionTimeScale{ get; private set; } = 0.04f;

}
