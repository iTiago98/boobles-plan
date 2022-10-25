using Booble.Flags;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Booble.MainMenu
{
	public class MainMenu : MonoBehaviour
	{
		[SerializeField] private RectTransform _mainMenu;
		[SerializeField] private RectTransform _credits;
		[SerializeField] private float _duration;

		private bool _onTween;

    	public void PlayButton()
		{
			if(_onTween)
				return;
			
			FlagManager.Instance.SetFlag(Flag.Reference.Car0);
			SceneLoader.Instance.LoadScene("Car");
			MusicManager.Instance.PlayMusic(MusicReference.Lounge);
		}

		public void CreditsOnOffButton()
		{
			if(_onTween)
				return;

			_onTween = true;
			_mainMenu.DOMoveX(_credits.position.x, _duration);
			_credits.DOMoveX(_mainMenu.position.x, _duration).OnComplete(() => _onTween = false);
		}

		public void QuitButton()
		{
			if(_onTween)
				return;

			Application.Quit();
		}
	}
}