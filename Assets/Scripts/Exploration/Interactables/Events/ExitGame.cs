using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Booble.Interactables.Events
{
	public class ExitGame : MonoBehaviour
	{
		[SerializeField] private Image _fadeScreen;
		[SerializeField] private float _duration;

		public void StartInteraction()
        {
			_fadeScreen.DOFade(1, _duration)
				.OnComplete(() => Application.Quit());
        }
	}
}
