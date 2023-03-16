using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [field : SerializeField] public PlayerUI[] playerUIs { get; private set; } = new PlayerUI[2];

    [field: SerializeField] public GameObject startText { get; private set; } = null;
    [field: SerializeField] public GameObject gameOverText { get; private set; } = null;
    [field: SerializeField] public GameObject victoryText { get; private set; } = null;
    [field: SerializeField] public GameObject loadingScreen { get; private set; } = null;
    [field: SerializeField] public TimerScript timer { get; private set; } = null;
}
