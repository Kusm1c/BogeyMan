using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    private int numberOfPlayersConnected = 0;
    
    public bool isWaitingForPlayers;

    public bool IsWaitingForPlayers
    {
        get => isWaitingForPlayers;
        set => isWaitingForPlayers = value;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void StartTheGame()
    {
        GameManager.Instance.StartTheGame();
    }
    
    // public void PauseTheGame()
    // {
    //     Time.timeScale = 0;
    // }
    //
    // public void ResumeTheGame()
    // {
    //     Time.timeScale = 1;
    // }
    
    public void QuitTheGame()
    {
        GameManager.Instance.QuitTheGame();
    }

    private void Update()
    {
        numberOfPlayersConnected = GameManager.Instance.Players.Length;

        if (!isWaitingForPlayers) return;
        switch (numberOfPlayersConnected)
        {
            case 0:
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(3).gameObject.SetActive(false);
                break;
            case 1:
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(3).gameObject.SetActive(false);
                break;
            case 2:
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(3).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(4).gameObject.SetActive(true);
                break;
        }
    }
}
