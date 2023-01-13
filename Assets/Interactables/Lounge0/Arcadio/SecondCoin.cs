using Booble.Flags;
using Booble.Player;
using UnityEngine;

namespace Booble.Interactables.Arcadio
{
	public class SecondCoin : MonoBehaviour
	{
        [SerializeField] private Flag.Reference _arcadioWaiting;
        [SerializeField] private float _threshold;
        [SerializeField] private Animator _anim;

        private bool _waiting;

        private void Start()
        {
            FlagManager.Instance.SetFlag(_arcadioWaiting, false);
            _waiting = false;
        }

        private void Update()
        {
            if (!_waiting)
                return;
            
            if(Controller.Instance.transform.position.x < _threshold)
            {
                _waiting = false;
                FlagManager.Instance.SetFlag(_arcadioWaiting, false);
                _anim.SetTrigger("Wait");
            }
        }

        public void SetWaiting()
        {
            _waiting = true;
            FlagManager.Instance.SetFlag(_arcadioWaiting);
        }
    }
}