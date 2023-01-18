using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Santi.Utils;
using Booble.Interactables;
using UnityEngine.EventSystems;

namespace Booble.Player
{
	public class Controller : Singleton<Controller>
	{
        private const float MAX_ERROR = 0.1f;

        public bool Arrived { get; private set; }

		[SerializeField] private KeyCode _moveKey;
		[SerializeField] private float _moveSpeed;
        [SerializeField] private Transform _wayPointsParent;

        private WayPointList.Direction _direction; 
        private float _destination;

        private WayPointList _wpList;
		private Camera _cam;
        private Animator _anim;

        private bool _endInteractionStagger;

        private void Awake()
        {
            Arrived = true;
            _wpList = new WayPointList(_wayPointsParent);
            _cam = Camera.main;
            _anim = GetComponent<Animator>();
            _destination = transform.position.x;
            Interactable.EndInteraction();
        }

        private void Update()
        {
            InputUpdate();
            MoveUpdate();
            AnimUpdate();
        }

        private void InputUpdate()
        {
            if (_endInteractionStagger != Interactable.BlockActions)
            {
                if (Input.GetKeyUp(_moveKey))
                {
                    _endInteractionStagger = Interactable.BlockActions;
                }
                return;
            }

            if(EventSystem.current.IsPointerOverGameObject())
                return;
            
            if(Interactable.InteractionOnGoing)
                return;

            if (Input.GetKeyDown(_moveKey) && Interactable.MouseOverInteractable)
                return;

            if (Input.GetKey(_moveKey))
            {
                SetDestination(_cam.ScreenToWorldPoint(Input.mousePosition).x);
            }
        }

        private void MoveUpdate()
        {
            if(!Arrived && _wpList.IsPlayerInLimit(transform.position.x, _direction))
            {
                _destination = transform.position.x;
            }

            if (Mathf.Abs(transform.position.x - _destination) < MAX_ERROR /*|| _wpList.IsPlayerInLimit(transform.position.x, _direction)*/)
            {
                Arrived = true;
                return;
            }

            transform.Translate((_wpList.CurrentWayPoint - transform.position).normalized * _moveSpeed * Time.deltaTime, Space.World);

            if((_wpList.CurrentWayPoint - transform.position).magnitude < MAX_ERROR)
            {
                _wpList.SetNextWayPointAsCurrent(_direction);
            }
        }

        private void AnimUpdate()
        {
            _anim.SetBool("WalkingLeft", transform.position.x > _destination + MAX_ERROR);
            _anim.SetBool("WalkingRight", transform.position.x < _destination - MAX_ERROR);
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
            }
            else if (_destination < transform.position.x)
            {
                _direction = WayPointList.Direction.Left;
                _wpList.SetCurrentWayPoint(transform.position.x, _direction);
            }

            Arrived = false;
        }

        public void StopMovement()
        {
            _destination = transform.position.x;
        }
    }
}
