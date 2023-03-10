using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grab : MonoBehaviour
{
    [SerializeField] Player player = null;

    private IGrabable grabbedObject = null;
    private List<IGrabable> grabableObjects = new List<IGrabable>();
    private IGrabable nearestGrabable = null;
    private float initialMass = 0;
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
            StartCoroutine(ThrowObject(grabbedObject));
            if (grabableObjects.Count > 0)
			{
                GrabObject(nearestGrabable);
            }
        }
	}

    private void GrabObject(IGrabable objectToGrab)
	{
        grabbedObject = objectToGrab;
        grabbedObject.transform.position = transform.position;
        grabbedObject.transform.parent = transform;
        rb = grabbedObject.transform.GetComponent<Rigidbody>();
        rb.Sleep();
        grabbedObject.Grab();
        grabableObjects.Remove(grabbedObject);
        initialMass = rb.mass;
        rb.mass = 0;
    }

    private IEnumerator ThrowObject(IGrabable objectToThrow)
    {
        rb.mass = initialMass;
        grabbedObject = null;
        objectToThrow.transform.parent = null;
        rb.WakeUp();
        Vector3 direction = (objectToThrow.transform.position - player.transform.position).normalized;
        
        objectToThrow.Throw();
        direction.y = 0;
        //rb.velocity = direction * player.settings.throwSpeed;
        rb.AddForce(direction * player.settings.throwSpeed);

        yield return new WaitForSeconds(player.settings.throwDuration);
        rb.velocity = Vector3.zero;
        objectToThrow.Impact();
    }

    private void Update()
    {
        if (grabableObjects.Count > 0)
        {
            nearestGrabable = grabableObjects[0];
            foreach(IGrabable grabable in grabableObjects)
			{
                if ((grabable.transform.position - transform.position).magnitude
                    < (nearestGrabable.transform.position - transform.position).magnitude)
				{
                    nearestGrabable = grabable;
				}
			}
        }

        if (grabbedObject != null)
		{
            grabbedObject.transform.position = transform.position;
		}
    }

    private void OnTriggerEnter(Collider other)
    {
        IGrabable grabable = other.gameObject.GetComponent<IGrabable>();
        if (grabable != null)
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
