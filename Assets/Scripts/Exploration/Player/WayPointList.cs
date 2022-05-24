using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Player
{
	public class WayPointList
	{
		public enum Direction { Left = -1, Right = 1 }

		public Vector3 CurrentWayPoint => _wayPoints[CurrentIndex].position;

		private List<Transform> _wayPoints;
		private int _currentIndex;
		private int CurrentIndex
        {
			get { return _currentIndex; }
            set
            {
				if (value < 0)
				{
					value = 0;
				}
				else if (value >= _wayPoints.Count)
				{
					value = _wayPoints.Count - 1;
				}

				_currentIndex = value;
			}
        }

		public WayPointList(Transform parentTransform)
		{
			_wayPoints = new List<Transform>();

			foreach(Transform t in parentTransform)
            {
				int lastLower = _wayPoints.FindLastIndex(wp => wp.position.x < t.position.x);
				_wayPoints.Insert(lastLower+1, t);
            }
		}

		public void SetCurrentWayPoint(float playerX, Direction direction)
        {
			switch(direction)
            {
				case Direction.Left:
					CurrentIndex = _wayPoints.FindLastIndex(wp => wp.position.x < playerX);
					break;
				case Direction.Right:
					CurrentIndex = _wayPoints.FindIndex(wp => wp.position.x > playerX);
					break;
            }
        }

		public void SetNextWayPointAsCurrent(Direction direction)
        {
			CurrentIndex += (int)direction;
        }

		public bool IsPlayerInLimit(float playerX, Direction direction)
        {
			switch(direction)
            {
				case Direction.Left:
					return Mathf.Abs(playerX - _wayPoints[0].position.x) < 0.1f;
				case Direction.Right:
					return Mathf.Abs(playerX - _wayPoints[_wayPoints.Count-1].position.x) < 0.1f;
			}

			throw new System.Exception("Unexpected trace in IsPlayerLimit from WayPointList");
        }
	}
}
