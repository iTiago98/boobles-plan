using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Booble.Interactables.Events
{
	public class StartInteraction : DialogueEvent
	{
		[SerializeField] private Interactable _interactable;
		
		public override void Execute()
        {
			_interactable.StartInteraction();
        }
	}
}
