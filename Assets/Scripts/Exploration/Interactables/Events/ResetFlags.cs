using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Booble.Interactables.Events
{
	public class ResetFlags : ThrowDialogue
	{
        [SerializeField] private Image _fadeScreen;
        [SerializeField] private float _fadeDuration;

        protected override void OnDialogueEnd()
        {
            Flags.FlagManager.Instance.ResetFlags();
            _fadeScreen.DOFade(1, _fadeDuration)
                .OnComplete(() =>
                {
                    Interactable.EndInteraction();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                });
        }
    }
}
