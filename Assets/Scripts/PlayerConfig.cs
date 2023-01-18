using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble
{
    public static class PlayerConfig
    {
        private static float MasterVolume;
        private static float BGMVolume;
        private static float SFXVolume;

        private static float CharacterDelay;

        private static bool ShowCluesAlerts;


        private const float MasterBusVolumeDefaultValue = 1;
        private const float BGMBusVolumeDefaultValue = 1;
        private const float SFXBusVolumeDefaultValue = 1;
        private const float CharacterDelayDefaultValue = 0.1f;

        private const string MasterBusVolumeKey = "MasterBusVolume";
        private const string BGMBusVolumeKey = "BGMBusVolume";
        private const string SFXBusVolumeKey = "SFXBusVolume";
        private const string CharacterDelayValueKey = "CharacterDelayValue";

        public static void InitializeValues()
        {
            if (PlayerPrefs.HasKey(MasterBusVolumeKey))
            {
                MasterVolume = PlayerPrefs.GetFloat(MasterBusVolumeKey);
            }
            else
            {
                MasterVolume = MasterBusVolumeDefaultValue;
            }

            if (PlayerPrefs.HasKey(BGMBusVolumeKey))
            {
                BGMVolume = PlayerPrefs.GetFloat(BGMBusVolumeKey);
            }
            else
            {
                BGMVolume = BGMBusVolumeDefaultValue;
            }

            if (PlayerPrefs.HasKey(SFXBusVolumeKey))
            {
                SFXVolume = PlayerPrefs.GetFloat(SFXBusVolumeKey);
            }
            else
            {
                SFXVolume = SFXBusVolumeDefaultValue;
            }

            if (PlayerPrefs.HasKey(CharacterDelayValueKey))
            {
                CharacterDelay = PlayerPrefs.GetFloat(CharacterDelayValueKey);
            }
            else
            {
                CharacterDelay = CharacterDelayDefaultValue;
            }

            //if (PlayerPrefs.HasKey(CharacterDelayValueKey))
            //    CharacterDelay = PlayerPrefs.GetFloat(CharacterDelayValueKey);
        }

        public static void SetPlayerPrefs()
        {
            PlayerPrefs.SetFloat(MasterBusVolumeKey, MasterVolume);
            PlayerPrefs.SetFloat(BGMBusVolumeKey, BGMVolume);
            PlayerPrefs.SetFloat(SFXBusVolumeKey, SFXVolume);
            PlayerPrefs.SetFloat(CharacterDelayValueKey, CharacterDelay);
        }

        #region Setters

        public static void SetMasterVolume(float value)
        {
            MasterVolume = value;
            PlayerPrefs.SetFloat(MasterBusVolumeKey, value);
        }

        public static void SetBGMVolume(float value)
        {
            BGMVolume = value;
            PlayerPrefs.SetFloat(BGMBusVolumeKey, value);
        }

        public static void SetSFXVolume(float value)
        {
            SFXVolume = value;
            PlayerPrefs.SetFloat(SFXBusVolumeKey, value);
        }

        public static void SetCharacterDelay(float value)
        {
            CharacterDelay = value;
            PlayerPrefs.SetFloat(CharacterDelayValueKey, value);
        }

        //public static void SetShowCluesAlerts(bool value)
        //{
        //    ShowCluesAlerts = value;
        //    PlayerPrefs.SetBool(MasterBusVolumeKey, value);
        //}

        #endregion

        #region Getters

        public static float GetMasterVolume() { return MasterVolume; }
        public static float GetBGMVolume() { return BGMVolume; }
        public static float GetSFXVolume() { return SFXVolume; }
        public static float GetCharacterDelay() { return CharacterDelay; }

        #endregion
    }
}