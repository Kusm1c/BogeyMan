using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
	[SerializeField] private Player player = null;
	[SerializeField] private AttackType attackType;

	private enum AttackType
	{
		Light, Heavy, HeavyProjection, CenserSpecial
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.GetComponent<Enemies.Enemy>()) return;

		Enemies.Enemy enemy = other.gameObject.GetComponent<Enemies.Enemy>();
		Vector2 direction = (enemy.transform.position - transform.position).normalized;

		switch (attackType)
		{
			case AttackType.Light:
			{
				enemy.TakeHit(player.settings.lightAttackProjectionForce, direction, player.settings.lightAttackDamage);
				break;
			}
			case AttackType.Heavy:
			{
				enemy.TakeHit(0, direction, player.settings.heavyAttackDamage);
				break;
			}
			case AttackType.HeavyProjection:
			{
				enemy.TakeHit(player.settings.heavyAttackProjectionForce, direction, 0);
				break;
			}
			case AttackType.CenserSpecial:
			{
				enemy.TakeHit(player.settings.censerAttackProjectionForce, direction, player.settings.censerAttackDamage);
				break;
			}
		}
	}
}
