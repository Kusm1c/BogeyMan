using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonerUI : MonoBehaviour
{
    [SerializeField] private GameObject dismembermentGaugeParent = null;
    [SerializeField] private Image filledDismembermentGauge = null;

	private void Start()
	{
		SetGaugeVisible(false);
	}

	public void SetGaugeVisible(bool active)
	{
        dismembermentGaugeParent.SetActive(active);
	}

    public void FillGauge(float fillAmount)
	{
        filledDismembermentGauge.fillAmount = fillAmount;
	}
}
