using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVfx : MonoBehaviour
{
	[SerializeField] private Transform playerPartToRotate = null;

	[Header("VFX References")]
    [SerializeField] private ParticleSystem lightAttackVfx = null;

    public void PlayLightAttackVfx()
	{
		lightAttackVfx.Play();
	}
}
