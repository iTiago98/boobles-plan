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
		[SerializeField] private bool _withinAnimation;
		
		public override void Execute()
        {
			_interactable.StartInteraction(_withinAnimation);
        }
	}
}
