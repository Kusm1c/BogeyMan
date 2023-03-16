using UnityEngine;
using UnityEngine.InputSystem;

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
    
    public void QuitTheGame()
    {
        GameManager.Instance.QuitTheGame();
    }

	private void Awake()
	{
        Time.timeScale = 0;
    }

	InputDevice[] connectedDevices = new InputDevice[0];

    private void Update()
    {
        if (connectedDevices.Length == InputSystem.devices.Count)
            return;

        connectedDevices = InputSystem.devices.ToArray();
        GameManager.Instance.Gamepads.Clear();
        numberOfPlayersConnected = 0;
        foreach (InputDevice device in connectedDevices)
		{
            if (device.description.deviceClass.Equals("Gamepad")
                || device.description.deviceClass.Equals(""))
			{
                numberOfPlayersConnected++;
                GameManager.Instance.Gamepads.Add(device);
            }
		}
        
        switch (numberOfPlayersConnected)
        {
            case 0:
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(true); // player1 off
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(false); // player1 on
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(2).gameObject.SetActive(true); // player2 off
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(3).gameObject.SetActive(false); // player2 on
                transform.GetChild(0).GetChild(1).GetChild(4).gameObject.SetActive(false);
                break;
            case 1:
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(3).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(4).gameObject.SetActive(false);
                break;
            default:
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(2).GetChild(3).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(1).GetChild(3).GetChild(3).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(1).GetChild(4).gameObject.SetActive(true);
                break;
        }
    }
}
