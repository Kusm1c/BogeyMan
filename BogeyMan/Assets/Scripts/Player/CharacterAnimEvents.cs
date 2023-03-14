using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimEvents : MonoBehaviour
{
    [SerializeField] private Player player = null;

    public void Grab()
	{
		player.playerController.grab.GrabInput();
	}

	public void Throw()
	{
		player.playerController.grab.GrabInput();
	}
}
