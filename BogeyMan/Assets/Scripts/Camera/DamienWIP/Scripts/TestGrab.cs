using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrab : MonoBehaviour
{
    [SerializeField] private GameObject grabHitbox;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            grabHitbox.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            grabHitbox.SetActive(false);
        }
    }
}
