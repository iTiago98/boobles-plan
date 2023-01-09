using Booble.CardGame.Managers;
using Booble.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }

        [SerializeField] private GameObject _pauseMenu;

        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _optionsMenu;

        [SerializeField] private Slider _generalMusicSlider;
        [SerializeField] private Slider _backgroundMusicSlider;
        [SerializeField] private Slider _sfxMusicSlider;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ShowHidePauseMenu()
        {
            if (_pauseMenu.activeSelf)
            {
                OnBackButtonClick();
                _pauseMenu.SetActive(false);
            }
            else
            {
                _pauseMenu.SetActive(true);
            }
        }

        public void OnResumeButtonClick()
        {
            GameManager.Instance.ResumeGame();
        }

        public void OnOptionsButtonClick()
        {
            _mainMenu.SetActive(false);
            _optionsMenu.SetActive(true);

            _generalMusicSlider.value = MusicManager.Instance.GetGeneralMusicVolume();
            _backgroundMusicSlider.value = MusicManager.Instance.GetBackgroundMusicVolume();
            _sfxMusicSlider.value = MusicManager.Instance.GetSFXMusicVolume();
        }

        public void OnReturnToMenuButtonClick()
        {
            GameManager.Instance.ResumeGame();

            if (SceneLoader.Instance.InInterview)
            {
                SceneLoader.Instance.UnloadInterviewScene();
                if (CardGameManager.Instance.playingStoryMode) SceneLoader.Instance.LoadMainMenuScene();
            }
            else if (SceneLoader.Instance.InExploration) SceneLoader.Instance.LoadMainMenuScene();
        }

        public void OnBackButtonClick()
        {
            _mainMenu.SetActive(true);
            _optionsMenu.SetActive(false);
        }

        public void OnGeneralMusicValueChanged(System.Single value)
        {
            MusicManager.Instance.ChangeGeneralMusicVolume(value);
        }

        public void OnBackgroundMusicSliderValueChanged(System.Single value)
        {
            MusicManager.Instance.ChangeBackgroundMusicVolume(value);
        }

        public void OnSFXMusicSliderValueChanged(System.Single value)
        {
            MusicManager.Instance.ChangeSFXVolume(value);
        }
    }
}