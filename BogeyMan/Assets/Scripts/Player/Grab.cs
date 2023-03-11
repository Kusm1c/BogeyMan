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
            
            if (grabableObjects.Count > 0)
			{
                GrabObject(nearestGrabable);
            }
        }
	}

    private void GrabObject(IGrabable objectToGrab)
	{
        player.playerState.isGrabbing = true;
        grabbedObject = objectToGrab;
        grabbedObject.transform.parent = grabbedObjectPivot;
        grabbedObject.transform.position = grabbedObjectPivot.position;
        rb = grabbedObject.transform.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        objectToGrab.GetCollider().enabled = false;
        grabbedObject.OnGrab();
        grabableObjects.Remove(grabbedObject);
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
        ThrownObjectParent trownObjectParentInstance = Instantiate(thrownObjectParentPrefab.gameObject, thrownObjectParentSpawnPos.position, Quaternion.identity).GetComponent<ThrownObjectParent>();
        trownObjectParentInstance.Throw(objectToThrow.GetThrowSpeed(), objectToThrow.GetThrowDuration(), player, direction, player.settings.throwDamage, objectToThrow);
        /*
        //rb.velocity = direction * player.settings.throwSpeed;
        rb.AddForce(direction * player.settings.throwSpeed);

        yield return new WaitForSeconds(player.settings.throwDuration);

        //rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        objectToThrow.Impact();*/
    }

    private void Release(IGrabable objectToRelease)
    {
        player.playerState.isGrabbing = false;
        rb.velocity = Vector3.zero;
        grabbedObject = null;
        ResetGrabbedObject(objectToRelease);
    }
    public static void ResetGrabbedObject(IGrabable objectToRelease)
    {
        objectToRelease.transform.GetComponent<Rigidbody>().isKinematic = false;
        objectToRelease.GetCollider().enabled = true;
        objectToRelease.transform.parent = null;
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
        /*
        if (grabbedObject != null)
		{
            grabbedObject.transform.position = grabbedObjectPivot.position;
		}*/
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
