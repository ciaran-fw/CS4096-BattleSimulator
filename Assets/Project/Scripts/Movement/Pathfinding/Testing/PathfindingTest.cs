using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;

    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform targetPosition;

    [SerializeField] private bool drawPathGizmos = false;
    [SerializeField] private bool drawWaypointGizmos = false;


    private List<Node> path;
    private List<Vector2> waypoints;

    [ContextMenu("Find Path")]
    public void FindPath()
    {
        if (startPosition == null || targetPosition == null)
        {
            Debug.LogError("Error - start position or target position not set..");
            return;
        }

        bool success = pathfinder.FindPath(startPosition.position, targetPosition.position, out path);
        waypoints = PathfindingWaypoints.GenerateWaypoints(path);

        if (success)
        {
            Debug.Log($"Path found containing {path.Count} nodes.");
            Debug.Log($"Waypoints generated containing {waypoints.Count} positions");
        }
        else Debug.Log("Error - could not find path!");
    }

    private void OnDrawGizmos()
    {
        if (drawPathGizmos)
        {
            if (path == null || path.Count == 0) return;

            Gizmos.color = Color.blue;

            foreach (Node node in path) Gizmos.DrawCube(node.WorldPosition, Vector3.one * 0.4f);
        }

        if (drawWaypointGizmos)
        {
            if (waypoints == null || waypoints.Count == 0) return;

            Gizmos.color = Color.cyan;

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Vector3 currentWaypoint = waypoints[i];
                Vector3 nextWaypoint = waypoints[i + 1];

                Gizmos.DrawSphere(currentWaypoint, 0.15f);
                Gizmos.DrawLine(currentWaypoint, nextWaypoint);
            }

            // draw last waypoint position.
            Gizmos.DrawSphere(waypoints[^1], 0.15f);
        }
    }
}
