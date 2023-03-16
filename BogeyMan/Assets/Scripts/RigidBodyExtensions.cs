using UnityEngine;

public static class RigidBodyExtensions
{
    public static void Disable(this Rigidbody rigidbody)
    {
        rigidbody.isKinematic = true;
        //rigidbody.detectCollisions = false;
    }
    
    public static void Enable(this Rigidbody rigidbody)
    {
        rigidbody.isKinematic = false;
        //rigidbody.detectCollisions = true;
    }
}