// PathWaypoint.cs - Simple waypoint marker for path visualization
using UnityEngine;

public class PathWaypoint : MonoBehaviour
{
    public Color waypointColor = Color.yellow;
    public float waypointSize = 0.5f;
    
    void OnDrawGizmos()
    {
        // Draw a sphere at waypoint position
        Gizmos.color = waypointColor;
        Gizmos.DrawSphere(transform.position, waypointSize);
        
        // Draw line to next waypoint if exists
        if (transform.parent != null)
        {
            int currentIndex = transform.GetSiblingIndex();
            if (currentIndex < transform.parent.childCount - 1)
            {
                Transform nextWaypoint = transform.parent.GetChild(currentIndex + 1);
                Gizmos.DrawLine(transform.position, nextWaypoint.position);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw larger sphere when selected
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, waypointSize * 1.5f);
    }
}