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

    // Aim arrow
    private bool isArrowActive = false;
    private Transform aimedTransform;

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
