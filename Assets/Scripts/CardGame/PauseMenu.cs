using Booble;
using CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseMenu;

        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _optionsMenu;

        [SerializeField] private Slider _generalMusicSlider;
        [SerializeField] private Slider _backgroundMusicSlider;
        [SerializeField] private Slider _sfxMusicSlider;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape)) ShowHidePauseMenu();
        }

        private void ShowHidePauseMenu()
        {
            if (_pauseMenu.activeSelf)
            {
                OnBackButtonClick();
                _pauseMenu.SetActive(false);
                CardGameManager.Instance.ResumeGame();
            }
            else
            {
                _pauseMenu.SetActive(true);
                CardGameManager.Instance.PauseGame();
            }
        }

        public void OnResumeButtonClick()
        {
            ShowHidePauseMenu();
        }

        public void OnOptionsButtonClick()
        {
            _mainMenu.SetActive(false);
            _optionsMenu.SetActive(true);
        }

        public void OnReturnToMenuButtonClick()
        {
            SceneLoader.Instance.UnloadInterviewScene();
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