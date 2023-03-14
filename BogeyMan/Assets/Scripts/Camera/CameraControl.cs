using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CameraControl : MonoBehaviour
{
    [Header("Zoom")]
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float zoomSpeed = 1f;
    [Space]

    [Header("Focus")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [Space]

    [Header("Position")]
    [SerializeField] private Quaternion rotation;
    [SerializeField] private Vector3 offset;
    [Space]

    [SerializeField] private float distance;
    [SerializeField] private Vector3 player1PositionOnScreen;
    [SerializeField] private Vector3 player2PositionOnScreen;
    [SerializeField] private Rect deadZone;
    [SerializeField] private float deadZoneSize = 0.1f;
    [SerializeField] private Image deadZoneImage;
    [SerializeField] private Rect zoomOutZone;
    [SerializeField] private float zoomOutZoneSize = 0.1f;
    [SerializeField] private Image zoomOutZoneImage;
    [SerializeField] private Rect zoomInZone;
    [SerializeField] private float zoomInZoneSize = 0.1f;
    [SerializeField] private Image zoomInZoneImage;
    
    private float zoom;
    
    [Header("Camera shake")]
    [SerializeField] private AnimationCurve shakeCurve;
    [SerializeField] private float time = 0;
    
    public static CameraControl instance;
    
    private void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        if (deadZoneImage != null && zoomOutZoneImage != null && zoomInZoneImage != null)
        {
            player1PositionOnScreen = Camera.main.WorldToScreenPoint(player1.transform.position);
            player2PositionOnScreen = Camera.main.WorldToScreenPoint(player2.transform.position);
            deadZone = new Rect(Screen.width * deadZoneSize, Screen.height * deadZoneSize,
                Screen.width * (1 - deadZoneSize * 2), Screen.height * (1 - deadZoneSize * 2));
            zoomOutZone = new Rect(Screen.width * zoomOutZoneSize, Screen.height * zoomOutZoneSize,
                Screen.width * (1 - zoomOutZoneSize * 2), Screen.height * (1 - zoomOutZoneSize * 2));
            zoomInZone = new Rect(Screen.width * zoomInZoneSize, Screen.height * zoomInZoneSize,
                Screen.width * (1 - zoomInZoneSize * 2), Screen.height * (1 - zoomInZoneSize * 2));
            deadZoneImage.rectTransform.sizeDelta = new Vector2(deadZone.width, deadZone.height);
            zoomOutZoneImage.rectTransform.sizeDelta = new Vector2(zoomOutZone.width, zoomOutZone.height);
            zoomInZoneImage.rectTransform.sizeDelta = new Vector2(zoomInZone.width, zoomInZone.height);

            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(ScreenShake());
            
            //if both players are in the zoom in zone 
            if (zoomInZone.Contains(player1PositionOnScreen) )
            {
                if (!zoomInZone.Contains(player2PositionOnScreen)) return;
                Debug.Log("Both players are in the zoom in zone");
                if (transform.position != (player1.transform.position + player2.transform.position) / 2f)
                {
                    var playerCenter = (player1.transform.position + player2.transform.position) / 2f;
                    transform.position = Vector3.Lerp(transform.position, playerCenter, Time.deltaTime * zoomSpeed);
                    return;
                }
                transform.position += offset;
                transform.rotation = rotation;
                
                distance = Vector3.Distance(player1.transform.position, player2.transform.position);
                zoom = Mathf.Clamp(distance, minZoom, maxZoom);
                offset = Vector3.Lerp(offset, offset.normalized * zoom, Time.deltaTime * zoomSpeed);
                return;
            }

            //if both players are in the deadzone and not in the zoom out zone neither the zoom in zone, do nothing
            if (deadZone.Contains(player1PositionOnScreen))
            {
                if (!deadZone.Contains(player2PositionOnScreen)) return;
                Debug.Log("Both players are in the deadzone");
                return;
            }

            //if both players are in the zoom out zone and not in the zoom in zone neither the dead zone, zoom out
            if (!zoomOutZone.Contains(player1PositionOnScreen)) return;
            {
                if (!zoomOutZone.Contains(player2PositionOnScreen) && 
                    zoomInZone.Contains(player2PositionOnScreen) && 
                    deadZone.Contains(player2PositionOnScreen)) return;
                Debug.Log("Both players are in the zoom out zone");
                
                if (transform.position != (player1.transform.position + player2.transform.position) / 2f)
                {
                    var playerCenter = (player1.transform.position + player2.transform.position) / 2f;
                    transform.position = Vector3.Lerp(transform.position, playerCenter, Time.deltaTime * zoomSpeed);
                    return;
                }
                transform.position += offset;
                transform.rotation = rotation;
                
                distance = Vector3.Distance(player1.transform.position, player2.transform.position);
                zoom = Mathf.Clamp(distance, minZoom, maxZoom);
                offset = Vector3.Lerp(offset, offset.normalized * zoom, Time.deltaTime * zoomSpeed);
            }
        }
        
        // distance = Vector3.Distance(player1.transform.position, player2.transform.position);      
        // zoom = Mathf.Clamp(distance, minZoom, maxZoom);

        // offset = Vector3.Lerp(offset, offset.normalized * zoom, Time.deltaTime * zoomSpeed);

    }

    public IEnumerator ScreenShake()
    {
        float timeElapsed = 0;
        while (timeElapsed < time)
        {
            timeElapsed += Time.deltaTime;
            float shake = shakeCurve.Evaluate(timeElapsed);
            transform.position = new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), Random.Range(-shake, shake));
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(offset, 0.1f);
    }
    
    //draw on screen
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"Zoom: {zoom}");
        GUI.Label(new Rect(10, 30, 100, 20), $"Distance: {distance}");
        GUI.Label(new Rect(10, 50, 100, 20), $"Offset: {offset}");
    }
}