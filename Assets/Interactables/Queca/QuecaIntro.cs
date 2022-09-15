using UnityEngine;
using Booble.Interactables.Events;

namespace Booble.Interactables.Queca
{
	public class QuecaIntro : MonoBehaviour
	{
    	[SerializeField] private ThrowDialogue _intro;

		private void Start()
		{
			Interactable.ManualInteractionActivation();
			Invoke(nameof(DelayedStart), 1f);
		}

		private void DelayedStart()
		{
			_intro.Execute();
		}
	}
}