using Booble.Flags;
using Booble.Player;
using UnityEngine;

namespace Booble.Interactables.Arcadio
{
	public class SecondCoin : MonoBehaviour
	{
        [SerializeField] private Flag.Reference _arcadioWaiting;
        [SerializeField] private Continues _arcCont;
        [SerializeField] private float _threshold;
        [SerializeField] private Animator _anim;

        private bool _waiting;

        private void Start()
        {
            _waiting = FlagManager.Instance.GetFlag(_arcadioWaiting);
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
                _arcCont.UpdateContinueDialogue();
            }
        }

        public void CheckIfSecondCoin()
        {
            if (_arcCont.CoinCount == 2)
            {
                _waiting = true;
                FlagManager.Instance.SetFlag(_arcadioWaiting);
            }
        }
	}
}