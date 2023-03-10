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

    [SerializeField] private float distance;
    [SerializeField] private float lastDistance;
    [SerializeField] private float minDistanceChangeToFocus = 1f;
    private float zoom;
    
    [Header("Camera shake")]
    [SerializeField] private AnimationCurve shakeCurve;
    [SerializeField] private float time = 0;
    
    public static CameraControl instance;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(ScreenShake());

        transform.position = (player1.transform.position + player2.transform.position) / 2f;
        transform.position += offset;
        transform.rotation = rotation;
        
        distance = Vector3.Distance(player1.transform.position, player2.transform.position);
        if (!(distance - lastDistance > minDistanceChangeToFocus)) return;
        
        zoom = Mathf.Clamp(distance, minZoom, maxZoom);

        offset = Vector3.Lerp(offset, offset.normalized * zoom, Time.deltaTime * zoomSpeed);

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
}
