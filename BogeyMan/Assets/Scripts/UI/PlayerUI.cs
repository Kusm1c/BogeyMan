using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] private Color greenColor;
	[SerializeField] private Color redColor;
	[SerializeField] private Image filledLifeBar = null;

	private void Start()
	{
		filledLifeBar.fillAmount = 1;
		filledLifeBar.color = greenColor;
	}

	public void UpdateLifeBar(int newLife, int maxLife)
	{
		float fillAmount = (float)newLife / maxLife;
		filledLifeBar.fillAmount = fillAmount;
		filledLifeBar.color = Color.Lerp(redColor, greenColor, fillAmount * 2);
	}
}