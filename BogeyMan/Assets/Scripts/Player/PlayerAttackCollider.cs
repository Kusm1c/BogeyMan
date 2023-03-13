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
		if (other.gameObject.GetComponent<Enemies.Enemy>() == true)
		{
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
		else
		{
			ThrownObjectParent thrownObject = other.gameObject.GetComponent<ThrownObjectParent>();
			if (thrownObject != null && thrownObject.LastHitingPlayer != player)
			{
				Vector3 direction = new Vector3(player.playerController.aimDirection.x, 0, player.playerController.aimDirection.y);
				direction.y = 0;
				direction = direction.normalized;

				thrownObject.Throw(thrownObject.Speed * player.settings.reflectSpeedMultiplier,
									thrownObject.Duration, 
									player,
									direction, 
									thrownObject.Damage + player.settings.reflectAdditionnalDamage);

				if (GameManager.Instance.SlowMotionIsActive == true)
				{
					GameManager.Instance.SlowMotionIsActive = false;
					Time.timeScale = 1;
					Time.fixedDeltaTime = 0.02F * Time.timeScale;
				}
			}
			
			/*Projectile projectile = other.gameObject.GetComponent<Projectile>();
			else if (projectile)
			{
				// renvoi dans l'autre sens

			}*/
		}

	}
}
