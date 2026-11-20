using System;
using System.Collections.Generic;
using UnityEngine;

public struct PathRequest
{
    public Vector2 PathStart;
    public Vector2 PathEnd;

    // action stores a method that can be called later.
    // it must accept a List<Vector2> (the calculated path waypoints) and,
    //  a bool (wheter a valid path was found).
    public Action<List<Vector2>, bool> Callback;

    public PathRequest(Vector2 pathStart, Vector2 pathEnd, Action<List<Vector2>, bool> callback)
    {
        PathStart = pathStart;
        PathEnd = pathEnd;
        Callback = callback;
    }
}