using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabable
{
    Transform transform { get; }

    public void Grab();

    public void Release();

    public void Impact();

    public void Throw();
}
