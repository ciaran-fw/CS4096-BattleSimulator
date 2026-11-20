using System;
using System.Collections.Generic;
using UnityEngine;

public struct PathRequest
{
    public Vector2 PathStart;
    public Vector2 PathEnd;

    // action stores a method that can be called later.
    // it must accept a List<Node> (the calculated path) and,
    //  a bool (wheter a valid path was found).
    public Action<List<Node>, bool> Callback;

    public PathRequest(Vector2 pathStart, Vector2 pathEnd, Action<List<Node>, bool> callback)
    {
        PathStart = pathStart;
        PathEnd = pathEnd;
        Callback = callback;
    }
}