using Booble.Flags;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;

namespace Booble.MainMenu
{
	public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _canvas;
        [SerializeField] private GameObject _eventSystem;

		[SerializeField] private RectTransform _mainMenu;
		[SerializeField] private RectTransform _credits;
        [SerializeField] private RectTransform _cardGameMenu;
		[SerializeField] private float _duration;

		private bool _onTween;

    	public void PlayButton()
		{
			if(_onTween)
				return;
			
			FlagManager.Instance.SetFlag(Flag.Reference.Car0);
			SceneLoader.Instance.LoadScene(Scenes.CAR_0);
			MusicManager.Instance.PlayMusic(MusicReference.Lounge);
		}

		public void CardGameButton()
        {
			if (_onTween)
				return;

			_onTween = true;
			_mainMenu.DOMoveX(_cardGameMenu.position.x, _duration);
			_cardGameMenu.DOMoveX(_mainMenu.position.x, _duration).OnComplete(() => _onTween = false);
		}

		public void TutorialCardsButton()
        {
			DeckManager.Instance.SetOpponentCards(CardGame.Opponent_Name.Tutorial);
			LoadInterview();
        }

		public void CitrianoCardsButton()
        {
			DeckManager.Instance.SetOpponentCards(CardGame.Opponent_Name.Citriano);
			LoadInterview();
		}

		public void PingPongBrosCardsButton()
        {
			DeckManager.Instance.SetOpponentCards(CardGame.Opponent_Name.PingPongBros);
			LoadInterview();
		}

		public void SecretaryCardsButton()
        {
			DeckManager.Instance.SetOpponentCards(CardGame.Opponent_Name.Secretary);
			LoadInterview();
        }

		public void LoadInterview()
        {
			SceneLoader.Instance.LoadInterviewScene(new List<GameObject>()
			{
				Camera.main.gameObject,
				_canvas,
				_eventSystem
			});
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

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}