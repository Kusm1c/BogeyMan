using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grab : MonoBehaviour
{
    [SerializeField] Player player = null;
    [SerializeField] private Transform grabbedObjectPivot = null;
    [SerializeField] private ThrownObjectParent thrownObjectParentPrefab = null;
    [SerializeField] private Transform thrownObjectParentSpawnPos = null;
    private IGrabable grabbedObject = null;
    private List<IGrabable> grabableObjects = new List<IGrabable>();
    private IGrabable nearestGrabable = null;
    Rigidbody rb;

    public void GrabInput()
	{
        if (grabbedObject == null)
		{
            if (grabableObjects.Count > 0)
            {
                GrabObject(nearestGrabable);
            }
        }
		else
		{
            if (grabbedObject.IsThrowable() == true)
            {
                ThrowObject(grabbedObject);
            }
            else
            {
                Release(grabbedObject);
            }
        }
	}

    private void GrabObject(IGrabable objectToGrab)
	{
        grabbedObject = objectToGrab;
        player.playerState.isGrabbing = true;
        grabbedObject.OnGrab(player);
        if(player.playerState.isGrabbingSummoner == false)
        {
            grabbedObject.transform.parent = grabbedObjectPivot;
            grabbedObject.transform.position = grabbedObjectPivot.position;
            rb = grabbedObject.transform.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            Enemies.Enemy enemy = grabbedObject.transform.GetComponent<Enemies.Enemy>();
            if (enemy != null)
			{
                StartCoroutine(WaitForRelease(player.settings.minMaxTimeGrabbingSwarmer));
			}
        }
        objectToGrab.GetCollider().enabled = false;
        grabableObjects.Remove(grabbedObject);
    }

    private IEnumerator WaitForRelease(Vector2 minMaxGrabTime)
	{
        float randomTime = UnityEngine.Random.Range(minMaxGrabTime.x, minMaxGrabTime.y);
        yield return new WaitForSeconds(randomTime);
        Release(grabbedObject);
	}

    private void ThrowObject(IGrabable objectToThrow)
    {
        player.playerState.isGrabbing = false;
        
        grabbedObject = null;
        objectToThrow.transform.parent = null;
        Vector3 direction = new Vector3(player.playerController.aimDirection.x, 0 ,player.playerController.aimDirection.y);
        
        objectToThrow.OnThrow();
        direction.y = 0;
        direction = direction.normalized;
        ThrownObjectParent thrownObjectParentInstance = Instantiate(thrownObjectParentPrefab.gameObject, thrownObjectParentSpawnPos.position, Quaternion.identity).GetComponent<ThrownObjectParent>();
        thrownObjectParentInstance.Throw(objectToThrow.GetThrowSpeed(), objectToThrow.GetThrowDuration(), player, direction, objectToThrow.GetThrowDamage(), objectToThrow);
    }

    public void Release()
	{
        if (grabbedObject == null)
            return;
        Release(grabbedObject);
	}

    private void Release(IGrabable objectToRelease)
    {
        player.playerState.isGrabbing = false;
        grabbedObject = null;
        StopAllCoroutines();
        if (player.playerState.isGrabbingSummoner == true)
        {
            objectToRelease.GetCollider().enabled = true;
            objectToRelease.OnRelease();
        }
        else
        {
            rb.velocity = Vector3.zero;
            ResetGrabbedObject(objectToRelease);
        }

    }

    public static void ResetGrabbedObject(IGrabable objectToRelease)
    {
        objectToRelease.transform.GetComponent<Rigidbody>().isKinematic = false;
        objectToRelease.transform.parent = null;
        objectToRelease.GetCollider().enabled = true;
        objectToRelease.OnRelease();
    }

    private void FixedUpdate()
    {
        if (grabableObjects.Count > 0)
        {
            nearestGrabable = grabableObjects[0];

            if (grabableObjects.Count > 1)
            {
                foreach (IGrabable grabable in grabableObjects)
                {
                    if ((grabable.transform.position - transform.position).magnitude
                        < (nearestGrabable.transform.position - transform.position).magnitude)
                    {
                        nearestGrabable = grabable;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IGrabable grabable = other.gameObject.GetComponent<IGrabable>();
        if (grabable != null && grabable != grabbedObject)
		{
            grabableObjects.Add(grabable);
        }
    }

	private void OnTriggerExit(Collider other)
	{
        IGrabable grabable = other.gameObject.GetComponent<IGrabable>();
        if (grabable != null)
        {
            grabableObjects.Remove(grabable);
        }
    }
}
