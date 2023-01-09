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

        private bool _hide;
        public bool hide => _hide;

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
                _hide = false;
            }
            else
            {
                _pauseMenu.SetActive(true);
            }
        }

        public void OnResumeButtonClick()
        {
            _hide = true;
        }

        public void OnOptionsButtonClick()
        {
            _mainMenu.SetActive(false);
            _optionsMenu.SetActive(true);
        }

        public void OnReturnToMenuButtonClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
            
            return;
            ShowHidePauseMenu();
            SceneLoader.Instance.UnloadInterviewScene();
            if (CardGameManager.Instance == null || CardGameManager.Instance.playingStoryMode) SceneLoader.Instance.LoadMainMenuScene();
        }

        public void OnBackButtonClick()
        {
            _mainMenu.SetActive(true);
            _optionsMenu.SetActive(false);
        }

        public void OnGeneralMusicValueChanged()
        {
            MusicManager.Instance.ChangeGeneralMusicVolume(_generalMusicSlider.value);
        }

        public void OnBackgroundMusicSliderValueChanged()
        {
            MusicManager.Instance.ChangeBackgroundMusicVolume(_backgroundMusicSlider.value);
        }

        public void OnSFXMusicSliderValueChanged()
        {
            MusicManager.Instance.ChangeSFXVolume(_sfxMusicSlider.value);
        }
    }
}