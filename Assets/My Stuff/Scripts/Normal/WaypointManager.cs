using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    int nextIndex;
    public GameObject[] waypoints;
    public GameObject NextWaypoint(GameObject current)
    {
        if (current != null)
        {
            // Find array index of given waypoint
            for (int i = 0; i < waypoints.Length; i++)
            {
                // Once found calculate next one
                if (current == waypoints[i])
                {
                    // Modulus operator helps to avoid to go out of bounds
                    // And resets to 0 the index count once we reach the end of the array
                    nextIndex = (i + 1) % waypoints.Length;
                }
            }
        }
        else
        {
            // Default is first index in array
            nextIndex = 0;
        }
        return waypoints[nextIndex];
    }

}
