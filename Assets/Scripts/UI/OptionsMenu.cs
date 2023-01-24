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
        [SerializeField] private Slider _textSpeedSlider;

        [SerializeField] private float _characterDelayMin;
        [SerializeField] private float _characterDelayMax;

        public void SetSliderValue()
        {
            _generalMusicSlider.value = MusicManager.Instance.GetGeneralMusicVolume();
            _backgroundMusicSlider.value = MusicManager.Instance.GetBackgroundMusicVolume();
            _sfxMusicSlider.value = MusicManager.Instance.GetSFXMusicVolume();
            _textSpeedSlider.value = CharacterDelayValueToSlider(PlayerConfig.CharacterDelay.Value);
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
            float characterDelay = CharacterDelaySliderToValue(value);

            if (DialogueManager.Instance != null)
                DialogueManager.Instance.ChangeCharacterDelay(characterDelay);
            else
                PlayerConfig.CharacterDelay.SetValue(characterDelay);
        }

        private float CharacterDelaySliderToValue(float value)
        {
            float characterDelay;
            if (value == 0)
            {
                characterDelay = _characterDelayMax;
            }
            else
            {
                characterDelay = _characterDelayMin / value;
                if (characterDelay > _characterDelayMax) characterDelay = _characterDelayMax;
            }
            return characterDelay;
        }

        private float CharacterDelayValueToSlider(float value)
        {
            float sliderValue;
            if (value == 0)
            {
                sliderValue = 0;
            }
            else
            {
                sliderValue = _characterDelayMin / value;
                if (sliderValue < 0) sliderValue = 0;
                else if (sliderValue > 1) sliderValue = 1;
            }
            return sliderValue;
        }
    }
}