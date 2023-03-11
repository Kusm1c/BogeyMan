using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabable
{
    Transform transform { get; }
    public bool IsThrowable();
    public float GetThrowSpeed();
    public float GetThrowDuration();
    public Collider GetCollider();
    public void OnGrab();

    public void OnRelease();

    public void OnImpact();

    public void OnThrow();
}
