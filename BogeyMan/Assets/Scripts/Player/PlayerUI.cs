using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] private Transform playerRotation;
    [SerializeField] private Image filledLifeBar = null;
    [SerializeField] private GameObject aimArrow = null;

	Camera cam;

    // Aim arrow
    private bool isArrowActive = false;
    private Transform aimedTransform;

	private void Start()
	{
		filledLifeBar.fillAmount = 1;
		cam = Camera.main;
	}

	public void UpdateLifeBar(int newLife, int maxLife)
	{
		filledLifeBar.fillAmount = (float)newLife / maxLife;
	}

    public void SetAimArrowActive(bool active, Transform transformToAim = null)
	{
        isArrowActive = active;
		aimArrow.SetActive(active);
        if (transformToAim != null)
		{
            aimedTransform = transformToAim;
		}
	}

	private void Update()
	{
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);

		if (isArrowActive)
		{
			//float angle = Mathf.Atan2(-aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
			aimArrow.transform.rotation = Quaternion.Euler(0, playerRotation.rotation.y, 0);
		}
	}
}
