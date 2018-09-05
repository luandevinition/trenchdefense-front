using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class NavigationVertex
{
    public Guid InstanceId
    {
        get { return _instanceID; }
    }

    public Vector2 Point
    {
        get { return _point; }
    }

    private readonly Guid _instanceID;

    private readonly Vector2 _point;

    public NavigationVertex(Vector2 point)
    {
        _instanceID = Guid.NewGuid();
        _point = point;
    }
}
