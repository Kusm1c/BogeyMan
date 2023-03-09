using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
	[SerializeField] private Player player = null;
	[SerializeField] private AttackType attackType;

	private enum AttackType
	{
		Light, Heavy, CenserSpecial
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("Enemies"))
			return;

		switch (attackType)
		{
			case AttackType.Light:
			{
				break;
			}
			case AttackType.Heavy:
			{
				break;
			}
			case AttackType.CenserSpecial:
			{
				break;
			}
		}
	}
}
