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

    private static PathRequestManager instance;

    public static PathRequestManager Instance => instance;

    private void Awake()
    {
        // make this the shared path request manager.
        if (instance != null && instance != this) return;

        instance = this;

        // make sure that pathfinder is set.
        if (pathfinder == null) Debug.LogError("Error - pathfinder not found!");
    }


    private void Update()
    {
        // each frame update will check the queue and process the request.
        // still running on the main thread we can improve later to have requests,
        // find paths together at same time.
        TryProcessNext();
    }

    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<List<Vector2>, bool> callback)
    {
        if (instance == null)
        {
            Debug.LogError("Error - instance does not exist, so cant request a path!");
            return;
        }

        // create new request and add it to the queue.
        PathRequest newRequest = new(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
    }

    private void TryProcessNext()
    {
        // only process a request if we are not already processing one.
        if (isProcessingPath || pathRequestQueue.Count == 0) return;

        // get the next request and calculate its path using the a* find path algorithm.
        currentPathRequest = pathRequestQueue.Dequeue();
        isProcessingPath = true;

        bool success = pathfinder.FindPath(
            currentPathRequest.PathStart,
            currentPathRequest.PathEnd,
            out List<Node> path
        );

        // handle the completed request.
        FinishedProcessingPath(path, success);
    }

    private void FinishedProcessingPath(List<Node> path, bool success)
    {
        // convert the node path into simplified waypoint positions.
        List<Vector2> waypoints = PathfindingWaypoints.GenerateWaypoints(path);

        // return the result to the unit that requested it.
        currentPathRequest.Callback?.Invoke(waypoints, success);

        // we are now finished processing request and can move onto next one.
        isProcessingPath = false;
    }
}
