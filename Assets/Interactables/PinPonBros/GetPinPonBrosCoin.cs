using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Booble.Player;
using Booble.Interactables.Dialogues;
using static Booble.Interactables.Interactable;

namespace Booble.Interactables.Events.PinPonBros
{
	public class GetPinPonBrosCoin : MonoBehaviour
	{
		[SerializeField] private Transform _playerDestination;
		[SerializeField] private Animator _playerAnimator;
		[SerializeField] private Dialogue _coinFoundDialogue;
		[SerializeField] private List<AnimatorIdentifier> _animatorIdentifiers;
		
		private DialogueManager _diagManager;

		protected virtual void Awake()
        {
            _diagManager = DialogueManager.Instance;
        }

    	public void StartInteraction()
		{
			Controller.Instance.SetDestination(_playerDestination.position.x);
			StartCoroutine(WaitForArrival());
		}

		private IEnumerator WaitForArrival()
		{
			while(!Controller.Instance.Arrived)
			{
				yield return null;
			}

			_playerAnimator.SetTrigger("Interact");
			ThrowDialogue();
		}
		
        private void ThrowDialogue()
        {
            _diagManager.StartDialogue(_coinFoundDialogue, _animatorIdentifiers);
            _diagManager.OnEndDialogue.RemoveAllListeners();
            _diagManager.OnEndDialogue.AddListener(() => Interactable.EndInteraction());
        }
	}
}