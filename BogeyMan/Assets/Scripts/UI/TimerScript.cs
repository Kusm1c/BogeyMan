using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private float remainingTime;
    [HideInInspector] public bool isWorking = false;
    [SerializeField] private TMP_Text text = null;

    public float RemainingTime
    {
        get => remainingTime;
        set => remainingTime = value;
    }
    
    public bool IsTimeUp => remainingTime <= 0;
    
    private int minutes;
    private int seconds;

	private void Start()
	{
        minutes = Mathf.FloorToInt(remainingTime / 60);
        seconds = Mathf.FloorToInt(remainingTime % 60);
        text.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

	void Update()
    {
        if (isWorking == false)
            return;
        remainingTime -= Time.deltaTime;
        minutes = Mathf.FloorToInt(remainingTime / 60);
        seconds = Mathf.FloorToInt(remainingTime % 60);
        text.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameManager.Instance.WinGame();
        }
    }
}
