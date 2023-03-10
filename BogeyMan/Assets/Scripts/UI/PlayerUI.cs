using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image filledLifeBar = null;

	private void Start()
	{
		filledLifeBar.fillAmount = 1;
	}

	public void UpdateLifeBar(int newLife, int maxLife)
	{
		filledLifeBar.fillAmount = (float)newLife / maxLife;
	}
}