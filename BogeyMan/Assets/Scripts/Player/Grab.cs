using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grab : MonoBehaviour
{
    private GameObject grabbedObject;


    private void Update()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            other.gameObject.transform.position = transform.position;
            grabbedObject = other.gameObject;
        }
    }

    private void OnDisable()
    {
        if (grabbedObject == null) return;
        grabbedObject.GetComponent<NavMeshAgent>().enabled = true;
            
        grabbedObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);

        grabbedObject = null;
    }
}
