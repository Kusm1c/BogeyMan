using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [field : SerializeField] public PlayerUI[] playerUIs { get; private set; } = new PlayerUI[2];

    [field: SerializeField] public GameObject startText { get; private set; } = null;
    [field: SerializeField] public GameObject gameOverText { get; private set; } = null;
    [field: SerializeField] public GameObject victoryText { get; private set; } = null;
    [field: SerializeField] public GameObject loadingScreen { get; private set; } = null;
    [field: SerializeField] public TimerScript timer { get; private set; } = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private Button resumeButton;
    

	private void Awake()
	{
        GameManager.Instance.Hud = this;
	}

    public void PauseTheGame()
    {
        if (pauseMenu.activeSelf == false)
		{
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            resumeButton.Select();
        }
		else
		{
            ResumeTheGame();
		}
    }

    public void ResumeTheGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
