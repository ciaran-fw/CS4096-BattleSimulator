using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    [Header("Pathfinder")]
    [SerializeField] private Pathfinder pathfinder;

    // stores path requests waiting to be processed.
    private Queue<PathRequest> pathRequestQueue = new();

    // the request that is currently being processed.
    private PathRequest currentPathRequest;

    private bool isProcessingPath;


    private void Update()
    {
        // each frame update will check the queue and process the request.
        // still running on the main thread we can improve later to have requests,
        // find paths together at same time.
        TryProcessNext();
    }

    public void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<List<Node>, bool> callback)
    {
        // create new request and add it to the queue.
        PathRequest newRequest = new(pathStart, pathEnd, callback);
        pathRequestQueue.Enqueue(newRequest);
    }

    private void TryProcessNext()
    {
        // only process a request if we are not already processing one.
        if (isProcessingPath || pathRequestQueue.Count == 0) return;

        // get the next request and calculate its path using the a* find path algorithm.
        currentPathRequest = pathRequestQueue.Dequeue();
        isProcessingPath = true;

        List<Node> path = pathfinder.FindPath(
            currentPathRequest.PathStart,
            currentPathRequest.PathEnd
        );

        // check whether a valid path was found.
        bool success = path.Count > 0;

        // return the result to the unit that requested it,
        // by calling the callback given.
        currentPathRequest.Callback?.Invoke(path, success);

        // we are now finished processing request and can move onto next one.
        isProcessingPath = false;
    }
}
