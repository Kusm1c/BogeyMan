using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private float remainingTime;
    public float RemainingTime
    {
        get => remainingTime;
        set => remainingTime = value;
    }
    
    public bool IsTimeUp => remainingTime <= 0;
    
    private int minutes;
    private int seconds;

    void Update()
    {

        remainingTime -= Time.deltaTime;
        minutes = Mathf.FloorToInt(remainingTime / 60);
        seconds = Mathf.FloorToInt(remainingTime % 60);
        GetComponent<TextMeshProUGUI>().text = minutes.ToString("00") + ":" + seconds.ToString("00");
        if (remainingTime <= 0)
        {
            remainingTime = 0;
        }
        
    }
}
