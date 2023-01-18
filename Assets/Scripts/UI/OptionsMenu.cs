using Booble.Interactables.Dialogues;
using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private Slider _generalMusicSlider;
        [SerializeField] private Slider _backgroundMusicSlider;
        [SerializeField] private Slider _sfxMusicSlider;

        public void SetSliderValue()
        {
            _generalMusicSlider.value = MusicManager.Instance.GetGeneralMusicVolume();
            _backgroundMusicSlider.value = MusicManager.Instance.GetBackgroundMusicVolume();
            _sfxMusicSlider.value = MusicManager.Instance.GetSFXMusicVolume();
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

        public void OnTextSpeedSliderValueChanged(System.Single value)
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.ChangeTextSpeed(value);
            else
                PlayerConfig.SetCharacterDelay(value);
        }
    }
}