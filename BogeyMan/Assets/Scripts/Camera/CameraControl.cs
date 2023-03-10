using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector4 = System.Numerics.Vector4;

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

    private float distance;
    private float zoom;
    
    [Header("Camera shake")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.5f;

    public static CameraControl instance;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (player1 == null || player2 == null)
        {
            Player[] players = FindObjectsOfType<Player>();
            if (players.Length > 0)
            {
                player1 = players[0].gameObject;
                
                if (players.Length > 1)
                {
                    player2 = players[1].gameObject;
                }
            }
            return;
        }
        transform.position = (player1.transform.position + player2.transform.position) / 2f;
        transform.position += offset;
        transform.rotation = rotation;
        
        distance = Vector3.Distance(player1.transform.position, player2.transform.position);
        zoom = Mathf.Clamp(distance, minZoom, maxZoom);
        
        offset = Vector3.Lerp(offset, offset.normalized * zoom, Time.deltaTime * zoomSpeed);

    }

    public IEnumerator ScreenShake()
    {
        float elapsed = 0.0f;
        Vector3 originalCamPos = transform.position;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalCamPos;
    }
}
