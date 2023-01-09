using Booble.Flags;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using Booble.CardGame;
using Booble.Managers;
using UnityEngine.UI;

namespace Booble.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Scene objects")]
        [SerializeField] private GameObject _canvas;
        [SerializeField] private GameObject _eventSystem;
        [SerializeField] private GameObject _background;

        [Header("Menu sections")]
        [SerializeField] private RectTransform _mainMenu;
        [SerializeField] private RectTransform _credits;
        [SerializeField] private RectTransform _options;
        [SerializeField] private RectTransform _cardGameMenu;
        [SerializeField] private float _duration;

        [Header("Options")]
        [SerializeField] private Slider _generalMusicSlider;
        [SerializeField] private Slider _backgroundMusicSlider;
        [SerializeField] private Slider _sfxMusicSlider;

        [Header("Backgrounds")]
        [SerializeField] private Image _cardGameMenuBackground;
        [SerializeField] private Sprite _tutorialBackground;
        [SerializeField] private Sprite _citrianoBackground;
        [SerializeField] private Sprite _ppBrosBackground;
        [SerializeField] private Sprite _secretaryBackground;
        [SerializeField] private Sprite _bossBackground;

        private bool _onTween;

        public void PlayButton()
        {
            if (_onTween)
                return;

            FlagManager.Instance.SetFlag(Flag.Reference.Car0);
            SceneLoader.Instance.LoadScene(Scenes.CAR_0);
            MusicManager.Instance.PlayMusic(MusicReference.Lounge);
        }

        #region Card Game

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
            DeckManager.Instance.SetOpponent(Opponent_Name.Tutorial);
            DeckManager.Instance.RemoveExtraCards();
            LoadInterview();
        }

        public void CitrianoCardsButton()
        {
            DeckManager.Instance.SetOpponent(Opponent_Name.Citriano);
            DeckManager.Instance.RemoveExtraCards();
            DeckManager.Instance.AddCitrianoCards();
            LoadInterview();
        }

        public void PingPongBrosCardsButton()
        {
            DeckManager.Instance.SetOpponent(Opponent_Name.PPBros);
            DeckManager.Instance.RemoveExtraCards();
            DeckManager.Instance.AddPPBrosCards();
            LoadInterview();
        }

        public void SecretaryCardsButton()
        {
            DeckManager.Instance.SetOpponent(Opponent_Name.Secretary);
            DeckManager.Instance.RemoveExtraCards();
            DeckManager.Instance.AddSecretaryCards();
            LoadInterview();
        }

        public void BossCardsButton()
        {
            DeckManager.Instance.SetOpponent(CardGame.Opponent_Name.Boss);
            DeckManager.Instance.RemoveExtraCards();
            DeckManager.Instance.AddBossCards();
            LoadInterview();
        }

        private void LoadInterview()
        {
            SceneLoader.Instance.LoadInterviewScene(new List<GameObject>()
            {
                Camera.main.gameObject,
                _canvas,
                _eventSystem,
                _background
            });
        }

        public void SetButtonBackground(string name)
        {
            Sprite sprite = null;
            switch (name)
            {
                case "Tutorial":
                    sprite = _tutorialBackground; break;
                case "Citriano":
                    sprite = _citrianoBackground; break;
                case "PPBros":
                    sprite = _ppBrosBackground; break;
                case "Secretary":
                    sprite = _secretaryBackground; break;
                case "Boss":
                    sprite = _bossBackground; break;

            }
            if (sprite != null)
            {
                _cardGameMenuBackground.sprite = sprite;
                _cardGameMenuBackground.DOFade(1, 0.2f);
            }
        }

        public void ResetButtonBackground()
        {
            _cardGameMenuBackground.DOFade(0, 0.2f);
        }

        #endregion

        public void CreditsOnOffButton()
        {
            if (_onTween)
                return;

            _onTween = true;
            _mainMenu.DOMoveX(_credits.position.x, _duration);
            _credits.DOMoveX(_mainMenu.position.x, _duration).OnComplete(() => _onTween = false);
        }

        public void OptionsOnOffButton()
        {
            if (_onTween)
                return;

            _onTween = true;
            _mainMenu.DOMoveX(_options.position.x, _duration);
            _options.DOMoveX(_mainMenu.position.x, _duration).OnComplete(() => _onTween = false);

            _generalMusicSlider.value = MusicManager.Instance.GetGeneralMusicVolume();
            _backgroundMusicSlider.value = MusicManager.Instance.GetBackgroundMusicVolume();
            _sfxMusicSlider.value = MusicManager.Instance.GetSFXMusicVolume();
        }

        public void QuitButton()
        {
            if (_onTween)
                return;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }
    }
}