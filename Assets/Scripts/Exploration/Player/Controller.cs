using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Santi.Utils;
using Booble.Interactables;

namespace Booble.Player
{
	public class Controller : Singleton<Controller>
	{
        public bool Arrived { get; private set; }

		[SerializeField] private KeyCode _moveKey;
		[SerializeField] private float _moveSpeed;
        [SerializeField] private Transform _wayPointsParent;

        private WayPointList.Direction _direction; 
        private float _destination;

        private WayPointList _wpList;
		private Camera _cam;
        private Animator _anim;

        private void Awake()
        {
            _wpList = new WayPointList(_wayPointsParent);
            _cam = Camera.main;
            _anim = GetComponent<Animator>();
            _destination = transform.position.x;
        }

        private void Update()
        {
            InputUpdate();
            MoveUpdate();
        }

        private void InputUpdate()
        {
            if (Interactable.BlockActions)
                return;

            if (Input.GetKeyDown(_moveKey))
            {
                SetDestination(_cam.ScreenToWorldPoint(Input.mousePosition).x);
            }
        }

        private void MoveUpdate()
        {
            if (Mathf.Abs(transform.position.x - _destination) < 0.1f || _wpList.IsPlayerInLimit(transform.position.x, _direction))
            {
                _anim.SetBool("IsWalking", false);
                Arrived = true;
                return;
            }

            transform.Translate((_wpList.CurrentWayPoint - transform.position).normalized * _moveSpeed * Time.deltaTime, Space.World);

            if((_wpList.CurrentWayPoint - transform.position).magnitude < 0.1f)
            {
                _wpList.SetNextWayPointAsCurrent(_direction);
            }

            _anim.SetBool("IsWalking", true);
        }

        public void SetDestination(float destination, float stopDistance = 0)
        {
            if(destination > transform.position.x)
            {
                _destination = destination - stopDistance;
            }
            else if(destination < transform.position.x)
            {
                _destination = destination + stopDistance;
            }

            if (_destination > transform.position.x)
            {
                _direction = WayPointList.Direction.Right;
                _wpList.SetCurrentWayPoint(transform.position.x, _direction);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (_destination < transform.position.x)
            {
                _direction = WayPointList.Direction.Left;
                _wpList.SetCurrentWayPoint(transform.position.x, _direction);
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            Arrived = false;
        }

        public void StopMovement()
        {
            _destination = transform.position.x;
        }
    }
}
