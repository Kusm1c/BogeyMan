using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWorldUI : MonoBehaviour
{
	[SerializeField] private Transform playerRotation;
    [SerializeField] private GameObject aimArrow = null;
	[SerializeField] private GameObject revivalGauge = null;
	[SerializeField] private Image filledRevivalGauge = null;

	Camera cam;

    // Aim arrow
    private bool isArrowActive = false;
    private Transform aimedTransform;

	private void Start()
	{
		cam = Camera.main;
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
			aimArrow.transform.rotation = Quaternion.Euler(0, playerRotation.rotation.y, 0);
		}
	}

	public void FillRevivalGauge(float fillAmount)
	{
		filledRevivalGauge.fillAmount = fillAmount;
	}

	public void SetRevivalGaugeVisible(bool visible)
	{
		revivalGauge.SetActive(visible);
	}
}
