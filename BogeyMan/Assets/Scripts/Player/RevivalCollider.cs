using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalCollider : MonoBehaviour
{
	[SerializeField] Player player = null;

	private void OnTriggerEnter(Collider other)
	{
		Player ally = other.GetComponent<Player>();
		if (ally != null)
		{
			ally.playerState.isOnDeadAlly = true;
			player.worldUi.SetRevivalGaugeVisible(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Player ally = other.GetComponent<Player>();
		if (ally != null)
		{
			ally.playerState.isOnDeadAlly = false;
			player.worldUi.SetRevivalGaugeVisible(false);
		}
	}
}
