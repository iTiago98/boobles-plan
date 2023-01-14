using System;
using Booble.Flags;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using Booble.CardGame;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using UnityEditor;
using UnityEngine.UI;
using Booble.UI;

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
        [SerializeField] private RectTransform _play;
        [SerializeField] private RectTransform _credits;
        [SerializeField] private RectTransform _options;
        [SerializeField] private RectTransform _cardGameMenu;
        [SerializeField] private float _duration;

        [Header("Play")]
        [SerializeField] private GameObject _continueButton;
        [SerializeField] private GameObject _mainPlayMenu;
        [SerializeField] private GameObject _confirmMenu;

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

        [Header("SceneLoad")]
        [SerializeField] private List<Triplet> _checkPoints;

        private bool _onTween;

        private void Start()
        {
            _continueButton.SetActive(FlagManager.Instance.GetFlag(Flag.Reference.HabemusPartida));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FlagManager.Instance.ResetFlags();
                StartDay1();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                FlagManager.Instance.ResetFlags();
                StartDay2();
            }
        }

        #region Play

        public void ContinueButton()
        {
            if (_onTween)
                return;

            DeckManager.Instance.SetBaseDeck();
            PauseMenu.Instance.InitializeCardMenu();
            DeckManager.Instance.CheckExtraCards();

            int i = 0;
            while (i < _checkPoints.Count && _checkPoints[i].Satisfied)
            {
                i++;
            }

            if (i < _checkPoints.Count)
            {
                SceneLoader.Instance.LoadScene(_checkPoints[i].Scene);
                // MusicManager.Instance.PlayMusic(_checkPoints[i].Music);
            }
            else
            {
                Debug.Log("Completed the game!");
            }
        }

        public void NewGameButton()
        {
            if (_onTween)
                return;

            if (_continueButton.activeSelf)
            {
                _mainPlayMenu.SetActive(false);
                _confirmMenu.SetActive(true);
            }
            else NewGame();
        }

        private void NewGame()
        {
            DeckManager.Instance.SetBaseDeck();
            PauseMenu.Instance.InitializeCardMenu();
            FlagManager.Instance.ResetFlags();
            SceneLoader.Instance.LoadCar0();
        }

        public void ConfirmNewGameButton()
        {
            NewGame();
        }

        public void CancelNewGameButton()
        {
            _mainPlayMenu.SetActive(true);
            _confirmMenu.SetActive(false);
        }

        #endregion

        [ContextMenu("Start Day 0")]
        public void StartDay0()
        {
            DeckManager.Instance.SetBaseDeck();
            PauseMenu.Instance.InitializeCardMenu();
            SceneLoader.Instance.LoadScene(Scenes.CAR_0);
        }

        [ContextMenu("Start Day 1")]
        public void StartDay1()
        {
            FlagManager.Instance.SetFlag(Flag.Reference.Car0);
            StartDay0();
        }

        [ContextMenu("Start Day 2")]
        public void StartDay2()
        {
            // FlagManager.Instance.SetFlag(Flag.Reference.Car1);
            FlagManager.Instance.SetFlag(Flag.Reference.Day1);
            StartDay1();
        }

        [ContextMenu("Start Day 3")]
        public void StartDay3()
        {
            // FlagManager.Instance.SetFlag(Flag.Reference.Car2);
            FlagManager.Instance.SetFlag(Flag.Reference.Day2);
            StartDay2();
        }

        [ContextMenu("Start Day 4")]
        public void StartDay4()
        {
            FlagManager.Instance.SetFlag(Flag.Reference.Car0);
            FlagManager.Instance.SetFlag(Flag.Reference.Day1);
            FlagManager.Instance.SetFlag(Flag.Reference.Day2);
            FlagManager.Instance.SetFlag(Flag.Reference.Day3);
            DeckManager.Instance.SetBaseDeck();
            PauseMenu.Instance.InitializeCardMenu();
            SceneLoader.Instance.LoadScene(Scenes.NELA_OFFICE_DAY_START);
        }

        [ContextMenu("Start Boss Hall 4")]
        public void StartBossHall4()
        {
            FlagManager.Instance.SetFlag(Flag.Reference.Car0);
            FlagManager.Instance.SetFlag(Flag.Reference.Day1);
            FlagManager.Instance.SetFlag(Flag.Reference.Day2);
            FlagManager.Instance.SetFlag(Flag.Reference.Day3);
            FlagManager.Instance.SetFlag(Flag.Reference.BossHall4);
            DeckManager.Instance.SetBaseDeck();
            PauseMenu.Instance.InitializeCardMenu();
            SceneLoader.Instance.LoadScene(Scenes.BOSS_HALL_4);
        }
        
        [ContextMenu("Get Alternate Win Cons")]
        public void GetAlternateWinCons()
        {
            FlagManager.Instance.SetFlag(Flag.Reference.CitrianoVictoriaAlternativa);
            FlagManager.Instance.SetFlag(Flag.Reference.PPBVictoriaAlternativa);
            FlagManager.Instance.SetFlag(Flag.Reference.SecretaryVictoriaAlternativa);
        }

        [ContextMenu("Add Gomu Gomu No")]
        public void AddNewCard()
        {
            DeckManager.Instance.AddGomuGomuNo();
        }

        #region Card Game

        public void TutorialCardsButton()
        {
            DeckManager.Instance.InitializeAuxDeck(Opponent_Name.Tutorial);
            LoadInterview();
        }

        public void CitrianoCardsButton()
        {
            DeckManager.Instance.InitializeAuxDeck(Opponent_Name.Citriano);
            LoadInterview();
        }

        public void PingPongBrosCardsButton()
        {
            DeckManager.Instance.InitializeAuxDeck(Opponent_Name.PPBros);
            LoadInterview();
        }

        public void SecretaryCardsButton()
        {
            DeckManager.Instance.InitializeAuxDeck(Opponent_Name.Secretary);
            LoadInterview();
        }

        public void BossCardsButton()
        {
            DeckManager.Instance.InitializeAuxDeck(Opponent_Name.Boss);
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

        #region Main

        public void PlayOnOffButton()
        {
            if (_onTween)
                return;

            _onTween = true;
            _mainMenu.DOMoveX(_play.position.x, _duration);
            _play.DOMoveX(_mainMenu.position.x, _duration).OnComplete(() => _onTween = false);
        }

        public void CardGameOnOffButton()
        {
            if (_onTween)
                return;

            _onTween = true;
            _mainMenu.DOMoveX(_cardGameMenu.position.x, _duration);
            _cardGameMenu.DOMoveX(_mainMenu.position.x, _duration).OnComplete(() => _onTween = false);
        }

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

        #endregion
    }

    [System.Serializable]
    public class Triplet
    {
        public bool Satisfied
        {
            get
            {
                return FlagManager.Instance.GetFlag(FlagRef);
            }

            set
            {
                FlagManager.Instance.SetFlag(FlagRef, value);
            }
        }

        [field: SerializeField] public string Scene { get; private set; }
        [field: SerializeField] public Flag.Reference FlagRef { get; private set; }
        [field: SerializeField] public MusicReference Music { get; private set; }
    }
}