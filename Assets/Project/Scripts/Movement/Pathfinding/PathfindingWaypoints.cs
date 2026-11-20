using System.Collections.Generic;
using UnityEngine;

public class PathfindingWaypoints
{
    public static List<Vector2> GenerateWaypoints(List<Node> path)
    {
        // this will be a list of world positions where the direction changes,
        // along the path got from a*. we are simplifying the path into the,
        // neccasary points the unit must visit along the path.
        List<Vector2> waypoints = new();

        if (path == null || path.Count == 0) return waypoints;

        // always include the first node in the path.
        waypoints.Add(path[0].WorldPosition);

        // go through all the nodes except first and last.
        for (int i = 1; i < path.Count - 1; i++)
        {
            // calculate direction between the previous node and current node.
            Vector2Int previousDirection = new(
                path[i].GridX - path[i - 1].GridX,
                path[i].GridY - path[i - 1].GridY
            );

            // calculate the direction between the current node and next node.
            Vector2Int nextDirection = new(
                path[i + 1].GridX - path[i].GridX,
                path[i + 1].GridY - path[i].GridY
            );

            // if the direction changes, we have reached a turning point.
            if (previousDirection != nextDirection)
            {
                waypoints.Add(path[i].WorldPosition);
            }
        }

        // always include the last node in the path.
        waypoints.Add(path[^1].WorldPosition);

        return waypoints;
    }
}
