using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Santi.Utils;

namespace Booble.MyCamera
{
	public class FollowTarget : MonoBehaviour
	{
		[SerializeField] private Transform _target;
		[SerializeField] private float _leftLimit;
		[SerializeField] private float _rightLimit;

        [SerializeField] private float _lerpFactor;

        private void Awake()
        {
            float newX = Mathf.Clamp(_target.position.x, _leftLimit, _rightLimit);
            transform.SetXPosition(newX);
        }

        private void Update()
        {
            float newX = Mathf.Lerp(transform.position.x, _target.position.x, _lerpFactor);
            newX = Mathf.Clamp(newX, _leftLimit, _rightLimit);
            transform.SetXPosition(newX);
        }

        [ContextMenu("Set Left Limit")]
        public void SetLeftLimit()
        {
            _leftLimit = transform.position.x;
        }
        
        [ContextMenu("Set Right Limit")]
        public void SetRightLimit()
        {
            _rightLimit = transform.position.x;
        }
    }
}
