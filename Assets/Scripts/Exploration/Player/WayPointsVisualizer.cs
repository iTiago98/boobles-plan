using UnityEngine;

namespace Booble.Player
{
	public class WayPointsVisualizer : MonoBehaviour
	{
        [SerializeField] private bool _visualizeWayPoints;

        private void OnDrawGizmos()
        {
            if (!_visualizeWayPoints)
                return;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);
                Gizmos.DrawIcon(t.position, "Waypoint");
            }
        }
    }
}