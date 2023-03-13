using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerFunny : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private Plane[] cameraFrustumPlanes;
    private Collider collider;
    
    private Vector3 screenPos;
    [SerializeField] private Vector2 deadZone;
    private void Start()
    {
        mainCamera = Camera.main;
        collider = GetComponent<Collider>();
    }
    void Update()
    {
        var bounds = collider.bounds;

        cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        if (!GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, bounds))
        {
            Debug.Log($"Object {gameObject.name} is not visible");
            switch (mainCamera.WorldToViewportPoint(transform.position).x)
            {
                case < 0:
                    Debug.Log("Object is not visible because it is too much to the left");
                    break;
                case > 1:
                    Debug.Log("Object is not visible because it is too much to the right");
                    break;
            }
            switch (mainCamera.WorldToViewportPoint(transform.position).y)
            {
                case < 0:
                    Debug.Log("Object is not visible because it is too much to the down");
                    break;
                case > 1:
                    Debug.Log("Object is not visible because it is too much to the up");
                    break;
            }
        }
        else
        {
            if (mainCamera.WorldToScreenPoint(transform.position).x < deadZone.x)
            {
                Debug.Log("Object is visible but too much to the left");
            }
            else if (mainCamera.WorldToScreenPoint(transform.position).x > Screen.width - deadZone.x)
            {
                Debug.Log("Object is visible but too much to the right");
            }
            else if (mainCamera.WorldToScreenPoint(transform.position).y < deadZone.y)
            {
                Debug.Log("Object is visible but too much to the down");
            }
            else if (mainCamera.WorldToScreenPoint(transform.position).y > Screen.height - deadZone.y)
            {
                Debug.Log("Object is visible but too much to the up");
            }
            else
            {
                // Debug.Log("Object is visible");
            }
        }
    }
}
