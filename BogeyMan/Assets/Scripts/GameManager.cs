using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    [field: SerializeField] public Player[] Players {get; private set; } = new Player[2];
    public bool SlowMotionIsActive = false;
    [HideInInspector] public List<InputDevice> Gamepads = new List<InputDevice>();
    [field: SerializeField] public CanvasManager Hud { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Time.timeScale = 0;
    }

    public void StartTheGame()
    {   
        foreach(Player player in Players)
		{
            player.playerController.AssignInputDevice();
		}
        Hud.startText.SetActive(true);
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
	{
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1;
        Hud.timer.isWorking = true;
    }

    public void QuitTheGame()
    {
        //TODO
        throw new System.NotImplementedException();
    }

    public void LoseGame()
	{
        Hud.gameOverText.SetActive(true);
        StartCoroutine(ReloadGame());
    }

    public void WinGame()
    {
        Hud.victoryText.SetActive(true);
        StartCoroutine(ReloadGame());
    }

    private IEnumerator ReloadGame()
	{
        Hud.timer.isWorking = false;
        yield return new WaitForSecondsRealtime(0.7f);
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(1.0f);
        Hud.loadingScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
